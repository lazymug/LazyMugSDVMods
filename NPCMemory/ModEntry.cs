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
        private NewsletterGenerator _newsletterGenerator = null!;
        private INewsletterMailer? _newsletterMailer;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _tracker = new ActivityTracker(Monitor, _config);
            _memoryStore = new MemoryStore(helper);
            _dialogueGenerator = new DialogueGenerator(helper, _memoryStore);
            _newsletterGenerator = new NewsletterGenerator(helper, _memoryStore, _config);

            // Soft dependency: MailerFactory.Create is [NoInlining] so MFM types are only JIT'd
            // when the factory method is actually called — never when Entry() is compiled.
            if (helper.ModRegistry.IsLoaded("DIGUS.MailFrameworkMod"))
                _newsletterMailer = MailerFactory.Create(ModManifest);

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
                "  npcmemory_simulate newsletter             — Generate & register the newsletter now\n" +
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

            gmcm.AddBoolOption(
                mod: ModManifest,
                getValue: () => _config.NewsletterEnabled,
                setValue: v => _config.NewsletterEnabled = v,
                name: () => Helper.Translation.Get("config.newsletter.enabled").ToString(),
                tooltip: () => Helper.Translation.Get("config.newsletter.enabled.tooltip").ToString()
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.NewsletterCadenceDays,
                setValue: v => _config.NewsletterCadenceDays = v,
                name: () => Helper.Translation.Get("config.newsletter.cadence").ToString(),
                tooltip: () => Helper.Translation.Get("config.newsletter.cadence.tooltip").ToString(),
                min: 1,
                max: 28,
                interval: 1
            );

            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => _config.NewsletterMaxHeadlines,
                setValue: v => _config.NewsletterMaxHeadlines = v,
                name: () => Helper.Translation.Get("config.newsletter.headlines").ToString(),
                tooltip: () => Helper.Translation.Get("config.newsletter.headlines.tooltip").ToString(),
                min: 1,
                max: 8,
                interval: 1
            );
        }

        /// <summary>
        /// Regenerates the current week's newsletter and (re)registers it with MailFrameworkMod.
        /// Safe to call every day: the letter id is stable per week, and MailFrameworkMod delivers
        /// it only on a cadence day. No-ops when MailFrameworkMod isn't installed.
        /// </summary>
        private void RefreshNewsletter()
        {
            if (!_config.EnableMod || !_config.NewsletterEnabled || _newsletterMailer == null)
                return;

            int cadence = System.Math.Max(1, _config.NewsletterCadenceDays);
            long daysPlayed = System.Math.Max(1, (long)Game1.stats.DaysPlayed);
            long weekKey = (daysPlayed - 1) / cadence;

            string? content = _newsletterGenerator.Generate();
            if (content == null)
                return;

            _newsletterMailer.Register(
                content,
                weekKey.ToString(),
                () => Game1.stats.DaysPlayed % (uint)cadence == 0
            );
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            _memoryStore.Load();
            _tracker.StartNewDay();
            RefreshNewsletter();
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!_config.EnableMod)
                return;

            InjectDialogues();
            RefreshNewsletter();
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

            if (subCommand == "newsletter")
            {
                string? content = _newsletterGenerator.Generate();
                if (content == null)
                {
                    Monitor.Log("No activity recorded yet — nothing to put in the newsletter. Try 'npcmemory_simulate all' first.", LogLevel.Warn);
                    return;
                }

                Monitor.Log("Generated newsletter:\n" + content.Replace("^", "\n"), LogLevel.Info);

                if (_newsletterMailer == null)
                {
                    Monitor.Log("MailFrameworkMod is not installed, so the newsletter can't be delivered to your mailbox.", LogLevel.Warn);
                    return;
                }

                _newsletterMailer.Register(content, $"test.{Game1.stats.DaysPlayed}", () => true);
                Monitor.Log("Newsletter registered for immediate delivery. Sleep to receive it in your mailbox.", LogLevel.Info);
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
