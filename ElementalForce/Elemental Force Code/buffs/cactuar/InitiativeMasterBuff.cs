using ElementalForce.Elemental_Force_Code.buffEffects;
using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.cactuar
{
    public class InitiativeMasterBuff : Buff
    {
        private const int Level = 3;
        
        public InitiativeMasterBuff(
        ) : base(
            id: BuffHelper.GetBuffInitiativeMasterId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.initiative_master.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.initiative_master.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                Speed = { 3.0f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexInitiativeMaster())
        {
        }
    }
}