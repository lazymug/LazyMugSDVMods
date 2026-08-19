using System.Text;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace LMQoL.Features.SpeciesTooltip
{
    /// <summary>Shows what a crop, bush or tree actually is when you hover over it.</summary>
    public class SpeciesTooltipFeature : IFeature
    {
        private ITranslationHelper _i18n = null!;

        public string Id => "SpeciesTooltip";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _i18n = helper.Translation;
            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.SpeciesTooltipEnabled || !Context.IsWorldReady)
                return;

            // don't fight menus, cutscenes or the vanilla item tooltip
            if (Game1.activeClickableMenu != null || Game1.eventUp || Game1.player.ActiveObject != null)
                return;

            var location = Game1.currentLocation;
            if (location == null)
                return;

            string? text = Describe(location, Game1.currentCursorTile);
            if (text == null)
                return;

            IClickableMenu.drawHoverText(e.SpriteBatch, text, Game1.smallFont);
        }

        /// <summary>Tiles below the cursor that still count as the same tree: a grown canopy is
        /// drawn above its base tile, so hovering the leaves must find the trunk underneath.</summary>
        private const int CanopyReach = 2;

        private string? Describe(GameLocation location, Vector2 tile)
        {
            if (location.terrainFeatures.TryGetValue(tile, out var feature))
            {
                switch (feature)
                {
                    case HoeDirt dirt when dirt.crop != null:
                        return DescribeCrop(dirt.crop);
                    case FruitTree fruitTree:
                        return DescribeFruitTree(fruitTree);
                    case Tree tree:
                        return DescribeTree(tree);
                }
            }
            else
            {
                for (int below = 1; below <= CanopyReach; below++)
                {
                    var baseTile = new Vector2(tile.X, tile.Y + below);
                    if (!location.terrainFeatures.TryGetValue(baseTile, out var trunk))
                        continue;

                    if (trunk is FruitTree fruitTree && fruitTree.growthStage.Value >= FruitTree.treeStage)
                        return DescribeFruitTree(fruitTree);
                    if (trunk is Tree tree && tree.growthStage.Value >= Tree.treeStage && !tree.stump.Value)
                        return DescribeTree(tree);
                }
            }

            // bushes live in largeTerrainFeatures and can span several tiles
            foreach (var large in location.largeTerrainFeatures)
            {
                if (large is Bush bush && bush.getBoundingBox().Contains((int)(tile.X * 64f) + 32, (int)(tile.Y * 64f) + 32))
                    return DescribeBush(bush);
            }

            return null;
        }

        private string DescribeCrop(Crop crop)
        {
            string name = SpeciesNames.ItemName(crop.indexOfHarvest.Value);
            var sb = new StringBuilder(name);

            if (crop.dead.Value)
                sb.Append('\n').Append(_i18n.Get("species.state.dead"));
            else if (crop.currentPhase.Value >= crop.phaseDays.Count - 1)
                sb.Append('\n').Append(_i18n.Get("species.state.ready"));
            else
                sb.Append('\n').Append(_i18n.Get("species.state.growing",
                    new { current = crop.currentPhase.Value + 1, total = crop.phaseDays.Count }));

            return sb.ToString();
        }

        private string DescribeFruitTree(FruitTree tree)
        {
            var sb = new StringBuilder(SpeciesNames.ForFruitTree(_i18n, tree));

            if (tree.stump.Value)
                sb.Append('\n').Append(_i18n.Get("species.state.stump"));
            else if (tree.growthStage.Value < FruitTree.treeStage)
                sb.Append('\n').Append(_i18n.Get("species.state.growing",
                    new { current = tree.growthStage.Value + 1, total = FruitTree.treeStage + 1 }));
            else if (tree.fruit.Count > 0)
                sb.Append('\n').Append(_i18n.Get("species.state.fruit", new { count = tree.fruit.Count }));

            return sb.ToString();
        }

        private string DescribeTree(Tree tree)
        {
            var sb = new StringBuilder(SpeciesNames.ForWildTree(_i18n, tree));

            if (tree.stump.Value)
                sb.Append('\n').Append(_i18n.Get("species.state.stump"));
            else if (tree.growthStage.Value < Tree.treeStage)
                sb.Append('\n').Append(_i18n.Get("species.state.growing",
                    new { current = tree.growthStage.Value + 1, total = Tree.treeStage + 1 }));

            if (tree.tapped.Value)
                sb.Append('\n').Append(_i18n.Get("species.state.tapped"));
            if (tree.hasMoss.Value)
                sb.Append('\n').Append(_i18n.Get("species.state.moss"));

            return sb.ToString();
        }

        private string DescribeBush(Bush bush)
        {
            var sb = new StringBuilder(SpeciesNames.ForBush(_i18n, bush));

            string shakeOff = bush.GetShakeOffItem();
            if (!bush.townBush.Value && !string.IsNullOrEmpty(shakeOff) && bush.tileSheetOffset.Value == 1)
                sb.Append('\n').Append(_i18n.Get("species.state.ready"));

            return sb.ToString();
        }
    }
}
