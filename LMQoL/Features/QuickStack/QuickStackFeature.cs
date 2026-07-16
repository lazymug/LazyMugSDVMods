using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace LMQoL.Features.QuickStack
{
    /// <summary>
    /// On a configurable keypress, tops up existing item stacks in nearby chests with matching
    /// items from the player's inventory. Only fills stacks the chest already has — it never
    /// moves an item type the chest doesn't already contain, so nothing gets misplaced.
    /// </summary>
    public class QuickStackFeature : IFeature
    {
        public string Id => "QuickStack";

        private IModHelper _helper = null!;

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.QuickStackEnabled || !Context.IsPlayerFree)
                return;

            if (!config.QuickStackKey.JustPressed())
                return;

            DoQuickStack(config.QuickStackRadius);
        }

        private void DoQuickStack(int radius)
        {
            var player = Game1.player;
            var location = player.currentLocation;
            if (location == null)
                return;

            var chests = FindNearbyChests(location, player.Tile, radius);
            if (chests.Count == 0)
            {
                Game1.playSound("cancel");
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("quickstack.nochests").ToString(), HUDMessage.error_type));
                return;
            }

            int movedItems = 0;
            var inventory = player.Items;
            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                if (item == null || item.maximumStackSize() <= 1)
                    continue;

                foreach (var chest in chests)
                {
                    int before = item.Stack;
                    int remaining = DepositIntoExistingStacks(item, chest);
                    movedItems += before - remaining;

                    if (remaining <= 0)
                    {
                        inventory[i] = null;
                        break;
                    }
                }
            }

            if (movedItems > 0)
            {
                Game1.playSound("Ship");
                Game1.addHUDMessage(new HUDMessage(
                    _helper.Translation.Get("quickstack.moved", new { count = movedItems }).ToString(),
                    HUDMessage.newQuest_type));
            }
            else
            {
                Game1.playSound("cancel");
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("quickstack.nothing").ToString(), HUDMessage.error_type));
            }
        }

        private static List<Chest> FindNearbyChests(GameLocation location, Vector2 origin, int radius)
        {
            var result = new List<Chest>();
            foreach (var pair in location.Objects.Pairs)
            {
                if (pair.Value is not Chest chest || !chest.playerChest.Value)
                    continue;

                // Never deposit into a mini-shipping bin (that would sell the items).
                if (chest.specialChestType.Value == Chest.SpecialChestTypes.MiniShippingBin)
                    continue;

                var tile = pair.Key;
                if (Math.Abs(tile.X - origin.X) <= radius && Math.Abs(tile.Y - origin.Y) <= radius)
                    result.Add(chest);
            }

            return result;
        }

        private static int DepositIntoExistingStacks(Item item, Chest chest)
        {
            var items = chest.GetItemsForPlayer();
            foreach (var slot in items)
            {
                if (slot == null || !slot.canStackWith(item))
                    continue;

                int space = slot.maximumStackSize() - slot.Stack;
                if (space <= 0)
                    continue;

                int move = Math.Min(space, item.Stack);
                slot.Stack += move;
                item.Stack -= move;

                if (item.Stack <= 0)
                    break;
            }

            return item.Stack;
        }
    }
}
