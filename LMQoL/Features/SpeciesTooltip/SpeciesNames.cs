using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;

namespace LMQoL.Features.SpeciesTooltip
{
    /// <summary>Resolves a readable species name for crops, bushes and trees.</summary>
    internal static class SpeciesNames
    {
        /// <summary>Wild tree ids (Tree.bushyTree etc.) have no display name in Data/WildTrees,
        /// so the vanilla ones are named here; anything else falls back to its seed's name.</summary>
        private static readonly Dictionary<string, string> WildTreeKeys = new()
        {
            [Tree.bushyTree] = "species.tree.oak",
            [Tree.leafyTree] = "species.tree.maple",
            [Tree.pineTree] = "species.tree.pine",
            [Tree.winterTree1] = "species.tree.mystic_winter1",
            [Tree.winterTree2] = "species.tree.mystic_winter2",
            [Tree.palmTree] = "species.tree.palm",
            [Tree.palmTree2] = "species.tree.palm",
            [Tree.mushroomTree] = "species.tree.mushroom",
            [Tree.mahoganyTree] = "species.tree.mahogany",
            [Tree.greenRainTreeBushy] = "species.tree.greenrain",
            [Tree.greenRainTreeLeafy] = "species.tree.greenrain",
            [Tree.greenRainTreeFern] = "species.tree.greenrain",
            [Tree.mysticTree] = "species.tree.mystic",
        };

        public static string ForWildTree(ITranslationHelper i18n, Tree tree)
        {
            string type = tree.treeType.Value ?? "";
            if (WildTreeKeys.TryGetValue(type, out string? key))
                return i18n.Get(key);

            // modded tree: name it after its seed, which is the best label the data gives us
            if (Tree.TryGetData(type, out var data) && !string.IsNullOrEmpty(data.SeedItemId))
                return ItemName(data.SeedItemId);

            return i18n.Get("species.tree.unknown");
        }

        public static string ForFruitTree(ITranslationHelper i18n, FruitTree tree)
        {
            var data = tree.GetData();
            if (data != null && !string.IsNullOrEmpty(data.DisplayName))
                return TokenParser.ParseText(data.DisplayName);

            return i18n.Get("species.tree.unknown");
        }

        public static string ForBush(ITranslationHelper i18n, Bush bush)
        {
            switch (bush.size.Value)
            {
                case Bush.greenTeaBush: return i18n.Get("species.bush.tea");
                case Bush.walnutBush: return i18n.Get("species.bush.walnut");
                default: return i18n.Get("species.bush.berry");
            }
        }

        public static string ItemName(string itemId)
        {
            var data = ItemRegistry.GetData(itemId);
            return data?.DisplayName ?? itemId;
        }
    }
}
