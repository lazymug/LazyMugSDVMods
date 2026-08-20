using System.Collections.Generic;

namespace Allellopathy.Constants
{
    /// <summary>
    /// Contains constants for crop IDs organized by season.
    /// </summary>
    public static class CropIds
    {
        /// <summary>
        /// Spring crops and their IDs.
        /// </summary>
        public static class Spring
        {
            public const string BlueJazz = "597";
            public const string Cauliflower = "190";
            public const string CoffeeBean = "433";
            public const string Garlic = "248";
            public const string GreenBean = "188";
            public const string Kale = "250";
            public const string Parsnip = "24";
            public const string Potato = "192";
            public const string Rhubarb = "252";
            public const string Strawberry = "400";
            public const string Tulip = "591";
            public const string UnmilledRice = "271";
            
            // Carrot doesn't have a standard ID in vanilla Stardew Valley
            // public const int Carrot = -1;

            /// <summary>
            /// Gets all spring crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<string> All = new[]
            {
                BlueJazz, Cauliflower, CoffeeBean, Garlic, GreenBean, Kale,
                Parsnip, Potato, Rhubarb, Strawberry, Tulip, UnmilledRice
            };
        }

        /// <summary>
        /// Summer crops and their IDs.
        /// </summary>
        public static class Summer
        {
            public const string Blueberry = "481";
            public const string Corn = "272";
            public const string Hops = "304";
            public const string HotPepper = "260";
            public const string Melon = "254";
            public const string Poppy = "376";
            public const string Radish = "264";
            public const string RedCabbage = "266";
            public const string Starfruit = "268";
            public const string SummerSpangle = "593";
            public const string Sunflower = "432";
            public const string Tomato = "476";
            public const string WheatCrop = "271";
            
            // Summer Squash doesn't have a standard ID in vanilla Stardew Valley
            // public const int SummerSquash = -1;

            /// <summary>
            /// Gets all summer crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<string> All = new[]
            {
                Blueberry, Corn, Hops, HotPepper, Melon, Poppy, Radish,
                RedCabbage, Starfruit, SummerSpangle, Sunflower, Tomato, WheatCrop
            };
            
            /// <summary>
            /// Gets multi-season crops that grow in summer.
            /// </summary>
            public static readonly IReadOnlyList<string> MultiSeason = new[]
            {
                CoffeeBean, Corn, Sunflower, WheatCrop
            };
        }

        /// <summary>
        /// Fall crops and their IDs.
        /// </summary>
        public static class Fall
        {
            public const string Amaranth = "300";
            public const string Artichoke = "274";
            public const string Beet = "284";
            public const string BokChoy = "278";
            public const string Cranberries = "282";
            public const string Eggplant = "256";
            public const string FairyRose = "595";
            public const string Grape = "398";
            public const string Pumpkin = "276";
            public const string Yam = "280";
            
            // Broccoli doesn't have a standard ID in vanilla Stardew Valley
            // public const int Broccoli = -1;

            /// <summary>
            /// Gets all fall crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<string> All = new[]
            {
                Amaranth, Artichoke, Beet, BokChoy, Cranberries, Eggplant,
                FairyRose, Grape, Pumpkin, Yam
            };
            
            /// <summary>
            /// Gets multi-season crops that grow in fall.
            /// </summary>
            public static readonly IReadOnlyList<string> MultiSeason = new[]
            {
                Summer.Corn, Summer.Sunflower, Summer.WheatCrop
            };
        }

        /// <summary>
        /// Winter crops and their IDs.
        /// </summary>
        public static class Winter
        {
            // Winter crops are limited in vanilla Stardew Valley
            // Powdermelon is from 1.6 update
            // public const int Powdermelon = -1;
            
            /// <summary>
            /// Gets all winter crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<string> All = new string[0];
        }

        /// <summary>
        /// Special crops and their IDs.
        /// </summary>
        public static class Special
        {
            public const string AncientFruit = "454";
            public const string CactusFruit = "90";
            public const string Pineapple = "832";
            public const string SweetGemBerry = "347";
            public const string TaroRoot = "830";
            public const string TeaLeaves = "815";

            /// <summary>
            /// Gets all special crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<string> All = new[]
            {
                AncientFruit, CactusFruit, Pineapple, SweetGemBerry, TaroRoot, TeaLeaves
            };
        }

        /// <summary>
        /// Gets the coffee bean ID, which is used in multiple seasons.
        /// </summary>
        /// <summary>
        /// Crop IDs added by Cornucopia - More Crops / More Flowers.
        /// Only the ones with a real-world allelopathic story are listed.
        /// </summary>
        public static class Cornucopia
        {
            public const string Asparagus = "Cornucopia_Asparagus";
            public const string Basil = "Cornucopia_Basil";
            public const string BellPepper = "Cornucopia_BellPepper";
            public const string Cabbage = "Cornucopia_Cabbage";
            public const string Celery = "Cornucopia_Celery";
            public const string Chickpea = "Cornucopia_Chickpea";
            public const string Chives = "Cornucopia_Chives";
            public const string Cilantro = "Cornucopia_Cilantro";
            public const string Cucumber = "Cornucopia_Cucumber";
            public const string Dill = "Cornucopia_Dill";
            public const string Fennel = "Cornucopia_Fennel";
            public const string GreenPeas = "Cornucopia_GreenPeas";
            public const string Lentils = "Cornucopia_Lentils";
            public const string Lettuce = "Cornucopia_Lettuce";
            public const string Mint = "Cornucopia_Mint";
            public const string Oats = "Cornucopia_Oats";
            public const string Onion = "Cornucopia_Onion";
            public const string AdzukiBean = "Cornucopia_AdzukiBean";
            public const string BlackBeans = "Cornucopia_BlackBeans";
            public const string KidneyBeans = "Cornucopia_KidneyBeans";
            public const string NavyBeans = "Cornucopia_NavyBeans";

            /// <summary>Legumes fix nitrogen, so they feed their neighbours.</summary>
            public static readonly IReadOnlyList<string> Legumes = new[]
            {
                AdzukiBean, BlackBeans, Chickpea, GreenPeas, KidneyBeans, Lentils, NavyBeans
            };

            /// <summary>Alliums repel pests, but are hard on legumes.</summary>
            public static readonly IReadOnlyList<string> Alliums = new[] { Chives, Onion };
        }

        /// <summary>
        /// Crop IDs added by Wildflour's Atelier Goods - mostly culinary herbs.
        /// </summary>
        public static class Wildflour
        {
            public const string Basil = "Wildflour.AtelierGoods_Basil";
            public const string Chamomile = "Wildflour.AtelierGoods_Chamomile";
            public const string Cilantro = "Wildflour.AtelierGoods_Cilantro";
            public const string Jasmine = "Wildflour.AtelierGoods_Jasmine";
            public const string Lavender = "Wildflour.AtelierGoods_Lavender";
            public const string Mallow = "Wildflour.AtelierGoods_Mallow";
            public const string Mint = "Wildflour.AtelierGoods_Mint";
            public const string RedOnion = "Wildflour.AtelierGoods_Red_Onion";
            public const string Rosemary = "Wildflour.AtelierGoods_Rosemary";
            public const string Sage = "Wildflour.AtelierGoods_Sage";
            public const string Thyme = "Wildflour.AtelierGoods_Thyme";
            public const string Vanilla = "Wildflour.AtelierGoods_Vanilla";
            public const string WildRaspberry = "Wildflour.AtelierGoods_Wild_Raspberry";
            public const string WildStrawberry = "Wildflour.AtelierGoods_Wild_Strawberry";
            public const string Wormwood = "Wildflour.AtelierGoods_Wormwood";

            /// <summary>Aromatic herbs whose oils deter pests on their neighbours.</summary>
            public static readonly IReadOnlyList<string> AromaticHerbs = new[]
            {
                Basil, Chamomile, Cilantro, Lavender, Mint, Rosemary, Sage, Thyme
            };
        }

        /// <summary>Matches any crop, as source or as target.</summary>
        public const string Any = "*";

        /// <summary>
        /// Gets the coffee bean ID, which is used in multiple seasons.
        /// </summary>
        public const string CoffeeBean = "433";
    }
}
