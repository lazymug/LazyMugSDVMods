using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class ExplosionBuff : Buff
    {
        private const int Level = 2;
        
        public ExplosionBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffExplosionId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // TODO: it can explode the enemy
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
        }
    }
}