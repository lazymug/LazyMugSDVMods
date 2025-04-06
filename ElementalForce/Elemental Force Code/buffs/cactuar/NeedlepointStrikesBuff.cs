using ElementalForce.Elemental_Force_Code.buffEffects;
using ElementalForce.Elemental_Force_Code.helpers;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;

namespace ElementalForce.Elemental_Force_Code.buffs.cactuar
{
    public class NeedlepointStrikesBuff : Buff
    {
        private const int Level = 2;
        
        public NeedlepointStrikesBuff(
        ) : base(
            id: BuffHelper.GetBuffNeedlepointStrikesId(),
            displayName: ModEntry.Instance.GetTextTranslation("buff.needlepoint_strikes.name"),
            description: ModEntry.Instance.GetTextTranslation("buff.needlepoint_strikes.description"),  
            duration: ENDLESS, 
            effects: new BuffEffects()
            {
                WeaponSpeedMultiplier = { 1.3f },
                WeaponPrecisionMultiplier = { 0.6f }
            }, 
            iconTexture: BuffHelper.GetIconTexture(), 
            iconSheetIndex: BuffHelper.GetBuffIndexNeedlepointStrikes())
        {
        }
    }
}