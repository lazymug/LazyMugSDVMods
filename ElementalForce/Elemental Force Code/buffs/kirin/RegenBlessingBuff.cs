using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.kirin
{
    public class RegenBlessingBuff : Buff
    {
        private const int Level = 3;
        
        public RegenBlessingBuff(
        ) : base(
            id: BuffHelper.GetBuffRegenBlessingId(), 
            displayName: ModEntry.Instance.GetTextTranslation("buff.regen_blessing.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.regen_blessing.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                // recovers Energy and Health from time to time
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexRegenBlessing())
        {
        }
    }
}