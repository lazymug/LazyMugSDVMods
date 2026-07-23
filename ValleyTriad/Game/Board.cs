using System.Collections.Generic;
using ValleyTriad.Models;

namespace ValleyTriad.Game
{
    public enum Owner { None, P1, P2 }

    public class Cell
    {
        public Card? Card;
        public Owner Owner = Owner.None;
        public Season Element = Season.None; // elemental tile (optional)
        public bool Empty => Card == null;
    }

    /// <summary>
    /// The 3×3 Triple Triad board and its capture rules: basic adjacency capture,
    /// plus optional Same / Plus / Combo, and the elemental (+1/−1) tile rule.
    /// Pure logic — no game/UI dependencies, so it's unit-testable.
    /// </summary>
    public class Board
    {
        public const int N = 3;
        public readonly Cell[,] Cells = new Cell[N, N];
        public bool RuleSame, RulePlus, RuleCombo, RuleElemental;

        public Board(bool same = true, bool plus = true, bool combo = true, bool elemental = true)
        {
            RuleSame = same; RulePlus = plus; RuleCombo = combo; RuleElemental = elemental;
            for (int r = 0; r < N; r++)
                for (int c = 0; c < N; c++)
                    Cells[r, c] = new Cell();
        }

        public bool InBounds(int r, int c) => r >= 0 && r < N && c >= 0 && c < N;

        /// <summary>Edge value of the card at (r,c) facing <paramref name="dir"/>, with the elemental bonus applied.</summary>
        public int EffectiveEdge(int r, int c, Dir dir)
        {
            Cell cell = Cells[r, c];
            int val = cell.Card!.Edge(dir);
            if (RuleElemental && cell.Element != Season.None)
                val += (cell.Card.Element == cell.Element) ? 1 : -1;
            return val;
        }

        /// <summary>
        /// Places a card and resolves captures. Returns the list of positions flipped to <paramref name="owner"/>.
        /// </summary>
        public List<(int r, int c)> Place(Card card, Owner owner, int r, int c)
        {
            Cells[r, c].Card = card;
            Cells[r, c].Owner = owner;

            var captured = new List<(int, int)>();
            var comboSeeds = new List<(int, int)>();

            // Collect adjacency data for the placed card.
            var adj = new List<(Dir dir, int nr, int nc, int mine, int theirs)>();
            foreach (Dir dir in new[] { Dir.N, Dir.E, Dir.S, Dir.W })
            {
                var (dr, dc) = dir.Delta();
                int nr = r + dr, nc = c + dc;
                if (!InBounds(nr, nc) || Cells[nr, nc].Empty) continue;
                int mine = EffectiveEdge(r, c, dir);
                int theirs = EffectiveEdge(nr, nc, dir.Opposite());
                adj.Add((dir, nr, nc, mine, theirs));
            }

            // Same rule: 2+ neighbours where facing edges are equal -> capture the enemy ones (combo-eligible).
            if (RuleSame)
            {
                var same = adj.FindAll(a => a.mine == a.theirs);
                if (same.Count >= 2)
                    foreach (var a in same)
                        if (TryCapture(a.nr, a.nc, owner, captured)) comboSeeds.Add((a.nr, a.nc));
            }

            // Plus rule: 2+ neighbours sharing the same (mine+theirs) sum -> capture the enemy ones (combo-eligible).
            if (RulePlus)
            {
                var bySum = new Dictionary<int, List<(int nr, int nc)>>();
                foreach (var a in adj)
                {
                    int sum = a.mine + a.theirs;
                    if (!bySum.TryGetValue(sum, out var list)) bySum[sum] = list = new();
                    list.Add((a.nr, a.nc));
                }
                foreach (var kv in bySum)
                    if (kv.Value.Count >= 2)
                        foreach (var (nr, nc) in kv.Value)
                            if (TryCapture(nr, nc, owner, captured)) comboSeeds.Add((nr, nc));
            }

            // Basic capture: our facing edge strictly greater than theirs.
            foreach (var a in adj)
                if (a.mine > a.theirs)
                    TryCapture(a.nr, a.nc, owner, captured);

            // Combo: cards flipped by Same/Plus run their own basic captures, cascading.
            if (RuleCombo)
            {
                var queue = new Queue<(int r, int c)>(comboSeeds);
                while (queue.Count > 0)
                {
                    var (cr, cc) = queue.Dequeue();
                    foreach (Dir dir in new[] { Dir.N, Dir.E, Dir.S, Dir.W })
                    {
                        var (dr, dc) = dir.Delta();
                        int nr = cr + dr, nc = cc + dc;
                        if (!InBounds(nr, nc) || Cells[nr, nc].Empty) continue;
                        int mine = EffectiveEdge(cr, cc, dir);
                        int theirs = EffectiveEdge(nr, nc, dir.Opposite());
                        if (mine > theirs && TryCapture(nr, nc, owner, captured))
                            queue.Enqueue((nr, nc));
                    }
                }
            }

            return captured;
        }

        private bool TryCapture(int r, int c, Owner owner, List<(int, int)> captured)
        {
            Cell cell = Cells[r, c];
            if (cell.Empty || cell.Owner == owner || cell.Owner == Owner.None) return false;
            cell.Owner = owner;
            captured.Add((r, c));
            return true;
        }

        /// <summary>Card count for an owner (board only). The winner is decided by board + held card.</summary>
        public int Count(Owner owner)
        {
            int n = 0;
            foreach (var cell in Cells)
                if (!cell.Empty && cell.Owner == owner) n++;
            return n;
        }

        public bool IsFull()
        {
            foreach (var cell in Cells) if (cell.Empty) return false;
            return true;
        }
    }
}
