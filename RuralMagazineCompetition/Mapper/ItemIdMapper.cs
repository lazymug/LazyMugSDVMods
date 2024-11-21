namespace RuralMagazineCompetition.Mapper
{
    public static class ItemIdMapper
    {

        public static class AnimalProduces
        {
            public const string Egg = "176";
            public const string LargeEgg = "174";
            public const string BrownEgg = "180";
            public const string LargeBrownEgg = "182";
            public const string VoidEgg = "305";
            public const string GoldenEgg = "928";
            public const string DuckEgg = "442";
            public const string DuckFeather = "444";
            public const string Wool = "440";
            public const string RabbitsFoot = "446";
            public const string DinosaurEgg = "107";
            public const string Milk = "184";
            public const string LargeMilk = "186";
            public const string GoatMilk = "436";
            public const string LargeGoatMilk = "438";
            public const string Truffle = "430";
            public const string OstrichEgg = "289";
            // todo: include ROE

            public static List<string> GetAll()
            {
                return new List<string>
                {
                    Egg, LargeEgg, BrownEgg, LargeBrownEgg, VoidEgg, GoldenEgg, DuckEgg,
                    DuckFeather, Wool, RabbitsFoot, DinosaurEgg, Milk, LargeMilk, GoatMilk,
                    LargeGoatMilk, Truffle, OstrichEgg
                };
            }

            public static int GetBasePoints(string itemId)
            {
                switch (itemId)
                {
                    case Egg:
                    case BrownEgg:
                        return 25;
                    case LargeEgg:
                    case LargeBrownEgg:
                        return 48;
                    case VoidEgg:
                        return 32;
                    case GoldenEgg:
                        return 225;
                    case DuckEgg:
                        return 45;
                    case DuckFeather:
                        return 105;
                    case Wool:
                        return 160;
                    case RabbitsFoot:
                        return 240;
                    case DinosaurEgg:
                        return 165;
                    case Milk:
                        return 60;
                    case LargeMilk:
                        return 95;
                    case GoatMilk:
                        return 100;
                    case LargeGoatMilk:
                        return 170;
                    case Truffle:
                        return 300;
                    case OstrichEgg:
                        return 290;
                    default:
                        return 0;
                }
            }
        }

        public static class ArtisanGoods
        {
            public const string Honey = "340";
            // todo: try to figure out how to get different kinds of honey
            public const string TulipHoney = "Egg";
            public const string BlueJazzHoney = "Egg";
            public const string SunflowerHoney = "Egg";
            public const string SummerSpangleHoney = "Egg";
            public const string PoppyHoney = "Egg";
            public const string FairyRoseHoney = "Egg";
            // todo: try to figure out how to get different kinds of wine
            public const string Wine = "348";
            public const string PaleAle = "303";
            public const string Beer = "346";
            public const string Mead = "459";
            public const string Cheese = "424";
            public const string GoatCheese = "426";
            // todo: try to figure out how to get different kinds of juice
            public const string Juice = "350";
            public const string GreenTea = "614";
            public const string Cloth = "428";
            public const string Mayonnaise = "306";
            public const string VoidMayonnaise = "308";
            public const string DuckMayonnaise = "307";
            public const string DinosaurMayonnaise = "807";
            public const string TruffleOil = "432";
            // todo: try to figure out how to get different kinds of pickles
            public const string Pickles = "342";
            // todo: try to figure out how to get different kinds of jelly
            public const string Jelly = "344";
            // todo: try to figure out how to get different kinds of aged roe
            public const string AgedRoe = "447";
            public const string Caviar = "445";
            // todo: try to figure out how to get different kinds of dried mushrooms
            public const string DriedMushrooms = "DriedMushrooms";
            // todo: try to figure out how to get different kinds of dried fruit
            public const string DriedFruits = "DriedFruits";
            // todo: try to figure out how to get different kinds of smoked fish
            public const string SmokedFish = "SmokedFish";
            public const string Raisins = "Raisins";

            public static List<string> GetAll()
            {
                return new List<string>
                {
                    Honey, Wine, PaleAle, Beer, Mead, Cheese, GoatCheese, Juice, GreenTea, Cloth,
                    Mayonnaise, VoidMayonnaise, DuckMayonnaise, DinosaurMayonnaise, TruffleOil,
                    Pickles, Jelly, AgedRoe, Caviar, DriedMushrooms, DriedFruits, SmokedFish,
                    Raisins
                };
            }
            
            public static int GetBasePoints(string itemId)
            {
                switch (itemId)
                {
                    case Honey:
                        return 55;
                    case Wine:
                        return 95;
                    case PaleAle:
                        return 120;
                    case Beer:
                        return 70;
                    case Mead:
                        return 120;
                    case Cheese:
                        return 90;
                    case GoatCheese:
                        return 160;
                    case Juice:
                        return 90;
                    case GreenTea:
                        return 50;
                    case Cloth:
                        return 210;
                    case Mayonnaise:
                        return 80;
                    case VoidMayonnaise:
                        return 130;
                    case DuckMayonnaise:
                        return 155;
                    case DinosaurMayonnaise:
                        return 340;
                    case TruffleOil:
                        return 480;
                    case Pickles:
                        return 60;
                    case Jelly:
                        return 60;
                    case AgedRoe:
                        return 75;
                    case Caviar:
                        return 225;
                    case DriedMushrooms:
                        return 215;
                    case DriedFruits:
                        return 215;
                    case SmokedFish:
                        return 100;
                    case Raisins:
                        return 100;
                    default:
                        return 0;
                }
            }
        }
        public static class Crops
        {
            // Spring Crops
            public const string BlueJazz = "597";
            public const string Cauliflower = "190";
            public const string Garlic = "248";
            public const string GreenBean = "188";
            public const string Kale = "250";
            public const string Parsnip = "24";
            public const string Potato = "192";
            public const string Rhubarb = "252";
            public const string Strawberry = "400";
            public const string Tulip = "591";
            public const string Carrot = "Carrot";

            // Summer Crops
            public const string Blueberry = "258";
            public const string CactusFruit = "90";
            public const string Hops = "304";
            public const string Melon = "254";
            public const string Pepper = "260";
            public const string Radish = "264";
            public const string RedCabbage = "266";
            public const string Starfruit = "268";
            public const string Sunflower = "421";
            public const string SummerSpangle = "593";
            public const string Tomato = "256";
            public const string Wheat = "262";
            public const string SummerSquash = "SummerSquash";

            // Fall Crops
            public const string Amaranth = "300";
            public const string Artichoke = "274";
            public const string Beet = "284";
            public const string Bokchoy = "278";
            public const string Corn = "270";
            public const string Cranberries = "282";
            public const string Eggplant = "272";
            public const string FairyRose = "595";
            public const string Grape = "398";
            public const string Pumpkin = "276";
            public const string Yam = "280";
            public const string Broccoli = "Broccoli";
            
            // Winter Crops
            public const string Powdermelon = "Powdermelon";
        }

        public static class Fishes
        {
            // Spring Fishes

            // Summer Fishes
            public const string Dorado = "704";
            public const string Octopus = "149";
            public const string Pufferfish = "128";
            public const string RainbowTrout = "138";

            // Fall Fishes
            public const string Salmon = "139";

            // Winter Fishes
            public const string Blobfish = "800";
            public const string Lingcod = "707";
            public const string MidnightSquid = "798";
            public const string Perch = "141";
            public const string SpookyFish = "799";
            public const string Squid = "151";
            
            // All Seasons
            public const string BlueDiscus = "838"; // Only available after Ginger Island
            public const string Bream = "132";
            public const string Bullhead = "700";
            public const string Chub = "702";
            public const string IcePip = "161";
            public const string Ghostfish = "156";
            public const string Goby = "Goby";
            public const string LargemouthBass = "136";
            public const string LavaEel = "162";
            public const string Lionfish = "837"; // Only available after Ginger Island
            public const string SandFish = "164";
            public const string ScorpionCarp = "165";
            public const string SlimeJack = "796"; // Only available after Witch's Swamp
            public const string Stingray = "836"; // Only available after Ginger Island
            public const string Stonefish = "158";
            public const string VoidSalmon = "795"; // Only available after Witch's Swamp
            public const string Woodskip = "734";
            
            
            // Spring and Fall
            public const string Anchovy = "129";
            public const string Eel = "148";
            public const string SmallmouthBass = "137";
            
            // Spring and Summer
            public const string Flounder = "267";
            public const string Sunfish = "145";

            // Summer, Fall, and Winter
            public const string RedSnapper = "150";
            
            // Summer and Fall
            public const string SuperCucumber = "155";
            public const string Tilapia = "701";
            
            // Spring, Fall, and Winter
            public const string Sardine = "131";
            
            // Spring, Summer, and Winter
            public const string Halibut = "708";
            
            // Spring, Summer, and Fall
            public const string Catfish = "143";
            public const string Shad = "706";

            // Summer and Winter
            public const string Pike = "134";
            public const string RedMullet = "146";
            public const string Sturgeon = "698";
            public const string Tuna = "130";
            
            // Fall and Winter
            public const string Albacore = "705";
            public const string MidnightCarp = "269";
            public const string SeaCucumber = "154";
            public const string TigerTrout = "699";
            public const string Walleye = "140";

            // Spring and Winter
            public const string Herring = "147";
            
            // Legend Fishes
            public const string Angler = "160"; // Fall
            public const string CrimsonFish = "159"; // Summer
            public const string Glacierfish = "775"; // Winter
            public const string Legend = "163"; // Spring
            public const string MutantCarp = "682"; // All
        }

        public static class Forages
        {
            // Spring Forages
            public const string Daffodil = "18";
            public const string Dandelion = "22";
            public const string Leek = "20";
            public const string SpringOnion = "399";
            public const string WildHorseradish = "16";

            // Summer Forages
            public const string CactusFruit = "90";
            public const string Coconut = "88";
            public const string FiddleheadFern = "259";
            public const string SpiceBerry = "396";
            public const string SweetPea = "402";

            // Fall Forages
            public const string Blackberry = "410";
            public const string CommonMushroom = "404";
            public const string Chanterelle = "281";
            public const string Hazelnut = "408";
            public const string WildPlum = "406";

            // Winter Forages
            public const string Crocus = "418";
            public const string CrystalFruit = "414";
            public const string Holly = "283";
            public const string SnowYam = "416";
            public const string WinterRoot = "412";
            
            // All Seasons
            public const string MysticSyrup = "MysticSyrup";
            public const string MapleSyrup = "724";
            public const string OakResin = "725";
            public const string PineTar = "726";
        }

        public static class Minerals
        {
            public const string Quartz = "80";
            public const string EarthCrystal = "86";
            public const string FrozenTear = "84";
            public const string FireQuartz = "82";
            public const string Emerald = "60";
            public const string Aquamarine = "62";
            public const string Ruby = "64";
            public const string Amethyst = "66";
            public const string Topaz = "68";
            public const string Jade = "70";
            public const string Diamond = "72";
            public const string PrismaticShard = "74";
            public const string Tigerseye = "562";
            public const string Opal = "564";
            public const string FireOpal = "565";
            public const string Alamite = "538";
            public const string Bixite = "539";
            public const string Baryte = "540";
            public const string Aerinite = "541";
            public const string Calcite = "542";
            public const string Dolomite = "543";
            public const string Esperite = "544";
            public const string Fluorapatite = "545";
            public const string Geminite = "546";
            public const string Helvite = "547";
            public const string Jamborite = "548";
            public const string Jagoite = "549";
            public const string Kyanite = "550";
            public const string Lunarite = "551";
            public const string Malachite = "552";
            public const string Neptunite = "553";
            public const string LemonStone = "554";
            public const string Nekoite = "555";
            public const string Orpiment = "556";
            public const string PetrifiedSlime = "557";
            public const string ThunderEgg = "558";
            public const string Pyrite = "559";
            public const string OceanStone = "560";
            public const string GhostCrystal = "561";
            public const string Jasper = "563";
            public const string Celestine = "566";
            public const string Marble = "567";
            public const string Sandstone = "568";
            public const string Granite = "569";
            public const string Basalt = "570";
            public const string Limestone = "571";
            public const string Soapstone = "572";
            public const string Hematite = "573";
            public const string Mudstone = "574";
            public const string Obsidian = "575";
            public const string Slate = "576";
            public const string FairyStone = "577";
            public const string StarShards = "578";

            public static List<string> GetAll()
            {
                return new List<string>
                {
                    Quartz, EarthCrystal, FrozenTear, FireQuartz, Emerald, Aquamarine, Ruby, Amethyst,
                    Topaz, Jade, Diamond, PrismaticShard, ThunderEgg, Pyrite, OceanStone, GhostCrystal,
                    Tigerseye, Opal, FireOpal, Alamite, Bixite, Baryte, Aerinite, Calcite, Dolomite,
                    Esperite, Fluorapatite, Jasper, Celestine, Marble, Sandstone, Granite,
                    Geminite, Helvite, Jamborite, Jagoite, Kyanite, Lunarite, Malachite, Neptunite,
                    LemonStone, Nekoite, Orpiment, PetrifiedSlime, Basalt, Limestone, Soapstone, Hematite,
                    Mudstone, Obsidian, Slate, FairyStone, StarShards
                };
            }

            public static int GetBasePoints(string itemId)
            {
                switch (itemId)
                {
                    case Quartz:
                        return 12;
                    case EarthCrystal:
                        return 20;
                    case FrozenTear:
                        return 32;
                    case FireQuartz:
                        return 50;
                    case Emerald:
                        return 140;
                    case Aquamarine:
                        return 100;
                    case Ruby:
                        return 140;
                    case Amethyst:
                        return 55;
                    case Topaz:
                        return 50;
                    case Jade:
                        return 110;
                    case Diamond:
                        return 430;
                    case PrismaticShard:
                        return 0;
                    case Tigerseye:
                        return 170;
                    case Opal:
                        return 85;
                    case FireOpal:
                        return 210;
                    case Alamite:
                        return 80;
                    case Bixite:
                        return 180;
                    case Baryte:
                        return 30;
                    case Aerinite:
                        return 75;
                    case Calcite:
                        return 40;
                    case Dolomite:
                        return 185;
                    case Esperite:
                        return 60;
                    case Fluorapatite:
                        return 125;
                    case GhostCrystal:
                        return 120;
                    case Jasper:
                        return 90;
                    case Celestine:
                        return 80;
                    case Marble:
                        return 65;
                    case Sandstone:
                        return 35;
                    case Granite:
                        return 40;
                    case Geminite:
                        return 90;
                    case Helvite:
                        return 290;
                    case Jamborite:
                        return 90;
                    case Jagoite:
                        return 70;
                    case Kyanite:
                        return 155;
                    case Lunarite:
                        return 125;
                    case Malachite:
                        return 65;
                    case Neptunite:
                        return 260;
                    case LemonStone:
                        return 125;
                    case Nekoite:
                        return 50;
                    case Orpiment:
                        return 50;
                    case PetrifiedSlime:
                        return 70;
                    case Basalt:
                        return 110;
                    case Limestone:
                        return 5;
                    case Soapstone:
                        return 75;
                    case Hematite:
                        return 95;
                    case Mudstone:
                        return 10;
                    case Obsidian:
                        return 120;
                    case Slate:
                        return 50;
                    case FairyStone:
                        return 155;
                    case StarShards:
                        return 330;
                    default:
                        return 0;
                }
            }
        }
    }
}