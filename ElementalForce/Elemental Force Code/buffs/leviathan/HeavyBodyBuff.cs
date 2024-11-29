using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class HeavyBodyBuff : Buff
    {
        private const int Level = 1;
        
        public HeavyBodyBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffHeavyBodyId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                KnockbackMultiplier = { 1.35f }
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
        }
    }
}