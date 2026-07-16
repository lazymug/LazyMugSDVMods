namespace NPCMemory
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public float DialogueChance { get; set; } = 0.3f;
        public bool ShowDebugMessages { get; set; } = false;

        // Weekly newsletter (requires MailFrameworkMod)
        public bool NewsletterEnabled { get; set; } = true;
        public int NewsletterCadenceDays { get; set; } = 7;
        public int NewsletterMaxHeadlines { get; set; } = 4;
    }
}
