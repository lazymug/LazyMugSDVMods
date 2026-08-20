using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LMQoL.Features.SellPriceTooltip
{
    /// <summary>Keeps the machine scan in step with the loaded content.</summary>
    public class SellPriceTooltipFeature : IFeature
    {
        public string Id => "SellPriceTooltip";

        private const string CornucopiaId = "Cornucopia.ArtisanMachines";
        private const string WildflourId = "Wildflour.AtelierGoods";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            // Detect the mods we surface a dedicated toggle for. The scan itself is generic — it
            // reads Data/Machines — but knowing which of these are present lets the options only
            // appear when they mean something, and lets each be switched off on its own.
            MachineScanner.CornucopiaLoaded = helper.ModRegistry.IsLoaded(CornucopiaId);
            MachineScanner.WildflourLoaded = helper.ModRegistry.IsLoaded(WildflourId);

            if (MachineScanner.CornucopiaLoaded)
                monitor.Log("Cornucopia detected — its machines will appear in the sell price tooltip.", LogLevel.Debug);
            if (MachineScanner.WildflourLoaded)
                monitor.Log("Wildflour's Atelier Goods detected — its machines will appear in the sell price tooltip.", LogLevel.Debug);

            // recipes come from Data/Machines, which content packs can reload at any time
            helper.Events.Content.AssetsInvalidated += (_, e) =>
            {
                foreach (var name in e.NamesWithoutLocale)
                {
                    if (name.IsEquivalentTo("Data/Machines"))
                    {
                        MachineScanner.ClearCache();
                        break;
                    }
                }
            };
            helper.Events.GameLoop.SaveLoaded += (_, _) => MachineScanner.ClearCache();
        }
    }
}
