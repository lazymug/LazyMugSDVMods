using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Menus;

namespace ElementalForce.Elemental_Force_Code.buffs
{
    // todo: reavaliar se removo ou não
    public abstract class CustomModBuff : Buff
    {
        /// <summary>
        /// Unique identifier for the custom buff.
        /// </summary>
        // public abstract string BuffId { get; }

        /// <summary>
        /// Constructor for the CustomModBuff class.
        /// </summary>
        /// <param name="buffId">Unique identifier for the buff.</param>
        /// <param name="displayName">The display name of the buff.</param>
        /// <param name="description">The description of the buff.</param>
        /// <param name="duration">Duration of the buff in game minutes.</param>
        /// <param name="source">The source of the buff (optional).</param>
        /// <param name="sourceDescription">Additional source description (optional).</param>
        protected CustomModBuff(string buffId, string displayName, string description, int duration, BuffEffects effects, Texture2D iconTexture, int iconSheetIndex)
            : base(id: buffId,  displayName: displayName, description: description, duration: duration, effects: effects, iconTexture: iconTexture, iconSheetIndex: iconSheetIndex)
        {
        }

        /// <summary>
        /// Abstract method for removing the buff from the BuffManager.
        /// </summary>
        /// <param name="buffs">The player's BuffManager instance.</param>
        public abstract void removeItself(BuffManager buffs);

        /// <summary>
        /// Adds the buff to the player's buff list.
        /// </summary>
        public void Apply(Farmer player)
        {
            player.applyBuff(this);
        }
    }
}