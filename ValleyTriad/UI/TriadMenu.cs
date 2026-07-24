using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using ValleyTriad.Game;
using ValleyTriad.Models;
using ValleyTriad.Rendering;
using Season = ValleyTriad.Models.Season;

namespace ValleyTriad.UI
{
    public enum Outcome { Win, Loss, Draw }

    public class MatchResult
    {
        public Outcome Outcome;
        public string? GainedCardId;
        public string? LostCardId;
    }

    public class MatchSettings
    {
        public List<Card> PlayerDeck = new();
        public List<Card> OppDeck = new();
        public StakeMode Stakes = StakeMode.Friendly;
        public int SuddenDeathRounds = 3;
        public int AiSkill = 2;
        public bool RuleSame = true, RulePlus = true, RuleCombo = true, RuleElemental = true;
        public int ElementalCells = 3;
        public string OpponentDisplay = "";
        public Action<MatchResult>? OnComplete;
        /// <summary>When set, the match runs as the guided tutorial (scripted moves, no stakes).</summary>
        public TutorialScript? Tutorial;
        /// <summary>Translation lookup (i18n key -> localized text).</summary>
        public Func<string, string>? T;
    }

    /// <summary>Playable Triple Triad match with a clean banded layout (opponent hand · board · your hand · status).</summary>
    public class TriadMenu : IClickableMenu
    {
        // layout
        private const int Cell = 100, Gap = 8, HandH = 106, BandGap = 14, TopPad = 46, StatusH = 44;
        private const float CardAspect = 92f / 128f;
        private static readonly int HandW = (int)(HandH * CardAspect);
        private enum State { Playing, PickReward, Done }

        private readonly CardRenderer _renderer;
        private readonly MatchSettings _s;

        private Board _board = null!;
        private List<Card> _playerHand = null!, _oppHand = null!;
        private readonly List<Card> _p1Played = new(), _p2Played = new();
        private Owner _turn = Owner.P1;
        private int _selected = -1, _round = 0;
        private State _state = State.Playing;
        private string? _statusOverride;
        private float _oppTimer;
        private readonly Dictionary<(int, int), float> _flash = new();
        private MatchResult? _result;
        private int _boardX, _boardY, _oppHandY, _playerHandY, _statusY;

        // tutorial state
        private int _tutIx;
        private float _tutOppTimer;
        private Texture2D? _abbyPortrait;
        private TutStep? TutCurrent =>
            _s.Tutorial != null && _tutIx < _s.Tutorial.Steps.Count ? _s.Tutorial.Steps[_tutIx] : null;
        private string Tr(string key) => _s.T?.Invoke(key) ?? key;

        private static readonly Color P1Tint = new(90, 150, 214), P2Tint = new(206, 96, 80);

        public TriadMenu(CardRenderer renderer, MatchSettings settings)
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            _renderer = renderer;
            _s = settings;
            _renderer.Prewarm(_s.PlayerDeck.Concat(_s.OppDeck));

            int boardPx = 3 * Cell + 2 * Gap;
            int handRowW = Deck.Size * (HandW + 10) - 10;
            int contentW = Math.Max(boardPx, handRowW);
            width = contentW + borderWidth * 2 + 64;
            height = borderWidth * 2 + TopPad + HandH + BandGap + boardPx + BandGap + HandH + StatusH + 16;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;

            _boardX = xPositionOnScreen + (width - boardPx) / 2;
            _oppHandY = yPositionOnScreen + borderWidth + TopPad;
            _boardY = _oppHandY + HandH + BandGap;
            _playerHandY = _boardY + boardPx + BandGap;
            _statusY = _playerHandY + HandH + 8;

            StartRound(new List<Card>(_s.PlayerDeck), new List<Card>(_s.OppDeck));
        }

        private void StartRound(List<Card> playerHand, List<Card> oppHand)
        {
            _board = new Board(_s.RuleSame, _s.RulePlus, _s.RuleCombo, _s.RuleElemental);
            if (_s.Tutorial != null)
                foreach (var (r, c, season) in _s.Tutorial.ElementalCells)
                    _board.Cells[r, c].Element = season;
            else if (_s.RuleElemental && _s.ElementalCells > 0) AssignElementalCells(_s.ElementalCells);
            _playerHand = playerHand;
            _oppHand = oppHand;
            _p1Played.Clear(); _p2Played.Clear();
            _turn = Owner.P1; _selected = -1; _flash.Clear();
        }

        private void AssignElementalCells(int count)
        {
            var seasons = new[] { Season.Spring, Season.Summer, Season.Fall, Season.Winter };
            var rng = Game1.random;
            var spots = new List<(int, int)>();
            for (int r = 0; r < 3; r++) for (int c = 0; c < 3; c++) spots.Add((r, c));
            for (int i = 0; i < count && spots.Count > 0; i++)
            {
                int k = rng.Next(spots.Count); var (r, c) = spots[k]; spots.RemoveAt(k);
                _board.Cells[r, c].Element = seasons[rng.Next(seasons.Length)];
            }
        }

        // ---- input ----
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y)) { Close(); return; }
            if (_state == State.Done) { Close(); return; }

            if (_state == State.PickReward)
            {
                var five = OppFive();
                for (int i = 0; i < five.Count; i++)
                    if (RewardRect(i, five.Count).Contains(x, y))
                    {
                        _result!.GainedCardId = five[i].Id;
                        Game1.playSound("coin");
                        _state = State.Done;
                        _s.OnComplete?.Invoke(_result);
                        return;
                    }
                return;
            }

            if (_s.Tutorial != null) { TutorialClick(x, y); return; }

            if (_turn != Owner.P1 || _oppTimer > 0) return;

            for (int i = 0; i < _playerHand.Count; i++)
                if (HandRect(i, true).Contains(x, y)) { _selected = i; Game1.playSound("smallSelect"); return; }

            if (_selected < 0 || _selected >= _playerHand.Count) return;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (CellRect(r, c).Contains(x, y) && _board.Cells[r, c].Empty)
                    {
                        var card = _playerHand[_selected];
                        var caps = _board.Place(card, Owner.P1, r, c);
                        _p1Played.Add(card);
                        _playerHand.RemoveAt(_selected);
                        _selected = -1;
                        FlashCaptures(caps);
                        Game1.playSound("bigSelect");
                        if (_board.IsFull()) BeginFinish();
                        else { _turn = Owner.P2; _oppTimer = 0.6f; }
                        return;
                    }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            float dt = (float)time.ElapsedGameTime.TotalSeconds;
            foreach (var k in _flash.Keys.ToList())
            {
                _flash[k] -= dt * 2.4f;
                if (_flash[k] <= 0) _flash.Remove(k);
            }
            if (_s.Tutorial != null)
            {
                if (_state == State.Playing) TutorialUpdate(dt);
                return;
            }

            if (_oppTimer > 0)
            {
                _oppTimer -= dt;
                if (_oppTimer <= 0 && _turn == Owner.P2 && !_board.IsFull())
                {
                    OpponentMove();
                    if (_board.IsFull()) BeginFinish();
                    else _turn = Owner.P1;
                }
            }
        }

        private void FlashCaptures(List<(int r, int c)> caps) { foreach (var p in caps) _flash[p] = 1f; }

        private void OpponentMove()
        {
            if (_oppHand.Count == 0) return;
            var rng = Game1.random;
            bool loose = _s.AiSkill == 0 ? rng.NextDouble() < 0.4 : _s.AiSkill == 1 && rng.NextDouble() < 0.15;

            int bi = 0, br = 0, bc = 0, best = -1;
            var empties = new List<(int r, int c)>();
            for (int r = 0; r < 3; r++) for (int c = 0; c < 3; c++) if (_board.Cells[r, c].Empty) empties.Add((r, c));

            if (loose)
            {
                bi = rng.Next(_oppHand.Count);
                var (rr, cc) = empties[rng.Next(empties.Count)]; br = rr; bc = cc;
            }
            else
            {
                for (int i = 0; i < _oppHand.Count; i++)
                    foreach (var (r, c) in empties)
                    {
                        int sc = _board.EvaluatePlacement(_oppHand[i], Owner.P2, r, c);
                        if (sc > best) { best = sc; bi = i; br = r; bc = c; }
                    }
            }
            var card = _oppHand[bi];
            var caps = _board.Place(card, Owner.P2, br, bc);
            _p2Played.Add(card);
            _oppHand.RemoveAt(bi);
            FlashCaptures(caps);
            Game1.playSound("bigSelect");
        }

        // ---- tutorial ----
        private void TutorialClick(int x, int y)
        {
            switch (TutCurrent)
            {
                case TutSay:
                    _tutIx++;
                    Game1.playSound("smallSelect");
                    TutMaybeFinish();
                    return;

                case TutPlayerMove pm:
                    // selecting a hand card: only the required one is accepted
                    for (int i = 0; i < _playerHand.Count; i++)
                        if (HandRect(i, true).Contains(x, y))
                        {
                            if (_playerHand[i].Id == pm.CardId) { _selected = i; Game1.playSound("smallSelect"); }
                            else Game1.playSound("cancel");
                            return;
                        }
                    // placing: only the required cell is accepted
                    if (_selected >= 0 && _selected < _playerHand.Count && _playerHand[_selected].Id == pm.CardId
                        && CellRect(pm.R, pm.C).Contains(x, y) && _board.Cells[pm.R, pm.C].Empty)
                    {
                        var card = _playerHand[_selected];
                        var caps = _board.Place(card, Owner.P1, pm.R, pm.C);
                        _p1Played.Add(card);
                        _playerHand.RemoveAt(_selected);
                        _selected = -1;
                        FlashCaptures(caps);
                        Game1.playSound("bigSelect");
                        _tutIx++;
                        TutMaybeFinish();
                    }
                    return;

                default: // TutOppMove pending or steps exhausted: ignore clicks
                    return;
            }
        }

        private void TutorialUpdate(float dt)
        {
            if (TutCurrent is not TutOppMove om) { _tutOppTimer = 0f; return; }
            _tutOppTimer += dt;
            if (_tutOppTimer < 0.7f) return;
            _tutOppTimer = 0f;
            int idx = _oppHand.FindIndex(c => c.Id == om.CardId);
            if (idx >= 0)
            {
                var card = _oppHand[idx];
                var caps = _board.Place(card, Owner.P2, om.R, om.C);
                _p2Played.Add(card);
                _oppHand.RemoveAt(idx);
                FlashCaptures(caps);
                Game1.playSound("bigSelect");
            }
            _tutIx++;
            TutMaybeFinish();
        }

        private void TutMaybeFinish()
        {
            if (_s.Tutorial == null || _tutIx < _s.Tutorial.Steps.Count) return;
            int p1 = _board.Count(Owner.P1) + _playerHand.Count;
            int p2 = _board.Count(Owner.P2) + _oppHand.Count;
            _result = new MatchResult { Outcome = p1 > p2 ? Outcome.Win : p1 < p2 ? Outcome.Loss : Outcome.Draw };
            _statusOverride = $"{Tr("tut.done")}  ({p1} × {p2})";
            _state = State.Done;
            _s.OnComplete?.Invoke(_result);
        }

        private List<Card> OppFive() => _p2Played.Concat(_oppHand).ToList();
        private List<Card> PlayerFive() => _p1Played.Concat(_playerHand).ToList();

        private void BeginFinish()
        {
            int p1 = _board.Count(Owner.P1) + _playerHand.Count;
            int p2 = _board.Count(Owner.P2) + _oppHand.Count;

            if (p1 == p2)
            {
                if (_round + 1 < _s.SuddenDeathRounds)
                {
                    _round++;
                    _statusOverride = $"Morte súbita! (round {_round + 1})";
                    StartRound(ControlledCards(Owner.P1), ControlledCards(Owner.P2));
                    return;
                }
                _result = new MatchResult { Outcome = Outcome.Draw };
                _statusOverride = $"Empate!  ({p1} × {p2})";
                _state = State.Done;
                _s.OnComplete?.Invoke(_result);
                return;
            }

            if (p1 > p2)
            {
                _result = new MatchResult { Outcome = Outcome.Win };
                _statusOverride = $"Você venceu!  ({p1} × {p2})";
                _state = State.PickReward;
            }
            else
            {
                _result = new MatchResult { Outcome = Outcome.Loss, LostCardId = LostCard() };
                _statusOverride = $"Você perdeu…  ({p1} × {p2})";
                _state = State.Done;
                _s.OnComplete?.Invoke(_result);
            }
        }

        private string? LostCard()
        {
            var five = PlayerFive();
            if (five.Count == 0) return null;
            return _s.Stakes switch
            {
                StakeMode.Hard => five[Game1.random.Next(five.Count)].Id,
                StakeMode.Ragnarok => five.OrderByDescending(c => c.EdgeSum()).First().Id,
                _ => null,
            };
        }

        private List<Card> ControlledCards(Owner owner)
        {
            var list = new List<Card>();
            foreach (var cell in _board.Cells)
                if (!cell.Empty && cell.Owner == owner) list.Add(cell.Card!);
            list.AddRange(owner == Owner.P1 ? _playerHand : _oppHand);
            return list;
        }

        private void Close()
        {
            if (_state != State.Done && _result == null)
                _s.OnComplete?.Invoke(new MatchResult { Outcome = Outcome.Draw });
            exitThisMenu();
        }

        // ---- layout ----
        private Rectangle CellRect(int r, int c) => new(_boardX + c * (Cell + Gap), _boardY + r * (Cell + Gap), Cell, Cell);

        private Rectangle HandRect(int i, bool bottom)
        {
            int count = (bottom ? _playerHand : _oppHand).Count;
            int total = count * (HandW + 10) - 10;
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (HandW + 10), bottom ? _playerHandY : _oppHandY, HandW, HandH);
        }

        private Rectangle RewardRect(int i, int n)
        {
            int total = n * (HandW + 12) - 12;
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (HandW + 12), yPositionOnScreen + (height - HandH) / 2, HandW, HandH);
        }

        /// <summary>Centres a card at the correct 92:128 aspect inside <paramref name="box"/>.</summary>
        private static Rectangle Fit(Rectangle box)
        {
            int h = box.Height, w = (int)(h * CardAspect);
            if (w > box.Width) { w = box.Width; h = (int)(w / CardAspect); }
            return new Rectangle(box.X + (box.Width - w) / 2, box.Y + (box.Height - h) / 2, w, h);
        }

        // ---- draw ----
        private void DrawCard(SpriteBatch b, Card card, Rectangle box, Color? tint = null)
        {
            var dest = Fit(box);
            if (tint != null) b.Draw(Game1.staminaRect, new Rectangle(dest.X - 3, dest.Y - 3, dest.Width + 6, dest.Height + 6), tint.Value);
            b.Draw(_renderer.Get(card), dest, Color.White);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // opponent name
            string opp = string.IsNullOrEmpty(_s.OpponentDisplay) ? "Oponente" : _s.OpponentDisplay;
            b.DrawString(Game1.smallFont, opp, new Vector2(xPositionOnScreen + borderWidth + 12, yPositionOnScreen + borderWidth + 8), Game1.textColor);

            // board
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    var rect = CellRect(r, c);
                    var cell = _board.Cells[r, c];
                    Color slot = cell.Element switch
                    {
                        Season.Spring => new(79, 170, 69), Season.Summer => new(224, 168, 40),
                        Season.Fall => new(210, 120, 50), Season.Winter => new(90, 165, 205), _ => new(74, 54, 34),
                    };
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), rect.X, rect.Y, rect.Width, rect.Height, slot * 0.7f, 1f, false);
                    if (!cell.Empty)
                        DrawCard(b, cell.Card!, new Rectangle(rect.X + 6, rect.Y + 6, rect.Width - 12, rect.Height - 12),
                                 cell.Owner == Owner.P1 ? P1Tint : P2Tint);
                    if (_flash.TryGetValue((r, c), out float f))
                        b.Draw(Game1.staminaRect, rect, Color.White * (f * 0.55f));
                }

            // hands
            for (int i = 0; i < _oppHand.Count; i++) DrawCard(b, _oppHand[i], HandRect(i, false), P2Tint);
            for (int i = 0; i < _playerHand.Count; i++)
            {
                var rect = HandRect(i, true);
                if (i == _selected) rect = new Rectangle(rect.X, rect.Y - 12, rect.Width, rect.Height);
                DrawCard(b, _playerHand[i], rect, P1Tint);
            }

            // status strip
            string status;
            if (_s.Tutorial != null && _state == State.Playing)
                status = TutCurrent switch
                {
                    TutPlayerMove pm => Tr(pm.HintKey),
                    TutOppMove => Tr("tut.status.opp"),
                    _ => Tr("tut.status.say"),
                };
            else
                status = _statusOverride ?? (_turn == Owner.P1 ? "Seu turno — escolha uma carta e uma casa" : "Vez do oponente…");

            string score = $"Você {_board.Count(Owner.P1)} × {_board.Count(Owner.P2)} {opp}";
            var ssz = Game1.smallFont.MeasureString(score);
            float availW = width - borderWidth * 2 - 36 - ssz.X;
            var stsz = Game1.smallFont.MeasureString(status);
            float stScale = Math.Min(1f, availW / Math.Max(1f, stsz.X));
            b.DrawString(Game1.smallFont, status, new Vector2(xPositionOnScreen + borderWidth + 12, _statusY), Game1.textColor, 0f, Vector2.Zero, stScale, SpriteEffects.None, 0f);
            b.DrawString(Game1.smallFont, score, new Vector2(xPositionOnScreen + width - borderWidth - 12 - ssz.X, _statusY), Game1.textColor);

            // tutorial visuals: pulsing highlights on the required card/cell, and Abigail's dialogue overlay
            if (_s.Tutorial != null && _state == State.Playing)
            {
                float pulse = 0.45f + 0.35f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 5.0);
                if (TutCurrent is TutPlayerMove pmv)
                {
                    int hi = _playerHand.FindIndex(c => c.Id == pmv.CardId);
                    if (hi >= 0)
                    {
                        var hr = HandRect(hi, true);
                        if (hi == _selected) hr = new Rectangle(hr.X, hr.Y - 12, hr.Width, hr.Height);
                        DrawHighlight(b, hr, pulse);
                    }
                    if (_selected >= 0) DrawHighlight(b, CellRect(pmv.R, pmv.C), pulse);
                }
                if (TutCurrent is TutSay say) DrawSayOverlay(b, say);
            }

            // reward pick overlay
            if (_state == State.PickReward)
            {
                var five = OppFive();
                b.Draw(Game1.fadeToBlackRect, new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height), Color.Black * 0.55f);
                string t = "Você venceu! Escolha uma carta:";
                var tsz = Game1.dialogueFont.MeasureString(t);
                b.DrawString(Game1.dialogueFont, t, new Vector2(xPositionOnScreen + (width - tsz.X) / 2, RewardRect(0, five.Count).Y - 56), Color.White);
                for (int i = 0; i < five.Count; i++) DrawCard(b, five[i], RewardRect(i, five.Count), P2Tint);
            }

            base.draw(b);
            drawMouse(b);
        }

        private static void DrawHighlight(SpriteBatch b, Rectangle r, float a)
        {
            Color c = Color.Gold * a;
            const int t = 4;
            b.Draw(Game1.staminaRect, new Rectangle(r.X - t, r.Y - t, r.Width + 2 * t, t), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.X - t, r.Bottom, r.Width + 2 * t, t), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.X - t, r.Y, t, r.Height), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.Right, r.Y, t, r.Height), c);
        }

        private void DrawSayOverlay(SpriteBatch b, TutSay say)
        {
            _abbyPortrait ??= Game1.content.Load<Texture2D>("Portraits/Abigail");
            string text = Tr(say.Key);
            int w = Math.Min(920, width - 32);
            int h = 172;
            int x = xPositionOnScreen + (width - w) / 2;
            int y = yPositionOnScreen + height - h - 46;
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, w, h, Color.White, 1f, true);
            b.Draw(_abbyPortrait, new Rectangle(x + 18, y + 22, 128, 128), new Rectangle(0, 0, 64, 64), Color.White);
            string wrapped = Game1.parseText(text, Game1.smallFont, w - 190);
            b.DrawString(Game1.smallFont, wrapped, new Vector2(x + 164, y + 22), Game1.textColor);
            float blink = 0.6f + 0.4f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 4.0);
            b.DrawString(Game1.smallFont, "▶", new Vector2(x + w - 34, y + h - 40), Game1.textColor * blink);
        }
    }
}
