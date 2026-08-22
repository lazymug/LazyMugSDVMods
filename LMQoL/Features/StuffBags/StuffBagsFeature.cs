using System;
using System.Collections.Generic;
using LMQoL.Features.BuildWithBags;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace LMQoL.Features.StuffBags
{
    /// <summary>Sweeps loose items out of your inventory and nearby chests into whichever Item Bag
    /// accepts them, in one go.
    ///
    /// Unlike the game's own "organise", a bag can live somewhere other than the item: a crop in a
    /// chest can go into a bag you're carrying, and vice versa. Every bag in reach is offered each
    /// item, and Item Bags itself decides whether it belongs there.</summary>
    public class StuffBagsFeature : IFeature
    {
        private IModHelper _helper = null!;
        private IMonitor _log = null!;

        public string Id => "StuffBags";

        internal static bool Available => BuildWithBagsFeature.Api != null && ModEntry.Config.StuffBagsEnabled;

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _log = monitor;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (!Available || !Context.IsWorldReady)
                return;

            if (!ModEntry.Config.StuffBagsKey.JustPressed())
                return;

            // allowed both in the world and with a menu open, so it works from the inventory screen
            if (!Context.IsPlayerFree && Game1.activeClickableMenu is not StardewValley.Menus.GameMenu)
                return;

            _helper.Input.SuppressActiveKeybinds(ModEntry.Config.StuffBagsKey);
            Run();
        }

        /// <summary>Move everything that fits into the bags within reach.</summary>
        internal void Run()
        {
            var api = BuildWithBagsFeature.Api;
            if (api == null)
                return;

            try
            {
                var player = Game1.player;
                var chests = NearbyChests(ModEntry.Config.StuffBagsRadius);

                var bags = new List<Item>();
                AddBags(api, player.Items, bags);
                foreach (var chest in chests)          // bags sitting in those chests count too
                    AddBags(api, chest.Items, bags);

                if (bags.Count == 0)
                {
                    Notify("stuffbags.nobags");
                    return;
                }

                int moved = Sweep(api, player.Items, bags);
                foreach (var chest in chests)
                    moved += Sweep(api, chest.Items, bags);

                Notify(moved > 0 ? "stuffbags.moved" : "stuffbags.nothing", moved);
            }
            catch (Exception ex)
            {
                _log.Log($"Could not stuff bags: {ex.Message}", LogLevel.Warn);
            }
        }

        /// <summary>Collect the bags in a container, unpacking Omni Bags.
        ///
        /// The API's GetItemBags only returns what sits directly in the container, and an Omni Bag
        /// holds other bags rather than items — so without stepping inside it, a player who keeps
        /// everything in one Omni Bag has no usable bag at all and nothing gets stored.</summary>
        private static void AddBags(IItemBagsApi api, IList<Item> container, List<Item> into)
        {
            foreach (var bag in api.GetItemBags(container))
            {
                foreach (var nested in Unpack(bag))
                {
                    if (!IsExcluded(nested) && !into.Contains(nested))
                        into.Add(nested);
                }
            }
        }

        /// <summary>Bag types that shouldn't receive items during a sweep.
        ///
        /// A Rucksack takes anything, so if it's in the list it swallows everything before the
        /// bag that item actually belongs to ever gets offered it. A Bundle Bag is likewise
        /// special-purpose — it's for Community Center bundles, not general storage.</summary>
        private static bool IsExcluded(Item bag)
        {
            string type = bag.GetType().Name;

            return type switch
            {
                "Rucksack" => !ModEntry.Config.StuffBagsIncludeRucksack,
                "BundleBag" => !ModEntry.Config.StuffBagsIncludeBundleBag,
                _ => false,
            };
        }

        /// <summary>An Omni Bag yields the bags inside it; anything else yields itself.</summary>
        /// <remarks>Reached by reflection: naming the OmniBag type would mean referencing
        /// ItemBags.dll, which would stop LM QoL loading without that mod.</remarks>
        private static IEnumerable<Item> Unpack(Item bag)
        {
            var nested = bag?.GetType().GetProperty("NestedBags")?.GetValue(bag) as System.Collections.IEnumerable;
            if (nested == null)
            {
                if (bag != null)
                    yield return bag;
                yield break;
            }

            foreach (var inner in nested)
            {
                if (inner is Item item)
                {
                    // an Omni Bag can hold another Omni Bag
                    foreach (var deeper in Unpack(item))
                        yield return deeper;
                }
            }
        }

        /// <summary>Offer every loose object in one container to every bag.</summary>
        private static int Sweep(IItemBagsApi api, IList<Item> source, List<Item> bags)
        {
            int moved = 0;

            // iterate backwards: entries disappear as they're absorbed
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i] is not Object item || api.IsItemBag(item))
                    continue;   // never stuff a bag into a bag

                foreach (var bag in bags)
                {
                    if (item.Stack <= 0)
                        break;

                    if (api.TryMoveObjectToBag(item, item.Stack, source, bag, out int qty, playSoundEffect: false))
                        moved += qty;
                }
            }

            return moved;
        }

        private static List<Chest> NearbyChests(int radius)
        {
            var found = new List<Chest>();
            var location = Game1.currentLocation;
            if (location == null)
                return found;

            var origin = Game1.player.Tile;
            foreach (var (tile, obj) in location.Objects.Pairs)
            {
                if (obj is not Chest chest || chest.SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
                    continue;

                if (Math.Abs(tile.X - origin.X) <= radius && Math.Abs(tile.Y - origin.Y) <= radius)
                    found.Add(chest);
            }

            return found;
        }

        private void Notify(string key, int count = 0)
        {
            string text = _helper.Translation.Get(key, new { count });
            Game1.addHUDMessage(new HUDMessage(text, count > 0 ? HUDMessage.achievement_type : HUDMessage.error_type));
        }
    }
}
