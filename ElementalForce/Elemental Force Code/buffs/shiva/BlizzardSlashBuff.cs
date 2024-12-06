using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.shiva
{
    public class BlizzardSlashBuff : Buff
    {
        private const int Level = 2;
        
        public BlizzardSlashBuff(
        ) : base(
            id: BuffHelper.GetBuffBlizzardSlashId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.blizzard_slash.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.blizzard_slash.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                CriticalChanceMultiplier = { 1.05f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexBlizzardSlash())
        {
            
        }
    }
}