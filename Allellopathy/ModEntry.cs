using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Allellopathy.Models;
using Allellopathy.Utils;
using System.Collections.Generic;

namespace Allellopathy
{
    /// <summary>
    /// The main entry point for the Allellopathy mod.
    /// </summary>
    public class ModEntry : Mod
    {
        /// <summary>
        /// The mod configuration.
        /// </summary>
        private ModConfig Config;
        
        /// <summary>
        /// Manager for allelopathic interactions.
        /// </summary>
        private AllelopathicInteractionManager _interactionManager;
        
        /// <summary>
        /// Dictionary to track which crops have been affected today.
        /// </summary>
        private Dictionary<string, bool> _affectedCropsToday = new();

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load configuration
            this.Config = this.Helper.ReadConfig<ModConfig>();
            
            // Initialize interaction manager
            _interactionManager = new AllelopathicInteractionManager();

            // Register event handlers
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Display.RenderedHud += OnRenderedHud;

            // Log startup message
            this.Monitor.Log("Allellopathy mod has been loaded!", LogLevel.Info);
        }

        /// <summary>
        /// Raised after the game is launched. Registers GMCM integration.
        /// </summary>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("allellopathy.config.enablemod").ToString(),
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("allellopathy.config.effectstrength").ToString(),
                getValue: () => Config.EffectStrength,
                setValue: value => Config.EffectStrength = value,
                min: 0f,
                max: 1f,
                interval: 0.05f
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("allellopathy.config.showeffectmessages").ToString(),
                getValue: () => Config.ShowEffectMessages,
                setValue: value => Config.ShowEffectMessages = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("allellopathy.config.showparticleeffects").ToString(),
                getValue: () => Config.ShowParticleEffects,
                setValue: value => Config.ShowParticleEffects = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("allellopathy.config.showhovertext").ToString(),
                getValue: () => Config.ShowHoverText,
                setValue: value => Config.ShowHoverText = value
            );
        }

        /// <summary>
        /// Raised after the player loads a save.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Skip if mod is disabled
            if (!Config.EnableMod)
                return;
                
            // Initialize mod when save is loaded
            this.Monitor.Log("Allellopathy initialized for this save.", LogLevel.Info);
            
            // Reset affected crops tracking
            _affectedCropsToday.Clear();
        }

        /// <summary>
        /// Raised after the day starts.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Skip if mod is disabled
            if (!Config.EnableMod)
                return;
                
            // Reset affected crops tracking
            _affectedCropsToday.Clear();
            
            // Process allelopathic effects on crops
            ProcessAllelopathicEffects();
        }
        
        /// <summary>
        /// Raised when the in-game time changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            // Skip if mod is disabled
            if (!Config.EnableMod)
                return;
                
            // Only process effects every 2 hours
            if (e.NewTime % 200 == 0)
            {
                ProcessAllelopathicEffects();
            }
        }
        
        /// <summary>
        /// Raised after the HUD is rendered to the screen.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Skip if mod is disabled or not in a location
            if (!Config.EnableMod || Game1.currentLocation == null)
                return;

            // Check if player is holding seeds for visual indicators
            if (Game1.player?.CurrentItem is StardewValley.Object obj &&
                (obj.Category == StardewValley.Object.SeedsCategory ||
                 obj.getCategoryName() == "Seeds"))
            {
                int seedCropId = GetCropIdFromSeed(obj.ParentSheetIndex);
                if (seedCropId > 0)
                {
                    ShowAllelopathicIndicators(e.SpriteBatch, seedCropId);
                }
            }

            // Always show hover text for crop under cursor
            ShowHoverTextForCropUnderCursor(e.SpriteBatch);
        }

        /// <summary>
        /// Process allelopathic effects between plants.
        /// </summary>
        private void ProcessAllelopathicEffects()
        {
            if (Game1.player?.currentLocation == null)
                return;

            // Process all locations that have crops
            var processedLocations = new HashSet<string>();
            foreach (GameLocation location in GetLocationsWithCrops())
            {
                // Avoid processing the same location twice
                if (!processedLocations.Add(location.Name))
                    continue;

                if (location.terrainFeatures == null)
                    continue;

                int cropCount = 0;
                int interactionCount = 0;

                foreach (var terrainFeaturePair in location.terrainFeatures.Pairs)
                {
                    if (terrainFeaturePair.Value is HoeDirt dirt && dirt.crop != null)
                    {
                        cropCount++;
                        if (ApplyAllelopathicEffects(location, terrainFeaturePair.Key, dirt))
                            interactionCount++;
                    }
                }

                if (cropCount > 0)
                    this.Monitor.Log($"[{location.Name}] Processed {cropCount} crops, found {interactionCount} with allelopathic interactions.", LogLevel.Trace);
            }
        }

        private IEnumerable<GameLocation> GetLocationsWithCrops()
        {
            yield return Game1.getFarm();

            var greenhouse = Game1.getLocationFromName("Greenhouse");
            if (greenhouse != null)
                yield return greenhouse;

            var islandWest = Game1.getLocationFromName("IslandWest");
            if (islandWest != null)
                yield return islandWest;

            // Also process the player's current location if it's not one of the above
            var current = Game1.player?.currentLocation;
            if (current != null && current != Game1.getFarm() && current != greenhouse && current != islandWest)
                yield return current;
        }

        /// <summary>
        /// Apply allelopathic effects to a crop based on its neighbors.
        /// </summary>
        /// <param name="location">The game location.</param>
        /// <param name="tilePosition">The position of the crop.</param>
        /// <param name="hoeDirt">The HoeDirt containing the crop.</param>
        /// <returns>True if at least one allelopathic interaction was found.</returns>
        private bool ApplyAllelopathicEffects(GameLocation location, Vector2 tilePosition, HoeDirt hoeDirt)
        {
            // Skip if the crop has already been affected today
            string cropKey = $"{location.Name}_{tilePosition.X}_{tilePosition.Y}";
            if (_affectedCropsToday.ContainsKey(cropKey))
                return false;

            // Get the crop ID
            int targetCropId = CropEffects.GetCropIdFromHoeDirt(hoeDirt);
            if (targetCropId == -1)
                return false;

            // Use the max interaction radius to find nearby crops
            var nearbyCrops = CropEffects.GetCropsInRadius(location, tilePosition, 3);
            bool hasPositive = false;
            bool hasNegative = false;

            foreach (var nearbyPair in nearbyCrops)
            {
                int sourceCropId = CropEffects.GetCropIdFromHoeDirt(nearbyPair.Value);
                if (sourceCropId == -1)
                    continue;

                var interaction = _interactionManager.GetInteraction(sourceCropId, targetCropId);
                if (interaction == null)
                    continue;

                // Check if the neighbor is within this interaction's effective radius
                float distance = Vector2.Distance(tilePosition, nearbyPair.Key);
                if (distance > interaction.EffectRadius)
                    continue;

                bool isPositive = interaction.EffectType == AllelopathicEffectType.Positive;
                if (isPositive) hasPositive = true;
                else hasNegative = true;

                // Try to apply the gameplay effect (random chance)
                bool effectApplied = CropEffects.ApplyEffect(location, tilePosition, hoeDirt, interaction, Config.EffectStrength);

                this.Monitor.Log($"[{location.Name}] {interaction.EffectType} interaction: source crop {sourceCropId} at {nearbyPair.Key} -> target crop {targetCropId} at {tilePosition} (distance: {distance:F1}, radius: {interaction.EffectRadius}, effect applied: {effectApplied})", LogLevel.Trace);

                // Occasional particle effect
                if (Game1.random.NextDouble() < 0.05)
                {
                    if (isPositive)
                        ShowPositiveParticles(location, tilePosition);
                    else
                        ShowNegativeParticles(location, tilePosition);
                }
            }

            // Persist effect flags to modData for tooltip display
            hoeDirt.modData["Allellopathy.HasPositiveEffect"] = hasPositive.ToString();
            hoeDirt.modData["Allellopathy.HasNegativeEffect"] = hasNegative.ToString();

            // Show a message to the player on the first occurrence today
            if ((hasPositive || hasNegative) &&
                Config.ShowEffectMessages &&
                location == Game1.currentLocation &&
                Vector2.Distance(Game1.player.Tile, tilePosition) <= 5)
            {
                string message = hasPositive ?
                    "allellopathy.message.positive" : "allellopathy.message.negative";

                Game1.showGlobalMessage(Helper.Translation.Get(message).ToString());
            }

            _affectedCropsToday[cropKey] = true;
            return hasPositive || hasNegative;
        }
        
        /// <summary>
        /// Gets the crop ID from a seed item.
        /// </summary>
        /// <param name="seedId">The seed item ID.</param>
        /// <returns>The crop ID, or -1 if not found.</returns>
        private int GetCropIdFromSeed(int seedId)
        {
            // This is a simplified implementation - in a real mod, you'd need to map seed IDs to crop IDs
            // For vanilla seeds, you can often derive the crop ID from the seed ID or use a dictionary
            
            // For testing purposes, return a valid crop ID
            return seedId + 1; // Simple mapping for testing
        }
        
        /// <summary>
        /// Shows allelopathic indicators for nearby tiles when holding seeds.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="seedCropId">The crop ID of the seed being held.</param>
        private void ShowAllelopathicIndicators(SpriteBatch spriteBatch, int seedCropId)
        {
            if (!Config.ShowVisualIndicators)
                return;
                
            // Get player's current tile position
            Vector2 playerTile = Game1.player.Tile;
            
            // Check surrounding tiles in a radius (e.g., 3 tiles)
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    Vector2 checkTile = new Vector2(playerTile.X + x, playerTile.Y + y);

                    // Check if there's a crop at this tile
                    if (Game1.currentLocation.terrainFeatures.TryGetValue(checkTile, out TerrainFeature feature) &&
                        feature is HoeDirt dirt && dirt.crop != null)
                    {
                        int existingCropId = CropEffects.GetCropIdFromHoeDirt(dirt);
                        if (existingCropId == -1)
                            continue;

                        // Check interaction between the seed crop and the existing crop
                        var interaction = _interactionManager.GetInteraction(seedCropId, existingCropId);
                        if (interaction != null)
                        {
                            // Draw indicator based on effect type
                            DrawEffectIndicator(spriteBatch, checkTile, interaction);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Draws an effect indicator for an allelopathic interaction.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="tile">The tile position.</param>
        /// <param name="interaction">The allelopathic interaction.</param>
        private void DrawEffectIndicator(SpriteBatch spriteBatch, Vector2 tile, AllelopathicInteraction interaction)
        {
            // Convert tile position to screen position
            Vector2 screenPos = Game1.GlobalToLocal(new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize));
            
            // Choose color based on effect type
            Color indicatorColor = interaction.EffectType switch
            {
                AllelopathicEffectType.Positive => Color.LightGreen,
                AllelopathicEffectType.Negative => Color.Red,
                _ => Color.White
            };
            
            // Apply opacity from config
            indicatorColor *= Config.IndicatorOpacity;
            
            // Choose icon based on crop effect type
            Rectangle iconRect;
            switch (interaction.CropEffect)
            {
                case CropEffectType.Quality:
                    iconRect = new Rectangle(338, 400, 8, 8); // Star icon
                    break;
                case CropEffectType.Quantity:
                    iconRect = new Rectangle(346, 392, 8, 8); // Multiple items icon
                    break;
                case CropEffectType.GrowthSpeed:
                    iconRect = new Rectangle(395, 497, 4, 10); // Clock icon
                    break;
                default:
                    iconRect = new Rectangle(403, 496, 5, 10); // Default icon
                    break;
            }
            
            // Draw indicator
            spriteBatch.Draw(
                Game1.mouseCursors, 
                screenPos + new Vector2(Game1.tileSize/2, -Game1.tileSize/2), 
                iconRect, 
                indicatorColor, 
                0f, 
                new Vector2(4, 5), 
                3.0f, 
                SpriteEffects.None, 
                1f
            );
        }
        
        /// <summary>
        /// Shows hover text for the crop under the cursor.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        private void ShowHoverTextForCropUnderCursor(SpriteBatch spriteBatch)
        {
            if (!Config.ShowHoverText)
                return;

            // Get tile under cursor (use integer tile coordinates)
            Vector2 cursorTile = new Vector2(
                (int)((Game1.getMouseX() + Game1.viewport.X) / (float)Game1.tileSize),
                (int)((Game1.getMouseY() + Game1.viewport.Y) / (float)Game1.tileSize)
            );

            // Check if there's a crop at this location
            if (Game1.currentLocation.terrainFeatures.TryGetValue(cursorTile, out TerrainFeature feature) &&
                feature is HoeDirt dirt && dirt.crop != null)
            {
                // First check modData (set during effect processing)
                bool hasPositive = dirt.modData.TryGetValue("Allellopathy.HasPositiveEffect", out string positiveStr) &&
                                   bool.TryParse(positiveStr, out bool posVal) && posVal;
                bool hasNegative = dirt.modData.TryGetValue("Allellopathy.HasNegativeEffect", out string negativeStr) &&
                                   bool.TryParse(negativeStr, out bool negVal) && negVal;

                // If no modData yet, check neighbors directly for real-time feedback
                if (!hasPositive && !hasNegative)
                {
                    int targetCropId = CropEffects.GetCropIdFromHoeDirt(dirt);
                    if (targetCropId != -1)
                    {
                        var nearbyCrops = CropEffects.GetCropsInRadius(Game1.currentLocation, cursorTile, 2);
                        foreach (var nearbyPair in nearbyCrops)
                        {
                            int sourceCropId = CropEffects.GetCropIdFromHoeDirt(nearbyPair.Value);
                            if (sourceCropId == -1) continue;

                            var interaction = _interactionManager.GetInteraction(sourceCropId, targetCropId);
                            if (interaction != null)
                            {
                                if (interaction.EffectType == AllelopathicEffectType.Positive)
                                    hasPositive = true;
                                else if (interaction.EffectType == AllelopathicEffectType.Negative)
                                    hasNegative = true;
                            }
                        }
                    }
                }

                if (hasPositive)
                    DrawHoverText(spriteBatch, cursorTile, true);
                else if (hasNegative)
                    DrawHoverText(spriteBatch, cursorTile, false);
            }
        }
        
        /// <summary>
        /// Draws hover text for a crop with allelopathic effects.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="tilePosition">The tile position.</param>
        /// <param name="isPositive">Whether the effect is positive.</param>
        private void DrawHoverText(SpriteBatch spriteBatch, Vector2 tilePosition, bool isPositive)
        {
            string effectTypeText = isPositive ? "Efeito Positivo" : "Efeito Negativo";
            Color textColor = isPositive ? Color.LightGreen : Color.Red;
            
            string hoverText = $"{effectTypeText}\nEsta planta está sendo afetada pela alelopatia.";
            
            // Draw hover text at mouse position
            IClickableMenu.drawHoverText(
                spriteBatch, 
                hoverText, 
                Game1.smallFont
            );
        }
        
        /// <summary>
        /// Shows positive particle effects for a crop with allelopathic effects.
        /// </summary>
        /// <param name="location">The game location.</param>
        /// <param name="tilePosition">The tile position.</param>
        private void ShowPositiveParticles(GameLocation location, Vector2 tilePosition)
        {
            if (!Config.ShowParticleEffects)
                return;
                
            Vector2 position = new Vector2(tilePosition.X * Game1.tileSize + Game1.tileSize / 2, 
                                          tilePosition.Y * Game1.tileSize + Game1.tileSize / 2);
            
            // Green sparkles
            for (int i = 0; i < 5; i++)
            {
                location.temporarySprites.Add(
                    new TemporaryAnimatedSprite(
                        "TileSheets\\animations", 
                        new Rectangle(0, 256, 64, 64), 
                        50f, 
                        8, 
                        1, 
                        position + new Vector2(Game1.random.Next(-32, 32), Game1.random.Next(-32, 32)), 
                        false, 
                        Game1.random.NextDouble() < 0.5, 
                        0.001f, 
                        0.01f, 
                        Color.LightGreen * 0.8f, 
                        0.5f, 
                        0f, 
                        0f, 
                        0f
                    )
                );
            }
        }
        
        /// <summary>
        /// Shows negative particle effects for a crop with allelopathic effects.
        /// </summary>
        /// <param name="location">The game location.</param>
        /// <param name="tilePosition">The tile position.</param>
        private void ShowNegativeParticles(GameLocation location, Vector2 tilePosition)
        {
            if (!Config.ShowParticleEffects)
                return;
                
            Vector2 position = new Vector2(tilePosition.X * Game1.tileSize + Game1.tileSize / 2, 
                                          tilePosition.Y * Game1.tileSize + Game1.tileSize / 2);
            
            // Red/brown particles
            for (int i = 0; i < 5; i++)
            {
                location.temporarySprites.Add(
                    new TemporaryAnimatedSprite(
                        "TileSheets\\animations", 
                        new Rectangle(0, 320, 64, 64), 
                        50f, 
                        8, 
                        1, 
                        position + new Vector2(Game1.random.Next(-32, 32), Game1.random.Next(-32, 32)), 
                        false, 
                        Game1.random.NextDouble() < 0.5, 
                        0.001f, 
                        0.01f, 
                        Color.Brown * 0.8f, 
                        0.5f, 
                        0f, 
                        0f, 
                        0f
                    )
                );
            }
        }
    }

    /// <summary>
    /// The mod configuration class.
    /// </summary>
    public class ModConfig
    {
        /// <summary>
        /// Whether the mod is enabled.
        /// </summary>
        public bool EnableMod { get; set; } = true;

        /// <summary>
        /// The strength of allelopathic effects (0.0 to 1.0).
        /// </summary>
        public float EffectStrength { get; set; } = 0.5f;
        
        /// <summary>
        /// Whether to show messages when allelopathic effects occur.
        /// </summary>
        public bool ShowEffectMessages { get; set; } = true;
        
        /// <summary>
        /// Whether to show visual indicators when holding seeds.
        /// </summary>
        public bool ShowVisualIndicators { get; set; } = true;
        
        /// <summary>
        /// Whether to show particle effects for allelopathic interactions.
        /// </summary>
        public bool ShowParticleEffects { get; set; } = true;
        
        /// <summary>
        /// Whether to show hover text with detailed information.
        /// </summary>
        public bool ShowHoverText { get; set; } = true;
        
        /// <summary>
        /// The opacity of visual indicators (0.0 to 1.0).
        /// </summary>
        public float IndicatorOpacity { get; set; } = 0.8f;
    }
}
