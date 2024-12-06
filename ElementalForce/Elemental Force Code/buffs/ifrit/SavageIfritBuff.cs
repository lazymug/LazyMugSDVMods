using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ifrit
{
    public class SavageIfritBuff : Buff
    {
        private const int level = 2;
        
        public SavageIfritBuff(
        ) : base(
            id: BuffHelper.GetBuffSavageIfritId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.savage_ifrit.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.savage_ifrit.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                AttackMultiplier = { 1.1f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexSavageIfrit())
        {
        }
    }
}