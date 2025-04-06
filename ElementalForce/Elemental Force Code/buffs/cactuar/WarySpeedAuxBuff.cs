using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.cactuar
{
    public class WarySpeedAuxBuff : Buff
    {
        private const int Level = 1;
        
        public WarySpeedAuxBuff(
        ) : base(
            id: BuffHelper.GetBuffWarySpeedAuxId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.wary_speed.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.wary_speed.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
            }, 
            iconTexture: null, 
            iconSheetIndex: -1)
        {
        }
    }
}