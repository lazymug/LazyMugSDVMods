using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.titan
{
    public class WrathPalmBuff : Buff
    {
        private const int Level = 2;

        public WrathPalmBuff()
            : base(
                id: BuffHelper.GetBuffWrathPalmId(), 
                displayName: ModEntry.Instance.GetTextTranslation("buff.wrath_palm.name"),
                description: ModEntry.Instance.GetTextTranslation("buff.wrath_palm.description"),  
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    AttackMultiplier = { 1.25f }
                }, 
                iconTexture: BuffHelper.GetIconTexture(), 
                iconSheetIndex: BuffHelper.GetBuffIndexWrathPalm())
        {
            
        }
    }
}