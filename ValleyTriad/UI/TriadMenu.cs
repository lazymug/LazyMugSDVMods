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
        // layout (computed from the UI viewport so the board uses the available screen)
        private const float CardAspect = 92f / 128f;
        private readonly int _cellSz, _gap = 10, _handH, _handW, _bandGap = 16, _topPad = 52, _statusH = 76;
        private static readonly Color Cream = new(242, 230, 199);
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

            // size the board to the screen: fixed = 2*border + pads/gaps/status; variable = 2 hands + 3 cells
            int availH = Game1.uiViewport.Height - 24;
            int fixedH = borderWidth * 2 + _topPad + _bandGap * 2 + _statusH + 16 + 2 * _gap;
            _cellSz = Math.Clamp((int)((availH - fixedH) / 5.3f), 88, 160);
            _handH = (int)(_cellSz * 1.15f);
            _handW = (int)(_handH * CardAspect);

            int boardPx = 3 * _cellSz + 2 * _gap;
            int handRowW = Deck.Size * (_handW + 10) - 10;
            int contentW = Math.Max(boardPx, handRowW);
            width = Math.Min(contentW + borderWidth * 2 + 64, Game1.uiViewport.Width - 16);
            height = borderWidth * 2 + _topPad + _handH + _bandGap + boardPx + _bandGap + _handH + _statusH + 16;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;

            _boardX = xPositionOnScreen + (width - boardPx) / 2;
            _oppHandY = yPositionOnScreen + borderWidth + _topPad;
            _boardY = _oppHandY + _handH + _bandGap;
            _playerHandY = _boardY + boardPx + _bandGap;
            _statusY = _playerHandY + _handH + 10;

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
        private Rectangle CellRect(int r, int c) => new(_boardX + c * (_cellSz + _gap), _boardY + r * (_cellSz + _gap), _cellSz, _cellSz);

        private Rectangle HandRect(int i, bool bottom)
        {
            int count = (bottom ? _playerHand : _oppHand).Count;
            int total = count * (_handW + 10) - 10;
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (_handW + 10), bottom ? _playerHandY : _oppHandY, _handW, _handH);
        }

        private Rectangle RewardRect(int i, int n)
        {
            int total = n * (_handW + 12) - 12;
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (_handW + 12), yPositionOnScreen + (height - _handH) / 2, _handW, _handH);
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

        /// <summary>Card-table backdrop: dark wood rim, gold inlay and a green felt playfield.</summary>
        private void DrawTable(SpriteBatch b)
        {
            var r = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            b.Draw(Game1.staminaRect, new Rectangle(r.X + 8, r.Y + 10, r.Width, r.Height), Color.Black * 0.4f); // sombra
            b.Draw(Game1.staminaRect, r, new Color(43, 28, 14));                                                // contorno
            b.Draw(Game1.staminaRect, new Rectangle(r.X + 5, r.Y + 5, r.Width - 10, r.Height - 10), new Color(112, 74, 40));
            b.Draw(Game1.staminaRect, new Rectangle(r.X + 13, r.Y + 13, r.Width - 26, r.Height - 26), new Color(82, 52, 26));
            DrawBorderRect(b, new Rectangle(r.X + 17, r.Y + 17, r.Width - 34, r.Height - 34), new Color(208, 172, 96), 2); // filete dourado
            var felt = new Rectangle(r.X + 21, r.Y + 21, r.Width - 42, r.Height - 42);
            b.Draw(Game1.staminaRect, felt, new Color(44, 92, 63)); // feltro
            for (int i = 0; i < 12; i++) // vinheta do feltro
                DrawBorderRect(b, new Rectangle(felt.X + i, felt.Y + i, felt.Width - 2 * i, felt.Height - 2 * i),
                    new Color(20, 52, 34) * (0.45f - i * 0.035f), 1);
        }

        private static void DrawShadowedText(SpriteBatch b, string text, Vector2 pos, Color color, float scale = 1f)
        {
            b.DrawString(Game1.smallFont, text, pos + new Vector2(2, 2), Color.Black * 0.55f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            b.DrawString(Game1.smallFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.55f);
            DrawTable(b);

            // opponent name (top-left) + live score (top-right)
            string opp = string.IsNullOrEmpty(_s.OpponentDisplay) ? "Oponente" : _s.OpponentDisplay;
            DrawShadowedText(b, opp, new Vector2(xPositionOnScreen + borderWidth + 4, yPositionOnScreen + borderWidth - 4), Cream);
            string scoreTop = $"Você {_board.Count(Owner.P1)} × {_board.Count(Owner.P2)} {opp}";
            var scsz = Game1.smallFont.MeasureString(scoreTop);
            DrawShadowedText(b, scoreTop, new Vector2(xPositionOnScreen + width - borderWidth - 4 - scsz.X, yPositionOnScreen + borderWidth - 4), Cream);

            // board
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    var rect = CellRect(r, c);
                    var cell = _board.Cells[r, c];
                    Color slot = cell.Element switch
                    {
                        Season.Spring => new(79, 170, 69), Season.Summer => new(224, 168, 40),
                        Season.Fall => new(210, 120, 50), Season.Winter => new(90, 165, 205), _ => new(30, 66, 45),
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

            // full-width status line at full size, wrapped to a second line when needed
            string wrappedStatus = Game1.parseText(status, Game1.smallFont, width - borderWidth * 2 - 24);
            DrawShadowedText(b, wrappedStatus, new Vector2(xPositionOnScreen + borderWidth + 4, _statusY), Cream);

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

        private static void DrawBorderRect(SpriteBatch b, Rectangle r, Color c, int t)
        {
            b.Draw(Game1.staminaRect, new Rectangle(r.X, r.Y, r.Width, t), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.X, r.Y, t, r.Height), c);
            b.Draw(Game1.staminaRect, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }

        private static void DrawHighlight(SpriteBatch b, Rectangle r, float a)
            => DrawBorderRect(b, new Rectangle(r.X - 4, r.Y - 4, r.Width + 8, r.Height + 8), Color.Gold * a, 4);

        private void DrawSayOverlay(SpriteBatch b, TutSay say)
        {
            _abbyPortrait ??= Game1.content.Load<Texture2D>("Portraits/Abigail");
            string text = Tr(say.Key);
            int w = Math.Min(1080, width - 24);
            int h = 212;
            int x = xPositionOnScreen + (width - w) / 2;
            int y = yPositionOnScreen + height - h - _statusH - 4;
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, w, h, Color.White, 1f, true);
            b.Draw(_abbyPortrait, new Rectangle(x + 20, y + 28, 156, 156), new Rectangle(0, 0, 64, 64), Color.White);
            string wrapped = Game1.parseText(text, Game1.smallFont, w - 230);
            b.DrawString(Game1.smallFont, wrapped, new Vector2(x + 196, y + 26), Game1.textColor);
            float blink = 0.6f + 0.4f * (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 4.0);
            b.DrawString(Game1.smallFont, "▶", new Vector2(x + w - 38, y + h - 44), Game1.textColor * blink);
        }
    }
}
