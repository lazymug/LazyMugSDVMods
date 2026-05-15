using System;
using System.Collections.Generic;
using Allellopathy.Constants;

namespace Allellopathy.Models
{
    /// <summary>
    /// Represents the type of allelopathic effect.
    /// </summary>
    public enum AllelopathicEffectType
    {
        /// <summary>
        /// Positive effect (beneficial).
        /// </summary>
        Positive,
        
        /// <summary>
        /// Negative effect (harmful).
        /// </summary>
        Negative,
        
        /// <summary>
        /// Neutral effect (no impact).
        /// </summary>
        Neutral
    }
    
    /// <summary>
    /// Represents the specific effect on a crop.
    /// </summary>
    public enum CropEffectType
    {
        /// <summary>
        /// Default effect (general growth).
        /// </summary>
        Default,
        
        /// <summary>
        /// Improves crop quality.
        /// </summary>
        Quality,
        
        /// <summary>
        /// Increases harvest quantity (for multi-harvest crops).
        /// </summary>
        Quantity,
        
        /// <summary>
        /// Reduces days until harvest.
        /// </summary>
        GrowthSpeed
    }

    /// <summary>
    /// Represents an allelopathic interaction between two crops.
    /// </summary>
    public class AllelopathicInteraction
    {
        /// <summary>
        /// The source crop ID that causes the effect.
        /// </summary>
        public int SourceCropId { get; set; }
        
        /// <summary>
        /// The target crop ID that receives the effect.
        /// </summary>
        public int TargetCropId { get; set; }
        
        /// <summary>
        /// The type of allelopathic effect.
        /// </summary>
        public AllelopathicEffectType EffectType { get; set; }
        
        /// <summary>
        /// The specific type of effect on the crop.
        /// </summary>
        public CropEffectType CropEffect { get; set; } = CropEffectType.Default;
        
        /// <summary>
        /// The strength of the effect (0.0 to 1.0).
        /// </summary>
        public float EffectStrength { get; set; }
        
        /// <summary>
        /// The maximum distance (in tiles) at which this effect applies.
        /// </summary>
        public int EffectRadius { get; set; }
        
        /// <summary>
        /// Description of the interaction for display purposes.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Manages allelopathic interactions between crops.
    /// </summary>
    public class AllelopathicInteractionManager
    {
        /// <summary>
        /// Dictionary of allelopathic interactions, keyed by source crop ID.
        /// </summary>
        private readonly Dictionary<int, List<AllelopathicInteraction>> _interactions = new();

        /// <summary>
        /// Initializes a new instance of the AllelopathicInteractionManager class.
        /// </summary>
        public AllelopathicInteractionManager()
        {
            InitializeInteractions();
        }

        /// <summary>
        /// Initializes the default allelopathic interactions.
        /// </summary>
        private void InitializeInteractions()
        {
            // Spring crop interactions
            InitializeSpringInteractions();
            
            // Summer crop interactions
            InitializeSummerInteractions();
            
            // Fall crop interactions
            InitializeFallInteractions();
            
            // Special crop interactions
            InitializeSpecialInteractions();
        }

        /// <summary>
        /// Adds an allelopathic interaction to the manager.
        /// </summary>
        /// <param name="sourceCropId">The source crop ID.</param>
        /// <param name="targetCropId">The target crop ID. Use -1 for "all crops".</param>
        /// <param name="effectType">The type of effect.</param>
        /// <param name="effectStrength">The strength of the effect.</param>
        /// <param name="effectRadius">The radius of the effect in tiles.</param>
        /// <param name="description">A description of the interaction.</param>
        /// <param name="cropEffect">The specific effect on the crop.</param>
        public void AddInteraction(int sourceCropId, int targetCropId, AllelopathicEffectType effectType, 
            float effectStrength, int effectRadius, string description, CropEffectType cropEffect = CropEffectType.Default)
        {
            if (!_interactions.ContainsKey(sourceCropId))
            {
                _interactions[sourceCropId] = new List<AllelopathicInteraction>();
            }
            
            _interactions[sourceCropId].Add(new AllelopathicInteraction
            {
                SourceCropId = sourceCropId,
                TargetCropId = targetCropId,
                EffectType = effectType,
                EffectStrength = effectStrength,
                EffectRadius = effectRadius,
                Description = description,
                CropEffect = cropEffect
            });
        }
        
        /// <summary>
        /// Adds an allelopathic interaction to the manager (legacy method).
        /// </summary>
        public void AddInteraction(int sourceCropId, int targetCropId, AllelopathicEffectType effectType, 
            float effectStrength, int effectRadius, string description)
        {
            AddInteraction(sourceCropId, targetCropId, effectType, effectStrength, effectRadius, description, CropEffectType.Default);
        }

        /// <summary>
        /// Gets all interactions for a specific source crop.
        /// </summary>
        /// <param name="sourceCropId">The source crop ID.</param>
        /// <returns>A list of allelopathic interactions.</returns>
        public List<AllelopathicInteraction> GetInteractionsForCrop(int sourceCropId)
        {
            return _interactions.ContainsKey(sourceCropId) 
                ? _interactions[sourceCropId] 
                : new List<AllelopathicInteraction>();
        }

        /// <summary>
        /// Initializes spring crop interactions.
        /// </summary>
        private void InitializeSpringInteractions()
        {
            // Green Bean — fixes nitrogen, benefits all nearby crops
            AddInteraction(CropIds.Spring.GreenBean, -1, AllelopathicEffectType.Positive, 0.2f, 1,
                "Green beans fix nitrogen in the soil, improving quality of nearby crops.", CropEffectType.Quality);

            // Parsnip
            AddInteraction(CropIds.Spring.Parsnip, CropIds.Spring.Cauliflower, AllelopathicEffectType.Negative, 0.2f, 1,
                "Parsnips inhibit cauliflower growth, reducing quality.", CropEffectType.Quality);

            // Cauliflower
            AddInteraction(CropIds.Spring.Cauliflower, CropIds.Spring.Potato, AllelopathicEffectType.Negative, 0.3f, 1,
                "Cauliflower competes with potatoes for nutrients, reducing harvest quantity.", CropEffectType.Quantity);
            AddInteraction(CropIds.Spring.Cauliflower, CropIds.Spring.Parsnip, AllelopathicEffectType.Negative, 0.2f, 1,
                "Cauliflower inhibits parsnip growth, reducing quality.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Cauliflower, CropIds.Spring.GreenBean, AllelopathicEffectType.Positive, 0.2f, 1,
                "Cauliflower benefits from green beans' nitrogen fixation, improving quality.", CropEffectType.Quality);

            // Garlic
            AddInteraction(CropIds.Spring.Garlic, CropIds.Spring.Strawberry, AllelopathicEffectType.Positive, 0.3f, 1,
                "Garlic repels pests that would damage strawberries, improving quality.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Garlic, CropIds.Spring.GreenBean, AllelopathicEffectType.Negative, 0.2f, 1,
                "Garlic inhibits the growth of green beans, reducing quality.", CropEffectType.Quality);

            // Kale
            AddInteraction(CropIds.Spring.Kale, CropIds.Spring.Potato, AllelopathicEffectType.Positive, 0.2f, 1,
                "Kale's deep roots bring up nutrients that benefit potatoes, increasing harvest quantity.", CropEffectType.Quantity);
            AddInteraction(CropIds.Spring.Kale, CropIds.Spring.GreenBean, AllelopathicEffectType.Positive, 0.2f, 1,
                "Kale benefits from green beans' nitrogen fixation, improving quality.", CropEffectType.Quality);

            // Potato
            AddInteraction(CropIds.Spring.Potato, CropIds.Spring.Rhubarb, AllelopathicEffectType.Positive, 0.25f, 1,
                "Potatoes deter certain pests that would otherwise attack rhubarb, improving quality.", CropEffectType.Quality);

            // Rhubarb
            AddInteraction(CropIds.Spring.Rhubarb, CropIds.Spring.Cauliflower, AllelopathicEffectType.Positive, 0.2f, 1,
                "Rhubarb provides shade that benefits cauliflower in hot weather, improving quality.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Rhubarb, CropIds.Spring.Parsnip, AllelopathicEffectType.Positive, 0.2f, 1,
                "Rhubarb enhances parsnip growth, improving quality.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Rhubarb, CropIds.Spring.GreenBean, AllelopathicEffectType.Positive, 0.2f, 1,
                "Rhubarb benefits from green beans' nitrogen fixation, improving quality.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Rhubarb, CropIds.Spring.Potato, AllelopathicEffectType.Negative, 0.2f, 1,
                "Rhubarb and potatoes compete for nutrients, reducing potato harvest quantity.", CropEffectType.Quantity);

            // Strawberry
            AddInteraction(CropIds.Spring.Strawberry, CropIds.Spring.Garlic, AllelopathicEffectType.Positive, 0.3f, 1,
                "Strawberries and garlic have a mutually beneficial relationship, reducing garlic's growth time.", CropEffectType.GrowthSpeed);
            AddInteraction(CropIds.Spring.Strawberry, CropIds.Spring.Potato, AllelopathicEffectType.Positive, 0.2f, 1,
                "Strawberries improve potato quality when grown nearby.", CropEffectType.Quality);
            AddInteraction(CropIds.Spring.Strawberry, CropIds.Spring.GreenBean, AllelopathicEffectType.Positive, 0.2f, 1,
                "Strawberries benefit from green beans' nitrogen fixation, reducing growth time.", CropEffectType.GrowthSpeed);
            AddInteraction(CropIds.Spring.Strawberry, CropIds.Spring.Strawberry, AllelopathicEffectType.Negative, 0.2f, 1,
                "Strawberries compete with each other when planted too closely, reducing fruit quality.", CropEffectType.Quality);
        }
        
        /// <summary>
        /// Initializes summer crop interactions.
        /// </summary>
        private void InitializeSummerInteractions()
        {
            // Tomatoes release substances that inhibit potato growth
            AddInteraction(CropIds.Summer.Tomato, CropIds.Spring.Potato, AllelopathicEffectType.Negative, 0.2f, 1, 
                "Tomatoes release substances that can inhibit potato growth.");
                
            // Sunflowers attract beneficial insects
            AddInteraction(CropIds.Summer.Sunflower, -1, AllelopathicEffectType.Positive, 0.1f, 2, 
                "Sunflowers attract beneficial insects that help nearby crops.");
                
            // Corn benefits from bean nitrogen fixation
            AddInteraction(CropIds.Spring.GreenBean, CropIds.Summer.Corn, AllelopathicEffectType.Positive, 0.3f, 1, 
                "Beans fix nitrogen in the soil, benefiting corn growth.");
                
            // Blueberries and strawberries compete for resources
            AddInteraction(CropIds.Summer.Blueberry, CropIds.Spring.Strawberry, AllelopathicEffectType.Negative, 0.25f, 1, 
                "Blueberries and strawberries compete for similar soil resources.");
            AddInteraction(CropIds.Spring.Strawberry, CropIds.Summer.Blueberry, AllelopathicEffectType.Negative, 0.25f, 1, 
                "Strawberries and blueberries compete for similar soil resources.");
                
            // Hot peppers can deter pests from other plants
            AddInteraction(CropIds.Summer.HotPepper, -1, AllelopathicEffectType.Positive, 0.15f, 1, 
                "Hot peppers can deter certain pests from nearby plants.");
                
            // Melons and radishes don't grow well together
            AddInteraction(CropIds.Summer.Melon, CropIds.Summer.Radish, AllelopathicEffectType.Negative, 0.2f, 1, 
                "Melons don't grow well with radishes.");
        }
        
        /// <summary>
        /// Initializes fall crop interactions.
        /// </summary>
        private void InitializeFallInteractions()
        {
            // Pumpkins are heavy feeders and can deplete soil nutrients
            AddInteraction(CropIds.Fall.Pumpkin, -1, AllelopathicEffectType.Negative, 0.15f, 1, 
                "Pumpkins are heavy feeders and can deplete soil nutrients.");
                
            // Fairy roses attract beneficial insects
            AddInteraction(CropIds.Fall.FairyRose, -1, AllelopathicEffectType.Positive, 0.2f, 2, 
                "Fairy roses attract beneficial insects that help nearby crops.");
                
            // Amaranth can help suppress weeds around other plants
            AddInteraction(CropIds.Fall.Amaranth, -1, AllelopathicEffectType.Positive, 0.1f, 1, 
                "Amaranth can help suppress weeds around other plants.");
                
            // Eggplants and potatoes don't grow well together (same family)
            AddInteraction(CropIds.Fall.Eggplant, CropIds.Spring.Potato, AllelopathicEffectType.Negative, 0.25f, 1, 
                "Eggplants and potatoes share diseases as they're in the same family.");
                
            // Yams and sweet potatoes compete for similar resources
            AddInteraction(CropIds.Fall.Yam, CropIds.Fall.Beet, AllelopathicEffectType.Negative, 0.2f, 1, 
                "Yams and beets compete for similar soil resources.");
                
            // Bok choy benefits from being near beets
            AddInteraction(CropIds.Fall.Beet, CropIds.Fall.BokChoy, AllelopathicEffectType.Positive, 0.15f, 1, 
                "Beets can help improve soil conditions for bok choy.");
        }
        
        /// <summary>
        /// Initializes special crop interactions.
        /// </summary>
        private void InitializeSpecialInteractions()
        {
            // Ancient fruit has mild beneficial effects on all crops
            AddInteraction(CropIds.Special.AncientFruit, -1, AllelopathicEffectType.Positive, 0.1f, 2, 
                "Ancient fruit seems to have mysterious beneficial properties for nearby plants.");
                
            // Sweet gem berries are extremely sensitive to other plants
            AddInteraction(-1, CropIds.Special.SweetGemBerry, AllelopathicEffectType.Negative, 0.3f, 1, 
                "Sweet gem berries are sensitive to the presence of other crops.");
                
            // Coffee beans help stimulate growth in nearby plants
            AddInteraction(CropIds.CoffeeBean, -1, AllelopathicEffectType.Positive, 0.15f, 1, 
                "Coffee plants release compounds that can stimulate growth in nearby plants.");
        }
        
        /// <summary>
        /// Gets the interaction between a source crop and a target crop.
        /// </summary>
        /// <param name="sourceCropId">The source crop ID.</param>
        /// <param name="targetCropId">The target crop ID.</param>
        /// <returns>The allelopathic interaction, or null if none exists.</returns>
        public AllelopathicInteraction? GetInteraction(int sourceCropId, int targetCropId)
        {
            // Check specific source → specific/wildcard target
            if (_interactions.TryGetValue(sourceCropId, out var sourceInteractions))
            {
                foreach (var interaction in sourceInteractions)
                {
                    if (interaction.TargetCropId == targetCropId || interaction.TargetCropId == -1)
                        return interaction;
                }
            }

            // Check wildcard source (-1) → specific target (e.g. "any crop affects Sweet Gem Berry")
            if (_interactions.TryGetValue(-1, out var wildcardInteractions))
            {
                foreach (var interaction in wildcardInteractions)
                {
                    if (interaction.TargetCropId == targetCropId)
                        return interaction;
                }
            }

            return null;
        }
    }
}
