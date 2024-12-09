using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.carbuncle
{
    public class MirrorReflectionBuff : Buff
    {
        private const int Level = 3;
        
        public MirrorReflectionBuff(
        ) : base(
            id: BuffHelper.GetBuffMirrorReflectionId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.mirror_reflection.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.mirror_reflection.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // todo: it has a chance to absorb the damage and return a percent of the damage to the enemy
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexMirrorReflection())
        {
        }
    }
}