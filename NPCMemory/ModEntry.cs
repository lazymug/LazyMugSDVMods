using System;
using System.Linq;
using NPCMemory.Data;
using NPCMemory.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NPCMemory
{
    public class ModEntry : Mod
    {
        private ModConfig _config = null!;
        private ActivityTracker _tracker = null!;
        private MemoryStore _memoryStore = null!;
        private DialogueGenerator _dialogueGenerator = null!;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _tracker = new ActivityTracker(Monitor, _config);
            _memoryStore = new MemoryStore(helper);
            _dialogueGenerator = new DialogueGenerator(helper, _memoryStore);

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.Player.Warped += OnWarped;

            helper.ConsoleCommands.Add("npcmemory_simulate",
                "Simulate activities for NPC Memory.\n" +
                "Usage:\n" +
                "  npcmemory_simulate all                    — Simulate all activity types\n" +
                "  npcmemory_simulate <activity> <count> [item] — Simulate a specific activity\n" +
                "  npcmemory_simulate inject                 — Force inject dialogues now\n" +
                "Activities: fishing, mining, farming, cooking, combat, foraging, animalcare,\n" +
                "            artisangoods, flowers, beverages, sweets, technology, voiditems, magic",
                OnSimulateCommand);
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => _config = new ModConfig(),
                save: () => Helper.WriteConfig(_config)
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.EnableMod,
                setValue: v => _config.EnableMod = v,
                name: () => Helper.Translation.Get("config.enablemod").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.DialogueChance,
                setValue: v => _config.DialogueChance = v,
                name: () => Helper.Translation.Get("config.dialoguechance").ToString(),
                tooltip: () => Helper.Translation.Get("config.dialoguechance.tooltip").ToString(),
                min: 0.1f,
                max: 1.0f,
                interval: 0.1f
            );

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.ShowDebugMessages,
                setValue: v => _config.ShowDebugMessages = v,
                name: () => Helper.Translation.Get("config.showdebug").ToString()
            );
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            _memoryStore.Load();
            _tracker.StartNewDay();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!_config.EnableMod)
                return;

            InjectDialogues();
            _tracker.StartNewDay();
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            if (!_config.EnableMod)
                return;

            var today = _tracker.GetTodayActivity();
            _memoryStore.PushDay(today);
            _memoryStore.Save();
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (!_config.EnableMod || !e.IsLocalPlayer)
                return;

            _tracker.OnInventoryChanged(e);
        }

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!_config.EnableMod || !e.IsLocalPlayer)
                return;

            _tracker.OnWarped(e);
        }

        private void OnSimulateCommand(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("You must load a save first.", LogLevel.Warn);
                return;
            }

            if (args.Length == 0)
            {
                Monitor.Log("Usage: npcmemory_simulate all | inject | <activity> <count> [item]", LogLevel.Info);
                return;
            }

            string subCommand = args[0].ToLower();

            if (subCommand == "all")
            {
                _tracker.SimulateAll();
                var today = _tracker.GetTodayActivity();
                _memoryStore.PushDay(today);
                Monitor.Log("Simulated all activities and saved to memory. Use 'npcmemory_simulate inject' to inject dialogues now.", LogLevel.Info);
                foreach (var kvp in today.Activities)
                    Monitor.Log($"  {kvp.Key}: count={kvp.Value.Count}, items=[{string.Join(", ", kvp.Value.NotableItems)}]", LogLevel.Info);
                return;
            }

            if (subCommand == "inject")
            {
                float savedChance = _config.DialogueChance;
                _config.DialogueChance = 1.0f;
                InjectDialogues();
                _config.DialogueChance = savedChance;
                Monitor.Log("Dialogues injected with 100% chance. Talk to NPCs now!", LogLevel.Info);
                return;
            }

            // Parse specific activity: npcmemory_simulate fishing 10 Salmon
            if (!Enum.TryParse<ActivityType>(subCommand, true, out var activityType))
            {
                Monitor.Log($"Unknown activity '{subCommand}'. Valid: {string.Join(", ", Enum.GetNames(typeof(ActivityType)).Select(n => n.ToLower()))}", LogLevel.Warn);
                return;
            }

            int count = 5;
            if (args.Length >= 2 && int.TryParse(args[1], out int parsed))
                count = parsed;

            string? itemName = args.Length >= 3 ? string.Join(" ", args.Skip(2)) : null;

            _tracker.SimulateActivity(activityType, count, itemName);
            var activity = _tracker.GetTodayActivity();
            _memoryStore.PushDay(activity);
            Monitor.Log($"Simulated {activityType}: count={count}, item={itemName ?? "(none)"}. Saved to memory.", LogLevel.Info);
            Monitor.Log("Use 'npcmemory_simulate inject' to inject dialogues now.", LogLevel.Info);
        }

        private void InjectDialogues()
        {
            var yesterday = _memoryStore.GetYesterday();
            if (yesterday == null)
                return;

            foreach (var npc in Utility.getAllVillagers())
            {
                string? dialogueText = _dialogueGenerator.GenerateDialogue(npc.Name);
                if (dialogueText == null)
                    continue;

                // Roll the dice — not every NPC comments every day
                if (Game1.random.NextDouble() > _config.DialogueChance)
                    continue;

                var dialogue = new Dialogue(npc, null, dialogueText);
                npc.CurrentDialogue.Push(dialogue);

                if (_config.ShowDebugMessages)
                    Monitor.Log($"[DialogueInjection] {npc.Name}: {dialogueText}", LogLevel.Debug);
            }
        }
    }
}
