using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ramuh
{
    public class ThunderCallerBuff : Buff
    {
        private const int Level = 3;

        public ThunderCallerBuff(
        ) : base(
            id: BuffHelper.GetBuffThunderCallerId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.thunder_caller.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.thunder_caller.description"),
            duration: ENDLESS,
            effects: new BuffEffects(),
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexThunderCaller())
        {
        }
    }
}