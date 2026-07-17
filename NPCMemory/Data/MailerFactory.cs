using System.Runtime.CompilerServices;
using StardewModdingAPI;

namespace NPCMemory.Data
{
    // Isolated factory so that Entry() never has NewsletterMailer (or MFM types) in its JIT scope.
    // NoInlining ensures this method body is compiled only when actually called — i.e. only when MFM is present.
    internal static class MailerFactory
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static INewsletterMailer Create(IManifest manifest) => new NewsletterMailer(manifest);
    }
}
