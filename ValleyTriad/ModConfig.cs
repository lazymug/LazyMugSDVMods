namespace ValleyTriad
{
    /// <summary>Loss severity when you lose a match.</summary>
    public enum StakeMode
    {
        Friendly, // no loss
        Hard,     // lose a random card from the five you played
        Ragnarok  // opponent takes your most valuable played card
    }

    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        // Advanced rules (all on at launch)
        public bool RuleSame { get; set; } = true;
        public bool RulePlus { get; set; } = true;
        public bool RuleCombo { get; set; } = true;

        // Elemental (seasons) rule
        public bool RuleElemental { get; set; } = true;
        public int ElementalCells { get; set; } = 3; // random tiles per match that gain an element

        // Stakes
        public StakeMode Stakes { get; set; } = StakeMode.Friendly;

        // Sudden death cap before a true draw
        public int SuddenDeathRounds { get; set; } = 3;
    }
}
