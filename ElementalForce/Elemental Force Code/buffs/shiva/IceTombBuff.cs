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
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null
        ) : base(
            id: BuffHelper.GetBuffIceTombId(), 
            displayName: displayName, 
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // todo: can freeze the enemy for some seconds
            }, 
            iconTexture: iconTexture, 
            iconSheetIndex: iconSheetIndex)
        {
            
        }
    }
}