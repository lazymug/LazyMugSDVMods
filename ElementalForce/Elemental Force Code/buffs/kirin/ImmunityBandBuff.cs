using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.kirin
{
    public class ImmunityBandBuff : Buff
    {
        private const int Level = 1;
        
        public ImmunityBandBuff(
            int currentImmunity
        ) : base(
            id: BuffHelper.GetBuffImmunityBandId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.immunity_band.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.immunity_band.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Immunity = { 4.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexImmunityBand())
        {
        }
    }
}