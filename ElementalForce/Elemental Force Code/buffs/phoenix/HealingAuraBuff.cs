using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class HealingAuraBuff : Buff
    {
        private const int Level = 1;
        
        public HealingAuraBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffHealingAuraId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // TODO: once per day restores user health and energy
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
        }
    }
}