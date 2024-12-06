using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.carbuncle
{
    public class CompanionProtectionBuff : Buff
    {
        private const int Level = 2;
        
        public CompanionProtectionBuff(
        ) : base(
            id: BuffHelper.GetBuffCompanionProtectionId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.companion_protection.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.companion_protection.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexCompanionProtection())
        {
        }
    }
}