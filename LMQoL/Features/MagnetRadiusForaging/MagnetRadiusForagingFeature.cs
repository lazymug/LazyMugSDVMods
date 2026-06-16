using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LMQoL.Features.MagnetRadiusForaging
{
    public class MagnetRadiusForagingFeature : IFeature
    {
        public string Id => "MagnetRadiusForaging";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.MagnetForagingEnabled || !Context.IsWorldReady)
                return;

            var player = Game1.player;
            var location = player.currentLocation;
            if (location == null)
                return;

            float radius = config.MagnetForagingRadius;
            float radiusPixels = radius * Game1.tileSize;
            float radiusSq = radiusPixels * radiusPixels;
            var playerCenter = player.getStandingPosition();

            CollectMapForages(location, player, playerCenter, radius);
            PullDebris(location, playerCenter, radiusSq, config.MagnetForagingSpeed);
        }

        private static void CollectMapForages(GameLocation location, Farmer player, Vector2 playerCenter, float radiusTiles)
        {
            var playerTile = player.Tile;
            int r = (int)radiusTiles + 1;
            var toRemove = new List<Vector2>();

            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    var tile = new Vector2(playerTile.X + dx, playerTile.Y + dy);
                    float distTiles = Vector2.Distance(playerTile, tile);
                    if (distTiles > radiusTiles)
                        continue;

                    if (!location.Objects.TryGetValue(tile, out var obj))
                        continue;

                    if (!obj.IsSpawnedObject || obj.questItem.Value)
                        continue;

                    if (!player.couldInventoryAcceptThisItem(obj))
                        continue;

                    var pixelOrigin = tile * Game1.tileSize;
                    var debris = new Debris(obj.getOne(), pixelOrigin);
                    location.debris.Add(debris);
                    toRemove.Add(tile);
                }
            }

            foreach (var tile in toRemove)
                location.Objects.Remove(tile);
        }

        private static void PullDebris(GameLocation location, Vector2 playerCenter, float radiusSq, int speed)
        {
            foreach (var debris in location.debris)
            {
                if (!IsCollectibleDebris(debris))
                    continue;

                var chunk = debris.Chunks.Count > 0 ? debris.Chunks[0] : null;
                if (chunk == null)
                    continue;

                var chunkPos = chunk.position.Value;
                float dx = playerCenter.X - chunkPos.X;
                float dy = playerCenter.Y - chunkPos.Y;
                float distSq = dx * dx + dy * dy;

                if (distSq > radiusSq || distSq < 1f)
                    continue;

                float dist = (float)System.Math.Sqrt(distSq);
                float moveX = dx / dist * speed;
                float moveY = dy / dist * speed;

                chunk.position.Value = new Vector2(chunkPos.X + moveX, chunkPos.Y + moveY);
            }
        }

        private static bool IsCollectibleDebris(Debris debris)
        {
            return debris.debrisType.Value == Debris.DebrisType.OBJECT
                || debris.debrisType.Value == Debris.DebrisType.RESOURCE
                || debris.debrisType.Value == Debris.DebrisType.ARCHAEOLOGY
                || debris.item != null;
        }
    }
}
