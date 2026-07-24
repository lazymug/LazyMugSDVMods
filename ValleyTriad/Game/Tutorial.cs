using System.Collections.Generic;
using Season = ValleyTriad.Models.Season;

namespace ValleyTriad.Game
{
    /// <summary>One step of the guided tutorial.</summary>
    public abstract class TutStep { }

    /// <summary>Abigail says a line (dialogue overlay); click to continue.</summary>
    public class TutSay : TutStep { public string Key = ""; }

    /// <summary>Guided player move: only this card on this cell is accepted (both highlighted).</summary>
    public class TutPlayerMove : TutStep { public string CardId = ""; public int R, C; public string HintKey = ""; }

    /// <summary>Scripted opponent move (no AI).</summary>
    public class TutOppMove : TutStep { public string CardId = ""; public int R, C; }

    public class TutorialScript
    {
        public List<TutStep> Steps = new();
        public List<(int r, int c, Season season)> ElementalCells = new();
    }

    /// <summary>
    /// The Level-2 guided tutorial vs Abigail. Both decks and every move are fixed, so the
    /// captures demonstrate exactly what each line teaches:
    ///  - basic capture (Pumpkin 6 &gt; Blueberry 4),
    ///  - the Spring elemental tile (+1 on Cauliflower),
    ///  - Same (Potato at top-right ties 2=2 and 3=3, flipping both).
    /// Final score is always player 8 × 2.
    /// </summary>
    public static class Tutorial
    {
        public static readonly string[] PlayerDeck = { "pumpkin", "cauliflower", "potato", "parsnip", "chicken" };
        public static readonly string[] OppDeck = { "blueberry", "salmonberry", "commonmushroom", "quartz", "greenbean" };

        public static TutorialScript BuildScript() => new()
        {
            ElementalCells = { (1, 0, Season.Spring) },
            Steps =
            {
                new TutSay { Key = "tut.intro1" },
                new TutSay { Key = "tut.intro2" },
                new TutSay { Key = "tut.opp1" },
                new TutOppMove { CardId = "blueberry", R = 1, C = 1 },
                new TutPlayerMove { CardId = "pumpkin", R = 2, C = 1, HintKey = "tut.hint.capture" },
                new TutSay { Key = "tut.captured" },
                new TutOppMove { CardId = "salmonberry", R = 0, C = 1 },
                new TutPlayerMove { CardId = "cauliflower", R = 1, C = 0, HintKey = "tut.hint.elemental" },
                new TutSay { Key = "tut.elemental" },
                new TutOppMove { CardId = "commonmushroom", R = 1, C = 2 },
                new TutPlayerMove { CardId = "potato", R = 0, C = 2, HintKey = "tut.hint.same" },
                new TutSay { Key = "tut.same" },
                new TutOppMove { CardId = "quartz", R = 0, C = 0 },
                new TutPlayerMove { CardId = "parsnip", R = 2, C = 0, HintKey = "tut.hint.block" },
                new TutOppMove { CardId = "greenbean", R = 2, C = 2 },
                new TutSay { Key = "tut.end" },
            },
        };
    }
}
