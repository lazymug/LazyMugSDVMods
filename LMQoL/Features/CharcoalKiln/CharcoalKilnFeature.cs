using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Machines;

namespace LMQoL.Features.CharcoalKiln
{
    /// <summary>Lets the charcoal kiln burn more than just a stack of wood.
    ///
    /// This is the old "[PFM] Custom Charcoal Kiln" content pack rebuilt on the vanilla 1.6
    /// machine data, so it no longer needs Producer Framework Mod: the rules are written straight
    /// into <c>Data/Machines</c> for <c>(BC)114</c>.</summary>
    public class CharcoalKilnFeature : IFeature
    {
        private const string KilnId = "(BC)114";
        private const string Coal = "(O)382";

        /// <summary>input item, how many it takes, coal produced, minutes to burn.</summary>
        private static readonly (string ItemId, int Count, int Coal, int Minutes)[] Recipes =
        {
            ("(O)169", 1, 1, 40),    // Driftwood
            ("(O)93", 1, 3, 30),     // Torch
            ("(O)388", 1, 5, 60),    // Wood
            ("(O)709", 1, 45, 550),  // Hardwood
        };

        private IModHelper _helper = null!;

        public string Id => "CharcoalKiln";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (!ModEntry.Config.CharcoalKilnEnabled || !e.NameWithoutLocale.IsEquivalentTo("Data/Machines"))
                return;

            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, MachineData>().Data;
                if (!data.TryGetValue(KilnId, out var kiln))
                    return;

                // replace the stock "10 wood -> 1 coal" rule set outright
                kiln.OutputRules = new List<MachineOutputRule>();
                foreach (var (itemId, count, coal, minutes) in Recipes)
                {
                    kiln.OutputRules.Add(new MachineOutputRule
                    {
                        Id = $"LMQoL_Kiln_{itemId}",
                        Triggers = new List<MachineOutputTriggerRule>
                        {
                            new()
                            {
                                Id = $"LMQoL_Kiln_{itemId}_Trigger",
                                Trigger = MachineOutputTrigger.ItemPlacedInMachine,
                                RequiredItemId = itemId,
                                RequiredCount = count,
                            },
                        },
                        OutputItem = new List<MachineItemOutput>
                        {
                            new() { ItemId = Coal, MinStack = coal, MaxStack = coal },
                        },
                        MinutesUntilReady = minutes,
                    });
                }
            }, AssetEditPriority.Late);
        }

        /// <summary>Refresh the machine data when the setting is toggled mid-session.</summary>
        public void Reapply() => _helper.GameContent.InvalidateCache("Data/Machines");
    }
}
