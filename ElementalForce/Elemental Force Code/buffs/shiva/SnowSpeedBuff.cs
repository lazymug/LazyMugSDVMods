using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.shiva
{
    public class SnowSpeedBuff : Buff
    {
        private const int Level = 1;
        
        public SnowSpeedBuff(
            float playerCurrentSpeed = 0f
        ) : base(
            id: BuffHelper.GetBuffSnowSpeedId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.snow_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.snow_speed.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Speed = { 3.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexSnowSpeed())
        {
            
        }
    }
}