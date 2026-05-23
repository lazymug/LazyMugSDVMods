using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using WeatheredLetters.Models;

namespace WeatheredLetters.Data
{
    public class LetterSpawner
    {
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private readonly CollectionTracker _tracker;
        private readonly ModConfig _config;

        public const string LetterItemId = "110"; // Secret Note item ID — looks like a paper note
        public const string ModDataKey = "WeatheredLetters";
        public const string ModDataValue = "true";

        private string? _spawnedMineLetterForId;

        public LetterSpawner(IModHelper helper, IMonitor monitor, CollectionTracker tracker, ModConfig config)
        {
            _helper = helper;
            _monitor = monitor;
            _tracker = tracker;
            _config = config;
        }

        public void RecoverLostLetter()
        {
            // Reset mine spawn flag daily so mine letters can respawn on the next visit
            _spawnedMineLetterForId = null;

            if (_tracker.PendingLetterId == null)
                return;

            var letter = LetterRegistry.AllLetters.Find(l => l.Id == _tracker.PendingLetterId);
            if (letter == null)
                return;

            // Only Farm and Beach have physical spawns that can be lost
            if (letter.LocationType != SpawnLocationType.Farm && letter.LocationType != SpawnLocationType.Beach)
                return;

            var location = letter.LocationType == SpawnLocationType.Farm
                ? Game1.getFarm()
                : Game1.getLocationFromName("Beach");

            if (location == null)
                return;

            // Check if the letter item still exists in the location
            foreach (var obj in location.Objects.Values)
            {
                if (obj.ItemId == LetterItemId && obj.modData.ContainsKey(ModDataKey))
                    return; // Still there
            }

            // Letter is gone — clear pending so TrySpawnLetter can respawn it
            _monitor.Log($"Pending letter '{letter.Id}' was lost from {letter.LocationType}. Resetting.", LogLevel.Warn);
            _tracker.PendingLetterId = null;
        }

        public void TrySpawnLetter()
        {
            if (_tracker.AllFound)
                return;

            // Already has a pending letter in the world
            if (_tracker.PendingLetterId != null)
                return;

            // Check cooldown
            int totalDays = Game1.Date.TotalDays;
            if (_tracker.FoundCount > 0 && totalDays - _tracker.LastLetterFoundDay < _config.DaysBetweenLetters)
                return;

            string currentSeason = Game1.currentSeason;
            var letter = _tracker.GetNextLetter(currentSeason);
            if (letter == null)
                return;

            bool spawned = letter.LocationType switch
            {
                SpawnLocationType.Farm => SpawnForageOnFarm(Game1.getFarm(), letter),
                SpawnLocationType.Beach => SpawnForageItem(Game1.getLocationFromName("Beach"), letter),
                SpawnLocationType.Mines => false, // Handled via monster drop / warp event
                SpawnLocationType.Trash => false,  // Handled via trash can check
                SpawnLocationType.Fishing => false, // Handled via fishing treasure
                _ => false
            };

            // For non-physical spawns, just mark as pending and show hint
            if (!spawned && (letter.LocationType == SpawnLocationType.Mines
                          || letter.LocationType == SpawnLocationType.Trash
                          || letter.LocationType == SpawnLocationType.Fishing))
            {
                _tracker.PendingLetterId = letter.Id;

                if (_config.ShowSpawnHints)
                {
                    string hintKey = letter.LocationType switch
                    {
                        SpawnLocationType.Trash => "hint.trash",
                        SpawnLocationType.Fishing => "hint.fishing",
                        _ => null!
                    };
                    if (hintKey != null)
                        ShowHint(hintKey);
                }

                _monitor.Log($"Letter '{letter.Id}' pending via {letter.LocationType}.", LogLevel.Trace);
            }

            if (spawned)
            {
                _tracker.PendingLetterId = letter.Id;
                _monitor.Log($"Spawned letter '{letter.Id}' at {letter.LocationType}.", LogLevel.Trace);
            }
        }

        private bool SpawnForageOnFarm(GameLocation? location, LetterData letter)
        {
            if (location == null) return false;

            var tile = FindOpenTile(location);
            if (tile == null) return false;

            var noteObj = new StardewValley.Object(LetterItemId, 1)
            {
                TileLocation = tile.Value,
                IsSpawnedObject = true
            };
            noteObj.modData[ModDataKey] = ModDataValue;
            location.Objects[tile.Value] = noteObj;

            if (_config.ShowSpawnHints)
                ShowHint("hint.farm");

            return true;
        }

        private bool SpawnForageItem(GameLocation? location, LetterData letter)
        {
            if (location == null) return false;

            var tile = FindOpenTile(location);
            if (tile == null) return false;

            // Place a forage-style object (Secret Note visual)
            var noteObj = new StardewValley.Object(LetterItemId, 1)
            {
                TileLocation = tile.Value,
                IsSpawnedObject = true
            };
            noteObj.modData[ModDataKey] = ModDataValue;
            location.Objects[tile.Value] = noteObj;

            if (_config.ShowSpawnHints)
                ShowHint("hint.beach");

            return true;
        }

        public void SpawnInMines(GameLocation mineLocation)
        {
            if (_tracker.PendingLetterId == null) return;

            // Only spawn once per pending letter
            if (_spawnedMineLetterForId == _tracker.PendingLetterId) return;

            var letter = LetterRegistry.AllLetters.Find(l => l.Id == _tracker.PendingLetterId);
            if (letter == null || letter.LocationType != SpawnLocationType.Mines) return;

            var tile = FindOpenTile(mineLocation);
            if (tile == null) return;

            var noteObj = new StardewValley.Object(LetterItemId, 1)
            {
                TileLocation = tile.Value,
                IsSpawnedObject = true
            };
            noteObj.modData[ModDataKey] = ModDataValue;
            mineLocation.Objects[tile.Value] = noteObj;
            _spawnedMineLetterForId = _tracker.PendingLetterId;

            if (_config.ShowSpawnHints)
                ShowHint("hint.mines");

            _monitor.Log($"Spawned mine letter '{letter.Id}' at {tile.Value}.", LogLevel.Trace);
        }

        private Vector2? FindOpenTile(GameLocation location)
        {
            var random = Game1.random;
            int attempts = 50;

            while (attempts-- > 0)
            {
                int x = random.Next(1, location.Map.Layers[0].LayerWidth - 1);
                int y = random.Next(1, location.Map.Layers[0].LayerHeight - 1);
                var tile = new Vector2(x, y);

                if (!location.Objects.ContainsKey(tile)
                    && !location.terrainFeatures.ContainsKey(tile)
                    && location.isTilePassable(new xTile.Dimensions.Location(x, y), Game1.viewport))
                {
                    return tile;
                }
            }

            return null;
        }

        private void ShowHint(string hintKey)
        {
            string message = _helper.Translation.Get(hintKey).ToString();
            if (!string.IsNullOrEmpty(message))
                Game1.addHUDMessage(new HUDMessage(message, HUDMessage.newQuest_type));
        }
    }
}
