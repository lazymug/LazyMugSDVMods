using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class RainWishBuff : Buff
    {
        private const int level = 2;

        public RainWishBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null)
            : base(
                id: BuffHelper.GetBuffRainWishId(), 
                displayName: displayName, 
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    // todo: increase chance of rain on next day
                }, 
                iconTexture: iconTexture, 
                iconSheetIndex: iconSheetIndex)
        {
            
        }
    }
}