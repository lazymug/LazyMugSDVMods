using ElementalForce.Elemental_Force_Code.buffEffects;
using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.cactuar
{
    public class WarySpeedBuff : Buff
    {
        private const int Level = 1;
        
        public WarySpeedBuff(
        ) : base(
            id: BuffHelper.GetBuffWarySpeedId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.wary_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.wary_speed.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // must apply when hit
                Speed = { 2.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexWarySpeed())
        {
        }
    }
}