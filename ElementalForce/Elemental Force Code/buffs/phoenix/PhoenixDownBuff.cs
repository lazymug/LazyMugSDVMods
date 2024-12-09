using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.phoenix
{
    public class PhoenixDownBuff : Buff
    {
        private const int Level = 3;
        
        public PhoenixDownBuff(
        ) : base(
            id: BuffHelper.GetBuffPhoenixDownId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.phoenix_down.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.phoenix_down.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // Revives the hero when dead or fatigued with 25% of energy or health
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexPhoenixDown())
        {
        }
    }
}