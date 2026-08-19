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

            CollectMapForages(location, player, radius);
            PullDebris(location, player, radiusSq, config.MagnetForagingSpeed);
        }

        private static void CollectMapForages(GameLocation location, Farmer player, float radiusTiles)
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

        private static void PullDebris(GameLocation location, Farmer player, float radiusSq, int speed)
        {
            var standing = player.StandingPixel;

            foreach (var debris in location.debris)
            {
                if (!IsCollectibleDebris(debris))
                    continue;

                int count = debris.Chunks.Count;
                if (count == 0)
                    continue;

                // The game refuses to collect debris the inventory can't take (Debris.updateChunks).
                // Dragging it in anyway leaves the chunk glued to the player, animating forever
                // without ever being picked up — so leave those on the ground, as vanilla does.
                if (!PlayerCanTake(player, debris))
                    continue;

                // Range is decided per DEBRIS, from the average of its chunks
                // (Debris.approximatePosition) — not per chunk. Pull the whole group in together:
                // if some chunks lag behind, the average stays out of the player's magnetic radius
                // and the game then collects none of them, leaving the rest orbiting the player.
                float avgX = 0f, avgY = 0f;
                foreach (var c in debris.Chunks)
                {
                    var p = c.position.Value;
                    avgX += p.X; avgY += p.Y;
                }
                avgX = avgX / count + 32f;
                avgY = avgY / count + 32f;

                float adx = standing.X - avgX, ady = standing.Y - avgY;
                if (adx * adx + ady * ady > radiusSq)
                    continue;

                foreach (var chunk in debris.Chunks)
                {
                    // the game treats position + 32 as the chunk's centre
                    var chunkPos = chunk.position.Value;
                    float dx = standing.X - (chunkPos.X + 32f);
                    float dy = standing.Y - (chunkPos.Y + 32f);
                    float distSq = dx * dx + dy * dy;

                    if (distSq < 1f)
                        continue;

                    float dist = (float)System.Math.Sqrt(distSq);
                    chunk.position.Value = new Vector2(chunkPos.X + dx / dist * speed, chunkPos.Y + dy / dist * speed);
                }
            }
        }

        /// <summary>Mirrors the acceptance test in <c>Debris.updateChunks</c>: only pull what the
        /// game would actually collect.</summary>
        private static bool PlayerCanTake(Farmer player, Debris debris)
        {
            var item = debris.item;
            if (item != null)
                return player.couldInventoryAcceptThisItem(item);

            string itemId = debris.itemId.Value;
            if (string.IsNullOrEmpty(itemId))
                return true;    // not an item-bearing debris; nothing to gate on

            switch (debris.debrisType.Value)
            {
                case Debris.DebrisType.RESOURCE:
                    return player.couldInventoryAcceptThisItem(itemId, 1);

                case Debris.DebrisType.OBJECT:
                case Debris.DebrisType.ARCHAEOLOGY:
                    if (itemId == "(O)102" && player.hasMenuOpen.Value)
                        return false;   // lost book: the game skips it while a menu is open
                    return player.couldInventoryAcceptThisItem(itemId, 1, debris.itemQuality);

                default:
                    return true;
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
