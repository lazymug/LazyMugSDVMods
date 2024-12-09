using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ramuh
{
    public class FlashSpeedBuff : Buff
    {
        private const int Level = 1;
        
        public FlashSpeedBuff(
            float currentSpeed
        ) : base(
            id: BuffHelper.GetBuffFlashSpeedId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.flash_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.flash_speed.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Speed = { currentSpeed * 1.15f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexFlashSpeed())
        {
        }
    }
}