using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class ExplosionBuff : Buff
    {
        private const int Level = 2;
        
        public ExplosionBuff(
        ) : base(
            id: BuffHelper.GetBuffExplosionId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.explosion.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.explosion.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // TODO: it can explode the enemy
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexExplosion())
        {
        }
    }
}