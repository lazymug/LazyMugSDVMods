using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.carbuncle
{
    public class SunnySpeedBuff : Buff
    {
        private const int Level = 1;
        
        public SunnySpeedBuff(
            float currentSpeed
        ) : base(
            id: BuffHelper.GetBuffSunnySpeedId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.sunny_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.sunny_speed.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // Improve speed on sunny days
                Speed = { 3.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexSunnySpeed())
        {
        }
    }
}