using ElementalForce.Elemental_Force_Code.helpers;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ifrit
{
    public class FireballBuff : Buff
    {
        private const int Level = 3;

        public FireballBuff(
        ) : base(
            id: BuffHelper.GetBuffFireballId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.fireball.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.fireball.description"),
            duration: ENDLESS,
            effects: new BuffEffects(),
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexFireball())
        {
        }
    }
}