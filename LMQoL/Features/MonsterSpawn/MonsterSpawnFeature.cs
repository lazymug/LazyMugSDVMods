using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace LMQoL.Features.MonsterSpawn
{
    /// <summary>Spawns a chosen monster in the mines — handy when a quest wants kills of something
    /// that doesn't live on the floor you can reach, or for testing drops.
    ///
    /// Monsters are resolved by name: first against the game's own monster classes (so they keep
    /// their real behaviour), falling back to the generic Monster type driven by Data/Monsters for
    /// anything else, including monsters added by mods.</summary>
    public class MonsterSpawnFeature : IFeature
    {
        private IModHelper _helper = null!;
        private IMonitor _log = null!;

        public string Id => "MonsterSpawn";

        public void Register(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            _log = monitor;

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Player.Warped += OnWarped;

            helper.ConsoleCommands.Add("lmqol_spawn",
                "Spawn a monster at your feet. Usage: lmqol_spawn <name> [count]\nExample: lmqol_spawn \"Green Slime\" 5",
                OnSpawnCommand);
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.MonsterSpawnEnabled || !Context.IsPlayerFree)
                return;

            if (!config.MonsterSpawnKey.JustPressed())
                return;

            _helper.Input.SuppressActiveKeybinds(config.MonsterSpawnKey);
            Spawn(config.MonsterSpawnName, config.MonsterSpawnCount, announce: true);
        }

        /// <summary>Optionally seed the configured monster when arriving on a specific mine floor.</summary>
        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            var config = ModEntry.Config;
            if (!config.MonsterSpawnEnabled || !config.MonsterSpawnOnFloorEntry || !e.IsLocalPlayer)
                return;

            if (e.NewLocation is not MineShaft mine || mine.mineLevel != config.MonsterSpawnFloor)
                return;

            Spawn(config.MonsterSpawnName, config.MonsterSpawnCount, announce: false);
        }

        private void OnSpawnCommand(string command, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                _log.Log("Load a save first.", LogLevel.Warn);
                return;
            }

            if (args.Length == 0)
            {
                _log.Log("Usage: lmqol_spawn <name> [count]", LogLevel.Info);
                return;
            }

            int count = args.Length > 1 && int.TryParse(args[^1], out int parsed) ? parsed : 1;
            string name = args.Length > 1 && int.TryParse(args[^1], out _)
                ? string.Join(" ", args[..^1])
                : string.Join(" ", args);

            int spawned = Spawn(name, count, announce: false);
            _log.Log(spawned > 0 ? $"Spawned {spawned}x {name}." : $"Could not spawn '{name}'.", spawned > 0 ? LogLevel.Info : LogLevel.Warn);
        }

        /// <summary>Place monsters around the player. Returns how many actually appeared.</summary>
        private int Spawn(string name, int count, bool announce)
        {
            var location = Game1.currentLocation;
            if (location == null || string.IsNullOrWhiteSpace(name))
                return 0;

            int spawned = 0;
            var origin = Game1.player.Tile;

            for (int i = 0; i < Math.Max(1, count); i++)
            {
                // ring out from the player so they don't all land on one tile
                var tile = FindFreeTile(location, origin, i);
                var monster = Create(name, tile * Game1.tileSize);
                if (monster == null)
                {
                    _log.LogOnce($"No monster matches '{name}'.", LogLevel.Warn);
                    break;
                }

                location.characters.Add(monster);
                spawned++;
            }

            if (announce && spawned > 0)
                Game1.addHUDMessage(new HUDMessage(_helper.Translation.Get("monster.spawned", new { count = spawned, name }), HUDMessage.newQuest_type));
            else if (announce)
                Game1.playSound("cancel");

            return spawned;
        }

        private static Vector2 FindFreeTile(GameLocation location, Vector2 origin, int attempt)
        {
            for (int radius = 1; radius <= 4; radius++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        var candidate = new Vector2(origin.X + dx, origin.Y + dy);
                        if (candidate == origin || !location.isTileOnMap(candidate))
                            continue;
                        if (location.IsTileOccupiedBy(candidate) || !location.isTilePassable(new xTile.Dimensions.Location((int)candidate.X, (int)candidate.Y), Game1.viewport))
                            continue;
                        if (attempt-- <= 0)
                            return candidate;
                    }
                }
            }
            return origin;
        }

        /// <summary>Build a monster by name, preferring the real class so its behaviour is intact.</summary>
        private static Monster? Create(string name, Vector2 pixelPosition)
        {
            var type = ResolveType(name);
            if (type != null)
            {
                try
                {
                    if (Activator.CreateInstance(type, pixelPosition) is Monster typed)
                        return typed;
                }
                catch
                {
                    // no (Vector2) constructor, or it threw — fall through to the data-driven one
                }
            }

            try
            {
                return new Monster(name, pixelPosition);   // driven by Data/Monsters
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Match "Green Slime" / "greenslime" / "GreenSlime" to the class of that name.</summary>
        private static Type? ResolveType(string name)
        {
            string wanted = new string(name.Where(char.IsLetterOrDigit).ToArray());
            return typeof(Monster).Assembly
                .GetTypes()
                .FirstOrDefault(t =>
                    !t.IsAbstract
                    && typeof(Monster).IsAssignableFrom(t)
                    && string.Equals(t.Name, wanted, StringComparison.OrdinalIgnoreCase));
        }
    }
}
