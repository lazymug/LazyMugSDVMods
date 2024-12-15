using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.titan
{
    public class EndlessStaminaBuff : Buff
    {
        private const int Level = 3;
        
        public EndlessStaminaBuff(
            int currentStamina
        ) : base(
            id: BuffHelper.GetBuffEndlessStaminaId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.endless_stamina.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.endless_stamina.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                MaxStamina = { 1.35f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexEndlessStamina())
        {
        }
    }
}