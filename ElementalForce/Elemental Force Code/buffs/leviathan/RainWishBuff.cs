using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class RainWishBuff : Buff
    {
        private const int Level = 2;

        public RainWishBuff()
            : base(
                id: BuffHelper.GetBuffRainWishId(),
                displayName: ModEntry.Instance.GetTextTranslation("buff.rain_wish.name"),
                description: ModEntry.Instance.GetTextTranslation("buff.rain_wish.description"),
                duration: ENDLESS,
                effects: new BuffEffects(),
                iconTexture: BuffHelper.GetIconTexture(),
                iconSheetIndex: BuffHelper.GetBuffIndexRainWish())
        {
        }
    }
}
