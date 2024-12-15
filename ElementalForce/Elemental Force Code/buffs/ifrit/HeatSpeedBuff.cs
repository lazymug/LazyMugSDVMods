using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ifrit
{
    public class HeatSpeedBuff : Buff
    {
        public HeatSpeedBuff(
            float currentSpeed = 0f
        ) : base(
            id: BuffHelper.GetBuffHeatSpeedId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.heat_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.heat_speed.description"),
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Speed = { 2.5f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexHeatSpeed())
        {
            
        }
    }
}