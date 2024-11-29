using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.leviathan
{
    public class DragonScaleBuff : Buff
    {
        private const int Level = 3;

        public DragonScaleBuff(
            int currentDefense = 0,
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null)
            : base(
                id: BuffHelper.GetBuffDragonScaleId(), 
                displayName: displayName, 
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    Defense = { currentDefense + 7 }
                }, 
                iconTexture: iconTexture, 
                iconSheetIndex: iconSheetIndex)
        {
            
        }
    }
}