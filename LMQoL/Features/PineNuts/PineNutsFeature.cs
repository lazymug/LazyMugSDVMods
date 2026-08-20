using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;

namespace LMQoL.Features.PineNuts
{
    /// <summary>Lets Cornucopia's Compact Mill press pine nuts from other tree seeds, not just
    /// pine cones.
    ///
    /// Rather than defining a new rule, this appends triggers to Cornucopia's own
    /// <c>Cornucopia_PineNuts</c> rule, so the yield and the 2250-minute press time stay exactly
    /// what that mod set — including if it rebalances them later. Editing Cornucopia's files
    /// directly would work too, but the change would be lost on its next update.</summary>
    public class PineNutsFeature : IFeature
    {
        private const string CornucopiaId = "Cornucopia.ArtisanMachines";
        private const string SveId = "FlashShifter.StardewValleyExpandedCP";
        private const string CompactMill = "(BC)Cornucopia_CompactMill";
        private const string PineNutsRuleId = "Cornucopia_PineNuts";

        /// <summary>Vanilla tree seeds to accept. Pine cones are already handled by Cornucopia.</summary>
        private static readonly string[] VanillaSeeds =
        {
            "(O)309",   // Acorn — oak
            "(O)310",   // Maple Seed
        };

        /// <summary>Stardew Valley Expanded's own trees.</summary>
        private static readonly string[] SveSeeds =
        {
            "(O)FlashShifter.StardewValleyExpandedCP_Birch_Seed",
            "(O)FlashShifter.StardewValleyExpandedCP_Fir_Cone",
        };

        private IModHelper _helper = null!;
        private bool _sveLoaded;

        public string Id => "PineNuts";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;

            if (!helper.ModRegistry.IsLoaded(CornucopiaId))
                return;   // the Compact Mill doesn't exist without it

            _sveLoaded = helper.ModRegistry.IsLoaded(SveId);
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (!ModEntry.Config.PineNutsFromTreeSeedsEnabled || !e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
                return;

            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, MachineData>().Data;
                if (!data.TryGetValue(CompactMill, out var mill) || mill.OutputRules == null)
                    return;

                var rule = mill.OutputRules.Find(r => r?.Id == PineNutsRuleId);
                if (rule?.Triggers == null)
                    return;   // Cornucopia changed its rule; leave well alone

                var seeds = new List<string>(VanillaSeeds);
                if (_sveLoaded)
                    seeds.AddRange(SveSeeds);

                foreach (string seed in seeds)
                {
                    if (rule.Triggers.Exists(t => t?.RequiredItemId == seed))
                        continue;

                    rule.Triggers.Add(new MachineOutputTriggerRule
                    {
                        Id = $"LMQoL_PineNuts_{seed}",
                        Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                        RequiredItemId = seed,
                        RequiredCount = 1,
                    });
                }
            }, AssetEditPriority.Late);
        }

        /// <summary>Refresh the machine data when the setting is toggled mid-session.</summary>
        public void Reapply() => _helper?.GameContent.InvalidateCache("Data/Machines");
    }
}
