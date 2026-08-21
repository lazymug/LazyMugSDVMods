using System.Collections.Generic;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace LMQoL.Features.BuildWithBags
{
    /// <summary>Mirror of the parts of Item Bags' public API we use.
    ///
    /// This is deliberately OUR OWN interface, not a reference to ItemBags.dll: SMAPI builds a
    /// proxy for it at runtime, so the mod still loads (and this feature simply stays off) when
    /// Item Bags isn't installed. Referencing their assembly directly would make SMAPI's Cecil
    /// rewriter fail to resolve it and refuse to load the whole mod.
    ///
    /// Only overloads whose parameters are vanilla types can be mirrored — hence the Item-typed
    /// bag overloads rather than the ItemBag-typed ones.</summary>
    public interface IItemBagsApi
    {
        bool IsItemBag(Item item);

        IList<GenericTool> GetItemBags(IList<Item> source);

        IList<Object> GetObjectsInsideBag(Item bag, bool includeNestedBags);

        bool TryRemoveObjectFromBag(Item bag, Object item, int quantity, IList<Item> targetContainer,
            int targetContainerCapacity, bool playSoundEffect, out int movedQty);

        bool TryMoveObjectToBag(Object item, int quantity, IList<Item> itemSourceContainer, Item target,
            out int movedQty, bool playSoundEffect);
    }
}
