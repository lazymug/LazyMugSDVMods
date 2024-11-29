using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ifrit
{
    public class FireballBuff : Buff
    {
        private const int Level = 3;
        
        public FireballBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffFireballId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // TODO: it can cast fireball into the enemy similar to slingshot
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
        }
    }
}