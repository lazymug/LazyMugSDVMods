using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ValleyTriad.Data;
using ValleyTriad.Game;
using ValleyTriad.Models;
using ValleyTriad.UI;

namespace ValleyTriad
{
    public class ModEntry : Mod
    {
        private ModConfig _config = null!;
        private readonly CardDatabase _cards = new();

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _cards.Load(helper, Monitor);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;

            helper.ConsoleCommands.Add("vt_test",
                "Run a headless Valley Triad engine self-test (placement + capture rules).", (_, _) => SelfTest());
            helper.ConsoleCommands.Add("vt_open",
                "Open the (WIP) Valley Triad board menu. Requires a loaded save.", (_, _) => OpenMenu());
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;

            gmcm.Register(ModManifest, () => _config = new ModConfig(), () => Helper.WriteConfig(_config));
            string T(string k) => Helper.Translation.Get(k);

            gmcm.AddBoolOption(ModManifest, () => _config.EnableMod, v => _config.EnableMod = v, () => T("config.enablemod"));
            gmcm.AddSectionTitle(ModManifest, () => T("config.rules"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleSame, v => _config.RuleSame = v, () => T("config.same"));
            gmcm.AddBoolOption(ModManifest, () => _config.RulePlus, v => _config.RulePlus = v, () => T("config.plus"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleCombo, v => _config.RuleCombo = v, () => T("config.combo"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleElemental, v => _config.RuleElemental = v, () => T("config.elemental"));
            gmcm.AddNumberOption(ModManifest, () => _config.ElementalCells, v => _config.ElementalCells = v, () => T("config.elementalcells"), null, 0, 6, 1);
            gmcm.AddSectionTitle(ModManifest, () => T("config.stakes"));
            gmcm.AddTextOption(ModManifest,
                () => _config.Stakes.ToString(),
                v => _config.Stakes = Enum.TryParse<StakeMode>(v, out var m) ? m : StakeMode.Friendly,
                () => T("config.stakemode"), null,
                Enum.GetNames(typeof(StakeMode)));
        }

        private void OpenMenu()
        {
            if (!Context.IsWorldReady) { Monitor.Log("Load a save first.", LogLevel.Warn); return; }
            Game1.activeClickableMenu = new TriadMenu();
        }

        /// <summary>Headless validation of the capture engine (no save required).</summary>
        private void SelfTest()
        {
            var board = new Board(_config.RuleSame, _config.RulePlus, _config.RuleCombo, _config.RuleElemental);
            Card? p2 = _cards.Get("parsnip");
            Card? p1 = _cards.Get("pumpkin");
            if (p1 == null || p2 == null) { Monitor.Log("Card data missing; can't run self-test.", LogLevel.Error); return; }

            board.Place(p2, Owner.P2, 0, 1);                     // opponent card above centre (S edge = 2)
            var captured = board.Place(p1, Owner.P1, 1, 1);      // pumpkin N edge = 6 > 2 -> capture

            Monitor.Log($"Placed {p2} (P2) at (0,1) and {p1} (P1) at (1,1).", LogLevel.Info);
            Monitor.Log($"Captured by P1: [{string.Join(", ", captured.Select(t => $"({t.r},{t.c})"))}]", LogLevel.Info);
            Monitor.Log($"Board count — P1: {board.Count(Owner.P1)}, P2: {board.Count(Owner.P2)} " +
                        $"(expected P1:2, P2:0). Rules: Same={_config.RuleSame} Plus={_config.RulePlus} " +
                        $"Combo={_config.RuleCombo} Elem={_config.RuleElemental}",
                        captured.Count == 1 && board.Count(Owner.P1) == 2 ? LogLevel.Info : LogLevel.Warn);
        }
    }
}
