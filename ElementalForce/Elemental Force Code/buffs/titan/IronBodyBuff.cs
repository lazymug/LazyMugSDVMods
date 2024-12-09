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
            int currentDefense = 0)
            : base(
                id: BuffHelper.GetBuffIronBodyId(), 
                displayName: ModEntry.Instance.GetTextTranslation("buff.iron_body.name"),
                description: ModEntry.Instance.GetTextTranslation("buff.iron_body.description"),  
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    Defense = { currentDefense + 2 }
                }, 
                iconTexture: BuffHelper.GetIconTexture(), 
                iconSheetIndex: BuffHelper.GetBuffIndexIronBody())
        {
            
        }

    }
}