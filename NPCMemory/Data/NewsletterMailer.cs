using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StardewModdingAPI;
using StardewValley;

namespace NPCMemory.Data
{
    /// <summary>
    /// Registers the weekly newsletter with MailFrameworkMod.
    ///
    /// This class talks to MailFrameworkMod entirely through reflection and never names a
    /// MailFrameworkMod type in its IL. That matters: SMAPI's assembly rewriter (Mono.Cecil)
    /// walks every type reference in the mod at load time and fails the whole mod if it can't
    /// resolve them. A direct reference to MailFrameworkMod.Letter would make NPC Memory
    /// impossible to load whenever MailFrameworkMod isn't installed — even inside an
    /// unreachable branch. Reflection keeps the reference out of the IL, so the mod loads
    /// cleanly either way and this class is only ever constructed when MFM is present.
    /// </summary>
    public class NewsletterMailer
    {
        private readonly IManifest _manifest;
        private readonly ConstructorInfo _letterCtor;
        private readonly MethodInfo _saveLetter;
        private readonly MethodInfo _wrapFunc;
        private readonly MethodInfo _wrapAction;

        public NewsletterMailer(IManifest manifest, Assembly mfmAssembly)
        {
            _manifest = manifest;

            Type letterType = mfmAssembly.GetType("MailFrameworkMod.Letter", throwOnError: true)!;
            Type repoType = mfmAssembly.GetType("MailFrameworkMod.MailRepository", throwOnError: true)!;

            Type funcType = typeof(Func<,>).MakeGenericType(letterType, typeof(bool));
            Type actionType = typeof(Action<>).MakeGenericType(letterType);

            // Letter(string id, string text, List<Item> items, Func<Letter,bool> condition, Action<Letter> callback, int whichBG)
            _letterCtor = letterType.GetConstructor(new[]
            {
                typeof(string), typeof(string), typeof(List<Item>),
                funcType, actionType, typeof(int)
            }) ?? throw new MissingMethodException("MailFrameworkMod.Letter", "expected (string, string, List<Item>, Func<Letter,bool>, Action<Letter>, int) constructor");

            _saveLetter = repoType.GetMethod("SaveLetter", BindingFlags.Public | BindingFlags.Static)
                ?? throw new MissingMethodException("MailFrameworkMod.MailRepository", "SaveLetter");

            // Build strongly-typed delegates whose parameter is MailFrameworkMod.Letter (which we
            // can't name in source) by closing these generic helpers over that type at runtime.
            _wrapFunc = typeof(NewsletterMailer)
                .GetMethod(nameof(WrapFunc), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(letterType);
            _wrapAction = typeof(NewsletterMailer)
                .GetMethod(nameof(WrapAction), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(letterType);
        }

        // The newsletter's delivery/callback logic never inspects the Letter argument (we already
        // know the id we assigned it), so these wrappers just discard it.
        private static Func<T, bool> WrapFunc<T>(Func<bool> inner) => _ => inner();
        private static Action<T> WrapAction<T>(Action inner) => _ => inner();

        /// <summary>
        /// (Re)registers the newsletter letter. The letter is delivered by MailFrameworkMod
        /// on the next day the <paramref name="deliverWhen"/> predicate returns true.
        /// The letter id is namespaced per save and per week so each edition is delivered once.
        /// </summary>
        public void Register(string content, string idSuffix, Func<bool> deliverWhen)
        {
            string letterId = $"{Game1.uniqueIDForThisGame}.{_manifest.UniqueID}.newsletter.{idSuffix}";

            Func<bool> condition = () =>
                !Game1.player.mailReceived.Contains(letterId) && deliverWhen();
            Action onReceived = () => Game1.player.mailReceived.Add(letterId);

            object conditionDelegate = _wrapFunc.Invoke(null, new object[] { condition })!;
            object callbackDelegate = _wrapAction.Invoke(null, new object[] { onReceived })!;

            object letter = _letterCtor.Invoke(new object?[]
            {
                letterId, content, new List<Item>(),
                conditionDelegate, callbackDelegate, 0
            });

            _saveLetter.Invoke(null, new[] { letter });
        }
    }
}
