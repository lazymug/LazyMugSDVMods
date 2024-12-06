using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ifrit
{
    public class HeatSpeedBuff : Buff
    {
        public HeatSpeedBuff(
            float currentSpeed = 0f,
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string description = null,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffHeatSpeedId(), 
            displayName: displayName,
            description: description,
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Speed = { currentSpeed * 1.25f }
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
            
        }
    }
}