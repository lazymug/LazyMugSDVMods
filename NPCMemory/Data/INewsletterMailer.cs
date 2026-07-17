using System;

namespace NPCMemory.Data
{
    internal interface INewsletterMailer
    {
        void Register(string content, string idSuffix, Func<bool> deliverWhen);
    }
}
