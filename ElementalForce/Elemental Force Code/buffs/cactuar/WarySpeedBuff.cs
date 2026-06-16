using ElementalForce.Elemental_Force_Code.helpers;
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
            duration: BuffConstants.WarySpeedDurationMs,
            effects: new BuffEffects()
            {
                Speed = { 2.0f }
            },
            iconTexture: BuffHelper.GetIconTexture(),
            iconSheetIndex: BuffHelper.GetBuffIndexWarySpeed())
        {
        }
    }
}
