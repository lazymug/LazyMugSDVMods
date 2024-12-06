using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class HeavyBodyBuff : Buff
    {
        private const int Level = 1;
        
        public HeavyBodyBuff(
        ) : base(
            id: BuffHelper.GetBuffHeavyBodyId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.heavy_body.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.heavy_body.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                KnockbackMultiplier = { 1.35f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexHeavyBody())
        {
        }
    }
}