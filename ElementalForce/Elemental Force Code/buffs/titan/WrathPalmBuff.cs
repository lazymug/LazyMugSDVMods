using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.titan
{
    public class WrathPalmBuff : Buff
    {
        private const int Level = 2;

        public WrathPalmBuff(
            Texture2D iconTexture = null,
            int iconSheetIndex = -1,
            string displayName = null)
            : base(
                id: BuffHelper.GetBuffWrathPalmId(), 
                displayName: displayName, 
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    AttackMultiplier = { 1.25f }
                }, 
                iconTexture: iconTexture, 
                iconSheetIndex: iconSheetIndex)
        {
            
        }
    }
}