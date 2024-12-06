using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.shiva
{
    public class IceTombBuff : Buff
    {
        private const int Level = 3;
        
        public IceTombBuff(
        ) : base(
            id: BuffHelper.GetBuffIceTombId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.ice_tomb.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.ice_tomb.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // todo: can freeze the enemy for some seconds
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexIceTomb())
        {
            
        }
    }
}