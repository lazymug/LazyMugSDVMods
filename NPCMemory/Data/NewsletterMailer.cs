using System;
using System.Collections.Generic;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewValley;

namespace NPCMemory.Data
{
    /// <summary>
    /// Registers the weekly newsletter with MailFrameworkMod. This class references
    /// MailFrameworkMod types directly, so it must only be instantiated when the
    /// mod is actually loaded (see ModEntry's soft-dependency guard).
    /// </summary>
    public class NewsletterMailer : INewsletterMailer
    {
        private readonly IManifest _manifest;

        public NewsletterMailer(IManifest manifest)
        {
            _manifest = manifest;
        }

        /// <summary>
        /// (Re)registers the newsletter letter. The letter is delivered by MailFrameworkMod
        /// on the next day the <paramref name="deliverWhen"/> predicate returns true.
        /// The letter id is namespaced per save and per week so each edition is delivered once.
        /// </summary>
        public void Register(string content, string idSuffix, Func<bool> deliverWhen)
        {
            string letterId = $"{Game1.uniqueIDForThisGame}.{_manifest.UniqueID}.newsletter.{idSuffix}";

            MailRepository.SaveLetter(new Letter(
                letterId,
                content,
                new List<Item>(),
                l => !Game1.player.mailReceived.Contains(l.Id)
                     && l.Id.Contains(Game1.uniqueIDForThisGame.ToString())
                     && deliverWhen(),
                l => Game1.player.mailReceived.Add(l.Id),
                whichBG: 0
            ));
        }
    }
}
