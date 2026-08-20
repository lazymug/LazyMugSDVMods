using System;
using System.Reflection;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace LMQoL.Features.ItemTotals
{
    /// <summary>While an Item Bag is open, shows how many of the hovered item you own in total —
    /// carried, stored in chests, and tucked away in other bags.
    ///
    /// The bag menu is reached by type name and reflection rather than a reference to ItemBags.dll,
    /// for the same reason as the build integration: a hard reference stops LM QoL loading when
    /// Item Bags isn't installed.</summary>
    public class ItemTotalsFeature : IFeature
    {
        private const string BagMenuType = "ItemBags.Menus.ItemBagMenu";

        private ITranslationHelper _i18n = null!;
        private FieldInfo? _hoveredField;
        private Type? _resolvedFor;

        // cache: recounting walks every location, so only redo it when the hovered item changes
        private string? _cachedId;
        private ItemTally _cached;

        public string Id => "ItemTotals";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _i18n = helper.Translation;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
            helper.Events.Display.MenuChanged += (_, _) => _cachedId = null;
        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.ItemTotalsEnabled || !Context.IsWorldReady)
                return;

            var menu = Game1.activeClickableMenu;
            if (menu == null || menu.GetType().FullName != BagMenuType)
                return;

            Item? hovered = ReadHoveredItem(menu);
            if (hovered == null)
            {
                _cachedId = null;
                return;
            }

            string id = hovered.QualifiedItemId;
            if (id != _cachedId)
            {
                _cached = ItemTally.Count(id, config.ItemTotalsIncludeChests, config.ItemTotalsIncludeBags);
                _cachedId = id;
            }

            IClickableMenu.drawHoverText(e.SpriteBatch, Format(_cached), Game1.smallFont,
                overrideX: 32, overrideY: Game1.uiViewport.Height - 160);
        }

        private string Format(ItemTally tally)
        {
            var sb = new StringBuilder(_i18n.Get("totals.total", new { count = tally.Total }).ToString());
            sb.Append('\n').Append(_i18n.Get("totals.carried", new { count = tally.Carried }));
            if (ModEntry.Config.ItemTotalsIncludeChests)
                sb.Append('\n').Append(_i18n.Get("totals.chests", new { count = tally.InChests }));
            if (ModEntry.Config.ItemTotalsIncludeBags && BuildWithBags.BuildWithBagsFeature.Api != null)
                sb.Append('\n').Append(_i18n.Get("totals.bags", new { count = tally.InBags }));
            return sb.ToString();
        }

        /// <summary>ItemBagMenu.HoveredItem, resolved once per menu type.</summary>
        private Item? ReadHoveredItem(IClickableMenu menu)
        {
            var type = menu.GetType();
            if (!ReferenceEquals(type, _resolvedFor))
            {
                _hoveredField = type.GetField("HoveredItem", BindingFlags.Public | BindingFlags.Instance);
                _resolvedFor = type;
            }

            return _hoveredField?.GetValue(menu) as Item;
        }
    }
}
