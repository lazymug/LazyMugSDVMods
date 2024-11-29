using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.titan
{
    public class IronBodyBuff: Buff
    {
        private const int Level = 1;

        public IronBodyBuff(
            int currentDefense = 0,
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null)
            : base(
                id: BuffHelper.GetBuffIronBodyId(), 
                displayName: displayName, 
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    Defense = { currentDefense + 2 }
                }, 
                iconTexture: iconTexture, 
                iconSheetIndex: iconSheetIndex)
        {
            
        }

    }
}