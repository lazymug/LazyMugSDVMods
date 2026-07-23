using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using ValleyTriad.Models;
using Season = ValleyTriad.Models.Season;

namespace ValleyTriad.Rendering
{
    /// <summary>
    /// Composes a pixel-art card texture at runtime (chassis + per-category scene + hero
    /// sprite + coins/name in the pixel font) and caches it. Nothing is shipped as an image;
    /// the hero comes from the player's own game content. Cache is dropped on locale change.
    /// </summary>
    public class CardRenderer : IDisposable
    {
        private const int LW = 92, LH = 128, S = 4;      // logical size and device scale
        private const int DEVW = LW * S, DEVH = LH * S;

        private static readonly Color DARK = new(53, 34, 18), WOOD = new(139, 94, 52),
            WOODHI = new(178, 131, 80), WOODLO = new(101, 66, 36);
        private static readonly Dictionary<Tier, Color> RAR = new()
        {
            [Tier.Common] = new(111, 161, 74), [Tier.Uncommon] = new(74, 143, 192),
            [Tier.Rare] = new(196, 122, 46), [Tier.Legendary] = new(217, 181, 58),
        };
        private static readonly Dictionary<Season, (Color col, char letter)> SEASON = new()
        {
            [Season.Spring] = (new(79, 170, 69), 'P'), [Season.Summer] = (new(224, 168, 40), 'V'),
            [Season.Fall] = (new(210, 120, 50), 'O'), [Season.Winter] = (new(90, 165, 205), 'I'),
        };

        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private readonly Dictionary<string, Texture2D> _cache = new();
        private Texture2D? _pixel, _circle;
        private string _locale = "";

        public CardRenderer(IModHelper helper, IMonitor monitor) { _helper = helper; _monitor = monitor; }

        private Texture2D Pixel => _pixel ??= MakePixel();
        private Texture2D Circle => _circle ??= MakeCircle();

        private Texture2D MakePixel()
        {
            var t = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1);
            t.SetData(new[] { Color.White });
            return t;
        }
        private Texture2D MakeCircle()
        {
            const int D = 64; var data = new Color[D * D]; float c = (D - 1) / 2f;
            for (int y = 0; y < D; y++)
                for (int x = 0; x < D; x++)
                {
                    float dist = (float)Math.Sqrt((x - c) * (x - c) + (y - c) * (y - c)) / c;
                    float a = Math.Max(0, 1 - dist);
                    data[y * D + x] = new Color(1f, 1f, 1f, a);
                }
            var t = new Texture2D(Game1.graphics.GraphicsDevice, D, D); t.SetData(data); return t;
        }

        /// <summary>Composes the cards up-front (call outside an active SpriteBatch, e.g. on menu open).</summary>
        public void Prewarm(IEnumerable<Card> cards) { foreach (var c in cards) Get(c); }

        public Texture2D Get(Card card)
        {
            string loc = LocalizedContentManager.CurrentLanguageCode.ToString();
            if (loc != _locale) { Clear(); _locale = loc; }
            if (_cache.TryGetValue(card.Id, out var tex)) return tex;
            tex = Compose(card);
            _cache[card.Id] = tex;
            return tex;
        }

        private string ResolveName(Card card)
        {
            if (card.Sprite.StartsWith("(O)"))
            {
                ParsedItemData? d = ItemRegistry.GetData(card.Sprite);
                if (d != null) return d.DisplayName;
            }
            return _helper.Translation.Get(card.NameKey).ToString();
        }

        private (Texture2D tex, Rectangle src)? ResolveHero(Card card)
        {
            try
            {
                if (card.Sprite.StartsWith("(O)"))
                {
                    var d = ItemRegistry.GetData(card.Sprite);
                    if (d != null) return (d.GetTexture(), d.GetSourceRect());
                }
                else if (card.Sprite.StartsWith("Villager:"))
                {
                    string n = card.Sprite["Villager:".Length..];
                    return (Game1.content.Load<Texture2D>("Portraits/" + n), new Rectangle(0, 0, 64, 64));
                }
                else if (card.Sprite.StartsWith("Monster:"))
                {
                    string n = card.Sprite["Monster:".Length..];
                    return (Game1.content.Load<Texture2D>("Characters/Monsters/" + n), new Rectangle(0, 0, 16, 24));
                }
                else if (card.Sprite.StartsWith("Animal:"))
                {
                    string n = card.Sprite["Animal:".Length..];
                    return (Game1.content.Load<Texture2D>("Animals/" + n), new Rectangle(0, 0, 16, 16));
                }
            }
            catch (Exception e) { _monitor.LogOnce($"No sprite for {card.Id} ({card.Sprite}): {e.Message}", LogLevel.Trace); }
            return null;
        }

        private Texture2D Compose(Card card)
        {
            var gd = Game1.graphics.GraphicsDevice;
            var rt = new RenderTarget2D(gd, DEVW, DEVH);
            var prev = gd.GetRenderTargets();
            gd.SetRenderTarget(rt);
            gd.Clear(Color.Transparent);
            var b = new SpriteBatch(gd);
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            DrawChassis(b, card);
            DrawScene(b, card, ResolveHero(card));
            DrawCoinsAndText(b, card);

            b.End();
            b.Dispose();
            gd.SetRenderTarget(null);
            if (prev.Length > 0) gd.SetRenderTargets(prev);

            var tex = new Texture2D(gd, DEVW, DEVH);
            var buf = new Color[DEVW * DEVH];
            rt.GetData(buf); tex.SetData(buf); rt.Dispose();
            return tex;
        }

        // ---- helpers (logical coords ×S) ----
        private void Fill(SpriteBatch b, int lx, int ly, int lw, int lh, Color c)
            => b.Draw(Pixel, new Rectangle(lx * S, ly * S, lw * S, lh * S), c);
        private static Color Lerp(Color a, Color c, float t) => new(
            (int)(a.R + (c.R - a.R) * t), (int)(a.G + (c.G - a.G) * t), (int)(a.B + (c.B - a.B) * t));
        private void VGrad(SpriteBatch b, int lx, int ly, int lw, int lh, Color c0, Color c1)
        {
            for (int i = 0; i < lh; i++) Fill(b, lx, ly + i, lw, 1, Lerp(c0, c1, (float)i / Math.Max(1, lh - 1)));
        }
        private void Blob(SpriteBatch b, float lcx, float lcy, float lr, Color c, float a = 1f)
        {
            int d = (int)(lr * 2 * S);
            b.Draw(Circle, new Rectangle((int)(lcx * S - d / 2f), (int)(lcy * S - d / 2f), d, d), c * a);
        }

        private void DrawChassis(SpriteBatch b, Card card)
        {
            Color inlay = RAR[card.Tier];
            Fill(b, 0, 0, LW, LH, DARK);
            Fill(b, 2, 2, LW - 4, LH - 4, WOOD);
            Fill(b, 2, 2, LW - 4, 1, WOODHI);
            Fill(b, 2, LH - 3, LW - 4, 1, WOODLO);
            // inlay border
            Fill(b, 4, 4, LW - 8, 1, inlay); Fill(b, 4, LH - 5, LW - 8, 1, inlay);
            Fill(b, 4, 4, 1, LH - 8, inlay); Fill(b, LW - 5, 4, 1, LH - 8, inlay);
            // studs
            foreach (var (x, y) in new[] { (5, 5), (LW - 6, 5), (5, LH - 6), (LW - 6, LH - 6) })
                Fill(b, x - 1, y - 1, 2, 2, inlay);
            // art-window border
            Fill(b, 7, 11, 78, 75, DARK);
            // name banner
            Fill(b, 6, 92, LW - 13, 14, WOODLO);
            Fill(b, 7, 93, LW - 15, 1, WOOD);
            // rarity gems
            int ng = card.Tier switch { Tier.Common => 1, Tier.Uncommon => 2, Tier.Rare => 3, _ => 4 };
            for (int i = 0; i < 4; i++)
                Fill(b, LW / 2 - 8 + i * 5, 112, 3, 3, i < ng ? inlay : new Color(90, 66, 38));
        }

        // window logical area
        private const int WX = 8, WY = 12, WW = 76, WH = 73;

        private void DrawScene(SpriteBatch b, Card card, (Texture2D tex, Rectangle src)? hero)
        {
            string kind = card.Category switch
            {
                Category.Animal => "pasture",
                Category.Fish => "sea",
                Category.Monster or Category.Mineral => "mine",
                Category.Villager => "saloon",
                Category.Special => "night",
                _ => "field",
            };
            switch (kind)
            {
                case "field": BgField(b); break;
                case "pasture": BgPasture(b); break;
                case "mine": BgMine(b); break;
                case "sea": BgSea(b); break;
                case "saloon": BgSaloon(b); break;
                case "night": BgNight(b); break;
            }
            // vignette
            for (int i = 0; i < 3; i++)
            {
                Color v = new Color(18, 20, 26) * (0.16f - i * 0.05f);
                Fill(b, WX + i, WY + i, WW - 2 * i, 1, v); Fill(b, WX + i, WY + WH - 1 - i, WW - 2 * i, 1, v);
                Fill(b, WX + i, WY + i, 1, WH - 2 * i, v); Fill(b, WX + WW - 1 - i, WY + i, 1, WH - 2 * i, v);
            }
            // hero highlight
            float hcx = WX + WW / 2f, hcy = WY + WH * (kind is "saloon" or "night" ? 0.60f : 0.66f);
            Blob(b, hcx, hcy, 12, new Color(255, 246, 206), 0.5f);
            Blob(b, hcx, hcy + 8, 10, Color.Black, 0.35f); // shadow
            if (hero != null) DrawHero(b, hero.Value.tex, hero.Value.src, hcx, hcy);
        }

        private void DrawHero(SpriteBatch b, Texture2D tex, Rectangle src, float lcx, float lcy)
        {
            int size = 30; // logical
            var dest = new Rectangle((int)(lcx * S - size * S / 2f), (int)(lcy * S - size * S / 2f - 2 * S), size * S, size * S);
            for (int oy = -1; oy <= 1; oy++) // dark outline
                for (int ox = -1; ox <= 1; ox++)
                    if (ox != 0 || oy != 0)
                        b.Draw(tex, new Rectangle(dest.X + ox * S, dest.Y + oy * S, dest.Width, dest.Height), src, Color.Black);
            b.Draw(tex, dest, src, Color.White);
        }

        private void BgField(SpriteBatch b)
        {
            int hz = WY + (int)(WH * 0.40f);
            VGrad(b, WX, WY, WW, hz - WY, new Color(250, 226, 168), new Color(150, 196, 224));
            Fill(b, WX, hz - 2, WW, 3, new Color(84, 124, 60));
            VGrad(b, WX, hz, WW, WY + WH - hz, new Color(120, 88, 56), new Color(80, 58, 36));
            var haze = new Color(150, 190, 178);
            (float y, float r)[] rows = { (hz + 3, 1.4f), (hz + 8, 2.0f), (hz + 15, 2.8f), (hz + 25, 3.8f), (hz + 40, 5.0f) };
            for (int i = 0; i < rows.Length; i++)
            {
                float t = (i + 1f) / rows.Length; var mid = Lerp(new Color(74, 134, 54), haze, (1 - t) * 0.45f);
                float step = Math.Max(2.4f, rows[i].r * 1.3f);
                for (float x = WX - rows[i].r; x < WX + WW + rows[i].r; x += step)
                    Blob(b, x, rows[i].y, rows[i].r, mid);
            }
        }
        private void BgPasture(SpriteBatch b)
        {
            int hz = WY + (int)(WH * 0.42f);
            VGrad(b, WX, WY, WW, hz - WY, new Color(150, 200, 232), new Color(206, 228, 236));
            VGrad(b, WX, hz, WW, WY + WH - hz, new Color(120, 176, 84), new Color(86, 140, 60));
            for (float x = WX; x < WX + WW; x += 12) Blob(b, x, hz + 2, 5, new Color(104, 158, 72));
            int fy = WY + (int)(WH * 0.6f);
            Fill(b, WX, fy, WW, 1, new Color(120, 86, 52)); Fill(b, WX, fy + 4, WW, 1, new Color(120, 86, 52));
            for (int px = WX + 3; px < WX + WW; px += 14) Fill(b, px, fy - 3, 1, 9, new Color(96, 66, 38));
        }
        private void BgMine(SpriteBatch b)
        {
            VGrad(b, WX, WY, WW, WH, new Color(64, 54, 60), new Color(34, 28, 34));
            var rng = new Random(7);
            for (int i = 0; i < 30; i++) Blob(b, WX + rng.Next(WW), WY + rng.Next(WH), rng.Next(1, 3),
                rng.Next(2) == 0 ? new Color(52, 44, 50) : new Color(80, 66, 74));
            Fill(b, WX, WY + WH - 5, WW, 5, new Color(46, 38, 42));
            Color[] gems = { new(160, 100, 210), new(90, 200, 150), new(90, 150, 220), new(210, 120, 90) };
            var rng2 = new Random(3);
            for (int i = 0; i < 16; i++)
            {
                float t = (float)rng2.NextDouble(); int yy = WY + (int)(WH * 0.45f + t * WH * 0.45f);
                Blob(b, WX + 3 + rng2.Next(WW - 6), yy, 1.2f + t * 2.2f, gems[rng2.Next(gems.Length)]);
            }
            Blob(b, WX + 6, WY + 6, 12, new Color(255, 180, 90), 0.4f); // torch glow
        }
        private void BgSea(SpriteBatch b)
        {
            int hz = WY + (int)(WH * 0.46f);
            VGrad(b, WX, WY, WW, hz - WY, new Color(250, 214, 150), new Color(160, 200, 224));
            Blob(b, WX + WW * 0.62f, hz - 3, 4, new Color(255, 238, 180));
            VGrad(b, WX, hz, WW, WY + WH - hz, new Color(84, 150, 196), new Color(28, 74, 132));
            var rng = new Random(5);
            for (int i = 0; i < 26; i++) { int x = WX + rng.Next(WW), y = hz + 2 + rng.Next(Math.Max(1, WY + WH - hz - 3)); Fill(b, x, y, rng.Next(2, 5), 1, new Color(170, 206, 224)); }
        }
        private void BgSaloon(SpriteBatch b)
        {
            for (int y = 0; y < WH; y++) Fill(b, WX, WY + y, WW, 1, (y / 4) % 2 == 0 ? new Color(146, 100, 58) : new Color(134, 90, 50));
            int shelf = WY + (int)(WH * 0.22f);
            Fill(b, WX, shelf, WW, 2, new Color(92, 60, 32));
            var rng = new Random(9);
            for (int bx = WX + 3; bx < WX + WW - 3; bx += 6)
            {
                Color[] bc = { new(70, 150, 90), new(200, 150, 60), new(160, 60, 60), new(80, 120, 180) };
                Fill(b, bx, shelf - 7, 3, 7, bc[rng.Next(bc.Length)]);
            }
            int counter = WY + (int)(WH * 0.66f);
            Fill(b, WX, counter, WW, WY + WH - counter, new Color(104, 68, 36));
            Fill(b, WX, counter, WW, 1, new Color(150, 104, 60));
            Blob(b, WX + WW / 2f, WY + 6, 14, new Color(255, 214, 140), 0.35f);
        }
        private void BgNight(SpriteBatch b)
        {
            VGrad(b, WX, WY, WW, WH, new Color(24, 26, 64), new Color(64, 52, 96));
            var rng = new Random(11);
            for (int i = 0; i < 36; i++) Fill(b, WX + rng.Next(WW), WY + rng.Next((int)(WH * 0.6f)), 1, 1, new Color(240, 240, 220));
            Blob(b, WX + WW * 0.72f, WY + WH * 0.2f, 6, new Color(238, 236, 208));
            Fill(b, WX, WY + (int)(WH * 0.7f), WW, WH - (int)(WH * 0.7f), new Color(30, 40, 52));
        }

        private void DrawCoinsAndText(SpriteBatch b, Card card)
        {
            int ax0 = 7, ay0 = 11, ax1 = LW - 8, ay1 = 86;
            var edges = new (int lx, int ly, Dir d)[]
            {
                ((ax0 + ax1) / 2, ay0, Dir.N), ((ax0 + ax1) / 2, ay1, Dir.S),
                (ax0, (ay0 + ay1) / 2, Dir.W), (ax1, (ay0 + ay1) / 2, Dir.E),
            };
            Color inlay = RAR[card.Tier];
            foreach (var (lx, ly, d) in edges)
            {
                Blob(b, lx, ly, 8, DARK);
                Blob(b, lx, ly, 7, new Color(198, 150, 78));
                Blob(b, lx, ly, 5.5f, new Color(240, 211, 150));
                int v = card.Edge(d);
                PixelFont.DrawCentered(b, Pixel, v == 10 ? "A" : v.ToString(), lx * S, ly * S, S, new Color(44, 28, 14), new Color(245, 224, 170));
            }
            // name (auto-fit block size)
            string name = ResolveName(card).ToUpperInvariant();
            int bannerDev = (LW - 15) * S;
            int blk = Math.Clamp(bannerDev / Math.Max(1, PixelFont.Width(name)), 1, 3);
            PixelFont.DrawCentered(b, Pixel, name, LW / 2 * S, 99 * S, blk, new Color(245, 233, 205), new Color(44, 28, 14));
            // season badge
            if (SEASON.TryGetValue(card.Element, out var se))
            {
                int bx = (LW - 11), by = 15;
                Blob(b, bx, by, 6.5f, new Color(245, 233, 205));
                Blob(b, bx, by, 5.5f, se.col);
                PixelFont.DrawCentered(b, Pixel, se.letter.ToString(), bx * S, by * S, 2, Color.White, new Color(30, 40, 20));
            }
        }

        public void Clear()
        {
            foreach (var t in _cache.Values) t.Dispose();
            _cache.Clear();
        }
        public void Dispose()
        {
            Clear(); _pixel?.Dispose(); _circle?.Dispose();
        }
    }
}
