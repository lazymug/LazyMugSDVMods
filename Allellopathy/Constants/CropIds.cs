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
            public const int BlueJazz = 597;
            public const int Cauliflower = 190;
            public const int CoffeeBean = 433;
            public const int Garlic = 248;
            public const int GreenBean = 188;
            public const int Kale = 250;
            public const int Parsnip = 24;
            public const int Potato = 192;
            public const int Rhubarb = 252;
            public const int Strawberry = 400;
            public const int Tulip = 591;
            public const int UnmilledRice = 271;
            
            // Carrot doesn't have a standard ID in vanilla Stardew Valley
            // public const int Carrot = -1;

            /// <summary>
            /// Gets all spring crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<int> All = new[]
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
            public const int Blueberry = 481;
            public const int Corn = 272;
            public const int Hops = 304;
            public const int HotPepper = 260;
            public const int Melon = 254;
            public const int Poppy = 376;
            public const int Radish = 264;
            public const int RedCabbage = 266;
            public const int Starfruit = 268;
            public const int SummerSpangle = 593;
            public const int Sunflower = 432;
            public const int Tomato = 476;
            public const int WheatCrop = 271;
            
            // Summer Squash doesn't have a standard ID in vanilla Stardew Valley
            // public const int SummerSquash = -1;

            /// <summary>
            /// Gets all summer crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<int> All = new[]
            {
                Blueberry, Corn, Hops, HotPepper, Melon, Poppy, Radish,
                RedCabbage, Starfruit, SummerSpangle, Sunflower, Tomato, WheatCrop
            };
            
            /// <summary>
            /// Gets multi-season crops that grow in summer.
            /// </summary>
            public static readonly IReadOnlyList<int> MultiSeason = new[]
            {
                CoffeeBean, Corn, Sunflower, WheatCrop
            };
        }

        /// <summary>
        /// Fall crops and their IDs.
        /// </summary>
        public static class Fall
        {
            public const int Amaranth = 300;
            public const int Artichoke = 274;
            public const int Beet = 284;
            public const int BokChoy = 278;
            public const int Cranberries = 282;
            public const int Eggplant = 256;
            public const int FairyRose = 595;
            public const int Grape = 398;
            public const int Pumpkin = 276;
            public const int Yam = 280;
            
            // Broccoli doesn't have a standard ID in vanilla Stardew Valley
            // public const int Broccoli = -1;

            /// <summary>
            /// Gets all fall crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<int> All = new[]
            {
                Amaranth, Artichoke, Beet, BokChoy, Cranberries, Eggplant,
                FairyRose, Grape, Pumpkin, Yam
            };
            
            /// <summary>
            /// Gets multi-season crops that grow in fall.
            /// </summary>
            public static readonly IReadOnlyList<int> MultiSeason = new[]
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
            public static readonly IReadOnlyList<int> All = new int[0];
        }

        /// <summary>
        /// Special crops and their IDs.
        /// </summary>
        public static class Special
        {
            public const int AncientFruit = 454;
            public const int CactusFruit = 90;
            public const int Pineapple = 832;
            public const int SweetGemBerry = 347;
            public const int TaroRoot = 830;
            public const int TeaLeaves = 815;

            /// <summary>
            /// Gets all special crop IDs.
            /// </summary>
            public static readonly IReadOnlyList<int> All = new[]
            {
                AncientFruit, CactusFruit, Pineapple, SweetGemBerry, TaroRoot, TeaLeaves
            };
        }

        /// <summary>
        /// Gets the coffee bean ID, which is used in multiple seasons.
        /// </summary>
        public const int CoffeeBean = 433;
    }
}
