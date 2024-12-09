using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class RainWishBuff : Buff
    {
        private const int level = 2;

        public RainWishBuff()
            : base(
                id: BuffHelper.GetBuffRainWishId(), 
                displayName: ModEntry.Instance.GetTextTranslation("buff.rain_wish.name"),
                description: ModEntry.Instance.GetTextTranslation("buff.rain_wish.description"),  
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    // todo: increase chance of rain on next day
                }, 
                iconTexture: BuffHelper.GetIconTexture(), 
                iconSheetIndex: BuffHelper.GetBuffIndexRainWish())
        {
            
        }
    }
}