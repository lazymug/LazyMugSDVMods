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
            int currentDefense = 0)
            : base(
                id: BuffHelper.GetBuffDragonScaleId(), 
                displayName: ModEntry.Instance.GetTextTranslation("buff.dragon_scale.name"),
                description: ModEntry.Instance.GetTextTranslation("buff.dragon_scale.description"),  
                duration: ENDLESS, 
                effects: new BuffEffects()
                {
                    Defense = { 7.0f }
                }, 
                iconTexture: BuffHelper.GetIconTexture(), 
                iconSheetIndex: BuffHelper.GetBuffIndexDragonScale())
        {
            
        }
    }
}