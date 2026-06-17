using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class HealingAuraBuff : Buff
    {
        private const int Level = 1;

        public HealingAuraBuff(
        ) : base(
            id: BuffHelper.GetBuffHealingAuraId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.healing_aura.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.healing_aura.description"),
            duration: ENDLESS,
            effects: new BuffEffects(),
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexHealingAura())
        {
        }
    }
}