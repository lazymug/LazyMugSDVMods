using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using WeatheredLetters.Data;

namespace WeatheredLetters
{
    public class ModEntry : Mod
    {
        internal static ModEntry Instance { get; private set; } = null!;
        internal static ModConfig Config { get; private set; } = null!;
        internal static CollectionTracker? Tracker { get; private set; }

        private LetterSpawner _spawner = null!;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Content.AssetRequested += OnAssetRequested;

            helper.ConsoleCommands.Add("wl_status", "Show Weathered Letters collection status.", OnStatusCommand);
            helper.ConsoleCommands.Add("wl_spawn", "Force spawn the next letter.", OnSpawnCommand);
            helper.ConsoleCommands.Add("wl_spawn_here", "Spawn a letter item next to the player. Usage: wl_spawn_here [letter_id]", OnSpawnHereCommand);
            helper.ConsoleCommands.Add("wl_read", "Open a letter to read. Usage: wl_read <letter_id> | Use 'wl_read list' to see all IDs.", OnReadCommand);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.EnableMod,
                setValue: v => Config.EnableMod = v,
                name: () => Helper.Translation.Get("config.enablemod").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DaysBetweenLetters,
                setValue: v => Config.DaysBetweenLetters = v,
                name: () => Helper.Translation.Get("config.daysbetween").ToString(),
                tooltip: () => Helper.Translation.Get("config.daysbetween.tooltip").ToString(),
                min: 1,
                max: 28,
                interval: 1
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.ShowSpawnHints,
                setValue: v => Config.ShowSpawnHints = v,
                name: () => Helper.Translation.Get("config.showhints").ToString()
            );
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Tracker = new CollectionTracker(Helper);
            Tracker.Load();
            _spawner = new LetterSpawner(Helper, Monitor, Tracker, Config);
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Config.EnableMod || Tracker == null)
                return;

            _spawner.RecoverLostLetter();
            _spawner.TrySpawnLetter();
            CheckReward();
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!Config.EnableMod || Tracker == null || !e.IsLocalPlayer)
                return;

            // Spawn letter in mines when player enters
            if (e.NewLocation.Name.StartsWith("UndergroundMine") || e.NewLocation.Name.StartsWith("SkullCave"))
                _spawner.SpawnInMines(e.NewLocation);
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (!Config.EnableMod || Tracker == null || !e.IsLocalPlayer)
                return;

            if (Tracker.PendingLetterId == null)
                return;

            // Check if player picked up the note item from the ground (farm/beach/mines forage)
            foreach (var added in e.Added)
            {
                if (added.ItemId == LetterSpawner.LetterItemId
                    && added.modData.ContainsKey(LetterSpawner.ModDataKey))
                {
                    var letter = LetterRegistry.AllLetters.Find(l => l.Id == Tracker.PendingLetterId);
                    if (letter != null && (letter.LocationType == Models.SpawnLocationType.Farm
                                        || letter.LocationType == Models.SpawnLocationType.Beach
                                        || letter.LocationType == Models.SpawnLocationType.Mines))
                    {
                        OnLetterCollected();
                    }
                    break;
                }
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Config.EnableMod || Tracker == null)
                return;

            if (!e.Button.IsActionButton() && !e.Button.IsUseToolButton())
                return;

            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
                return;

            // Check if player is using/selecting a weathered letter in inventory
            var heldItem = Game1.player.CurrentItem;
            if (heldItem == null || heldItem.ItemId != LetterSpawner.LetterItemId
                || !heldItem.modData.ContainsKey(LetterSpawner.ModDataKey))
                return;

            // Find the most recently found letter to display
            var lastFound = LetterRegistry.AllLetters
                .Where(l => Tracker.IsFound(l.Id))
                .OrderByDescending(l => l.Order)
                .FirstOrDefault();

            if (lastFound == null)
                return;

            Helper.Input.Suppress(e.Button);

            string content = Helper.Translation.Get(lastFound.TranslationKey).ToString();
            string title = Helper.Translation.Get("letter.title", new { author = lastFound.Author }).ToString();

            Game1.activeClickableMenu = new LetterViewerMenu(title + "^^" + content);

            // Consume the item
            Game1.player.removeItemFromInventory(heldItem);
        }

        internal void OnLetterCollected()
        {
            if (Tracker?.PendingLetterId == null)
                return;

            var letter = LetterRegistry.AllLetters.Find(l => l.Id == Tracker.PendingLetterId);
            if (letter == null)
                return;

            Tracker.MarkFound(letter.Id, Game1.Date.TotalDays);

            string message = Helper.Translation.Get("found.message", new
            {
                current = Tracker.FoundCount,
                total = Tracker.TotalCount
            }).ToString();
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type));

            // Immediately open the letter
            string content = Helper.Translation.Get(letter.TranslationKey).ToString();
            string title = Helper.Translation.Get("letter.title", new { author = letter.Author }).ToString();
            Game1.activeClickableMenu = new LetterViewerMenu(title + "^^" + content);

            Monitor.Log($"Letter '{letter.Id}' collected. {Tracker.FoundCount}/{Tracker.TotalCount}", LogLevel.Info);
        }

        private void CheckReward()
        {
            if (Tracker == null || !Tracker.AllFound || Tracker.RewardGiven)
                return;

            Tracker.RewardGiven = true;

            // Send final reward via mail
            Game1.player.mailForTomorrow.Add("WL_FinalReward");

            string message = Helper.Translation.Get("reward.message").ToString();
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.achievement_type));

            Monitor.Log("All letters found! Reward delivered.", LogLevel.Info);
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    string rewardText = Helper.Translation.Get("letter.reward").ToString();
                    data["WL_FinalReward"] = rewardText + " %item object 74 1 %%";
                });
            }
        }

        private void OnStatusCommand(string command, string[] args)
        {
            if (Tracker == null)
            {
                Monitor.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            Monitor.Log($"Weathered Letters: {Tracker.FoundCount}/{Tracker.TotalCount} collected", LogLevel.Info);
            Monitor.Log($"Pending letter: {Tracker.PendingLetterId ?? "none"}", LogLevel.Info);
            Monitor.Log($"Reward given: {Tracker.RewardGiven}", LogLevel.Info);

            foreach (var letter in LetterRegistry.AllLetters)
            {
                string status = Tracker.IsFound(letter.Id) ? "FOUND" : "not found";
                Monitor.Log($"  [{status}] #{letter.Order} {letter.Id} ({letter.Author}) - {letter.LocationType}", LogLevel.Info);
            }
        }

        private void OnSpawnCommand(string command, string[] args)
        {
            if (Tracker == null || !Context.IsWorldReady)
            {
                Monitor.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            // Reset cooldown and force spawn
            _spawner.TrySpawnLetter();
            if (Tracker.PendingLetterId == null)
            {
                // Force by bypassing cooldown
                var letter = Tracker.GetNextLetter(Game1.currentSeason);
                if (letter == null)
                {
                    Monitor.Log("All letters found!", LogLevel.Info);
                    return;
                }

                Tracker.PendingLetterId = letter.Id;
                Monitor.Log($"Force-set pending letter: {letter.Id} ({letter.LocationType}). If Farm/Beach, it will spawn tomorrow.", LogLevel.Info);

                // For immediate testing: give item directly
                var noteItem = ItemRegistry.Create(LetterSpawner.LetterItemId, 1);
                noteItem.modData[LetterSpawner.ModDataKey] = LetterSpawner.ModDataValue;
                Game1.player.addItemByMenuIfNecessary(noteItem);
                Monitor.Log("Added letter item to inventory for testing. Use it to read.", LogLevel.Info);
            }
            else
            {
                Monitor.Log($"Pending letter already exists: {Tracker.PendingLetterId}", LogLevel.Info);
            }
        }
        private void OnSpawnHereCommand(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            string? letterId = args.Length > 0 ? args[0] : null;
            var letter = letterId != null
                ? LetterRegistry.AllLetters.Find(l => l.Id == letterId)
                : LetterRegistry.AllLetters.FirstOrDefault();

            if (letter == null)
            {
                Monitor.Log($"Letter '{letterId}' not found. Use 'wl_read list' to see all IDs.", LogLevel.Warn);
                return;
            }

            var playerTile = Game1.player.Tile;
            var location = Game1.player.currentLocation;

            // Try adjacent tiles around the player
            Vector2[] offsets = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1), new(1, 1), new(-1, -1), new(1, -1), new(-1, 1) };
            Vector2? spawnTile = null;
            foreach (var offset in offsets)
            {
                var candidate = playerTile + offset;
                if (!location.Objects.ContainsKey(candidate))
                {
                    spawnTile = candidate;
                    break;
                }
            }

            if (spawnTile == null)
            {
                Monitor.Log("No open tile found near the player.", LogLevel.Warn);
                return;
            }

            var noteObj = new StardewValley.Object(LetterSpawner.LetterItemId, 1)
            {
                TileLocation = spawnTile.Value,
                IsSpawnedObject = true
            };
            noteObj.modData[LetterSpawner.ModDataKey] = LetterSpawner.ModDataValue;
            location.Objects[spawnTile.Value] = noteObj;

            if (Tracker != null)
            {
                Tracker.PendingLetterId = letter.Id;
            }

            Monitor.Log($"Spawned letter '{letter.Id}' ({letter.Author}) at {spawnTile.Value} in {location.Name}.", LogLevel.Info);
        }

        private void OnReadCommand(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            if (args.Length == 0 || args[0] == "list")
            {
                Monitor.Log("Available letters:", LogLevel.Info);
                foreach (var l in LetterRegistry.AllLetters)
                {
                    Monitor.Log($"  {l.Id} — {l.Author} ({l.LocationType}, {l.SeasonHint ?? "any season"})", LogLevel.Info);
                }
                Monitor.Log("Usage: wl_read <letter_id>", LogLevel.Info);
                return;
            }

            var letter = LetterRegistry.AllLetters.Find(l => l.Id == args[0]);
            if (letter == null)
            {
                Monitor.Log($"Letter '{args[0]}' not found. Use 'wl_read list' to see all IDs.", LogLevel.Warn);
                return;
            }

            string content = Helper.Translation.Get(letter.TranslationKey).ToString();
            string title = Helper.Translation.Get("letter.title", new { author = letter.Author }).ToString();
            Game1.activeClickableMenu = new LetterViewerMenu(title + "^^" + content);

            Monitor.Log($"Opened letter '{letter.Id}' from {letter.Author}.", LogLevel.Info);
        }
    }
}
