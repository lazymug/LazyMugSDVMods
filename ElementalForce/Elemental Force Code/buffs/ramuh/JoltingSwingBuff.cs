using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.ramuh
{
    public class JoltingSwingBuff : Buff
    {
        private const int Level = 3;
        
        public JoltingSwingBuff(
        ) : base(
            id: BuffHelper.GetBuffJoltingSwingId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.jolting_swing.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.jolting_swing.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                WeaponSpeedMultiplier = { 1.3f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexJoltingSwing())
        {
        }
    }
}