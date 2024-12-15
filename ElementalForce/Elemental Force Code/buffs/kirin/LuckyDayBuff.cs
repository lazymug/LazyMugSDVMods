using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.kirin
{
    public class LuckyDayBuff : Buff
    {
        private const int Level = 2;
        
        public LuckyDayBuff(
            int currentLuck
        ) : base(
            id: BuffHelper.GetBuffLuckDayId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.lucky_day.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.lucky_day.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                LuckLevel = { 2.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexLuckDay())
        {
        }
    }
}