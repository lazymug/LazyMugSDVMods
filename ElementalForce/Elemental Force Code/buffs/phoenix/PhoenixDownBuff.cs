using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class PhoenixDownBuff : Buff
    {
        private const int Level = 3;
        
        public PhoenixDownBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffPhoenixDownId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // Revives the hero when dead or fatigued with 25% of energy or health
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
        }
    }
}