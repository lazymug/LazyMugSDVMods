using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LMQoL.Features.AutoGate
{
    public class AutoGateFeature : IFeature
    {
        public string Id => "AutoGate";

        private readonly Dictionary<GateInfo, int> _openedGates = new();
        private const int GateOpenPosition = 88;
        private const int GateClosedPosition = 0;

        public void Register(IModHelper helper, IMonitor monitor)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.AutoGateEnabled || !Context.IsWorldReady)
                return;

            var player = Game1.player;
            var location = player.currentLocation;
            if (location == null)
                return;

            var playerTile = player.Tile;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    var tile = new Vector2(playerTile.X + dx, playerTile.Y + dy);
                    var fence = GetGateAt(location, tile);
                    if (fence == null)
                        continue;

                    var info = new GateInfo(location.Name, tile);

                    if (fence.gatePosition.Value <= GateClosedPosition)
                    {
                        fence.gatePosition.Value = GateOpenPosition;
                        if (!_openedGates.ContainsKey(info))
                            _openedGates[info] = 0;
                    }

                    if (_openedGates.ContainsKey(info))
                        _openedGates[info] = 0;
                }
            }

            var toRemove = new List<GateInfo>();
            foreach (var kvp in _openedGates)
            {
                _openedGates[kvp.Key] = kvp.Value + 1;

                if (kvp.Value < config.AutoGateCloseDelayTicks)
                    continue;

                var gateLoc = Game1.getLocationFromName(kvp.Key.LocationName);
                if (gateLoc != null)
                {
                    var fence = GetGateAt(gateLoc, kvp.Key.Tile);
                    if (fence != null && fence.gatePosition.Value >= GateOpenPosition)
                        fence.gatePosition.Value = GateClosedPosition;
                }

                toRemove.Add(kvp.Key);
            }

            foreach (var key in toRemove)
                _openedGates.Remove(key);
        }

        private static Fence? GetGateAt(GameLocation location, Vector2 tile)
        {
            if (location.Objects.TryGetValue(tile, out var obj) && obj is Fence fence && fence.isGate.Value)
                return fence;
            return null;
        }
    }

    internal readonly record struct GateInfo(string LocationName, Vector2 Tile);
}
