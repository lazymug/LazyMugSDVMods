using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ValleyTriad.Rendering
{
    /// <summary>
    /// A hand-drawn 5×7 bitmap pixel font (digits, A–Z, '-', space, pt-BR accents).
    /// Drawn with filled blocks so it scales crisply with the rest of the pixel-art card.
    /// Falls back silently for glyphs it doesn't have (caller may use Game1.smallFont for CJK).
    /// </summary>
    public static class PixelFont
    {
        private static readonly Dictionary<char, string[]> F = new()
        {
            ['0']=new[]{"01110","10001","10011","10101","11001","10001","01110"},
            ['1']=new[]{"00100","01100","00100","00100","00100","00100","01110"},
            ['2']=new[]{"01110","10001","00001","00110","01000","10000","11111"},
            ['3']=new[]{"11111","00010","00100","00010","00001","10001","01110"},
            ['4']=new[]{"00010","00110","01010","10010","11111","00010","00010"},
            ['5']=new[]{"11111","10000","11110","00001","00001","10001","01110"},
            ['6']=new[]{"00110","01000","10000","11110","10001","10001","01110"},
            ['7']=new[]{"11111","00001","00010","00100","01000","01000","01000"},
            ['8']=new[]{"01110","10001","10001","01110","10001","10001","01110"},
            ['9']=new[]{"01110","10001","10001","01111","00001","00010","01100"},
            ['A']=new[]{"01110","10001","10001","11111","10001","10001","10001"},
            ['B']=new[]{"11110","10001","10001","11110","10001","10001","11110"},
            ['C']=new[]{"01110","10001","10000","10000","10000","10001","01110"},
            ['D']=new[]{"11100","10010","10001","10001","10001","10010","11100"},
            ['E']=new[]{"11111","10000","10000","11100","10000","10000","11111"},
            ['F']=new[]{"11111","10000","10000","11100","10000","10000","10000"},
            ['G']=new[]{"01110","10001","10000","10111","10001","10001","01110"},
            ['H']=new[]{"10001","10001","10001","11111","10001","10001","10001"},
            ['I']=new[]{"01110","00100","00100","00100","00100","00100","01110"},
            ['J']=new[]{"00111","00010","00010","00010","10010","10010","01100"},
            ['K']=new[]{"10001","10010","10100","11000","10100","10010","10001"},
            ['L']=new[]{"10000","10000","10000","10000","10000","10000","11111"},
            ['M']=new[]{"10001","11011","10101","10101","10001","10001","10001"},
            ['N']=new[]{"10001","10001","11001","10101","10011","10001","10001"},
            ['O']=new[]{"01110","10001","10001","10001","10001","10001","01110"},
            ['P']=new[]{"11110","10001","10001","11110","10000","10000","10000"},
            ['Q']=new[]{"01110","10001","10001","10001","10101","10010","01101"},
            ['R']=new[]{"11110","10001","10001","11110","10100","10010","10001"},
            ['S']=new[]{"01110","10001","10000","01110","00001","10001","01110"},
            ['T']=new[]{"11111","00100","00100","00100","00100","00100","00100"},
            ['U']=new[]{"10001","10001","10001","10001","10001","10001","01110"},
            ['V']=new[]{"10001","10001","10001","10001","10001","01010","00100"},
            ['W']=new[]{"10001","10001","10001","10101","10101","11011","10001"},
            ['X']=new[]{"10001","10001","01010","00100","01010","10001","10001"},
            ['Y']=new[]{"10001","10001","01010","00100","00100","00100","00100"},
            ['Z']=new[]{"11111","00001","00010","00100","01000","10000","11111"},
            ['-']=new[]{"00000","00000","00000","11111","00000","00000","00000"},
            [' ']=new[]{"00000","00000","00000","00000","00000","00000","00000"},
        };

        private static readonly Dictionary<string, string[]> ACC = new()
        {
            ["acute"]=new[]{"00010","00100"}, ["grave"]=new[]{"01000","00100"},
            ["til"]=new[]{"01101","10010"}, ["circ"]=new[]{"00100","01010"},
        };
        private static readonly Dictionary<char, (char, string)> ACCENTED = new()
        {
            ['Á']=('A',"acute"),['À']=('A',"grave"),['Ã']=('A',"til"),['Â']=('A',"circ"),
            ['É']=('E',"acute"),['Ê']=('E',"circ"),['Í']=('I',"acute"),
            ['Ó']=('O',"acute"),['Ô']=('O',"circ"),['Õ']=('O',"til"),['Ú']=('U',"acute"),['Ç']=('C',"ced"),
        };

        private const int GW = 6; // 5 glyph + 1 spacing

        public static int Width(string text) => text.Length * GW - 1;

        private static (string[] rows, string? acc) Glyph(char ch)
        {
            ch = char.ToUpperInvariant(ch);
            if (ACCENTED.TryGetValue(ch, out var a)) return (F.TryGetValue(a.Item1, out var r) ? r : F[' '], a.Item2);
            return (F.TryGetValue(ch, out var rr) ? rr : F[' '], null);
        }

        /// <summary>Draws <paramref name="text"/> centred at (cx,cy) with block size <paramref name="blk"/>.</summary>
        public static void DrawCentered(SpriteBatch b, Texture2D pixel, string text, int cx, int cy, int blk, Color fill, Color outline)
        {
            int total = Width(text) * blk;
            int x0 = cx - total / 2, y0 = cy - (7 * blk) / 2;
            for (int ci = 0; ci < text.Length; ci++)
            {
                var (rows, acc) = Glyph(text[ci]);
                int gx = x0 + ci * GW * blk;
                var pts = new List<(int c, int r)>();
                for (int r = 0; r < rows.Length; r++)
                    for (int c = 0; c < rows[r].Length; c++)
                        if (rows[r][c] == '1') pts.Add((c, r));
                if (acc == "ced") { pts.Add((1, 7)); pts.Add((1, 8)); }
                else if (acc != null && ACC.TryGetValue(acc, out var am))
                    for (int r = 0; r < am.Length; r++)
                        for (int c = 0; c < am[r].Length; c++)
                            if (am[r][c] == '1') pts.Add((c, r - 2));

                foreach (var (c, r) in pts) // 8-neighbour outline
                {
                    int X = gx + c * blk, Y = y0 + r * blk;
                    for (int oy = -1; oy <= 1; oy++)
                        for (int ox = -1; ox <= 1; ox++)
                            if (ox != 0 || oy != 0)
                                b.Draw(pixel, new Rectangle(X + ox * blk, Y + oy * blk, blk, blk), outline);
                }
                foreach (var (c, r) in pts)
                    b.Draw(pixel, new Rectangle(gx + c * blk, y0 + r * blk, blk, blk), fill);
            }
        }
    }
}
