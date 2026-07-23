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
    /// <summary>
    /// A playable Triple Triad match: click a card in your hand, then an empty cell to place it.
    /// The opponent plays with a simple greedy AI. Captures resolve via <see cref="Board"/>.
    /// </summary>
    public class TriadMenu : IClickableMenu
    {
        private const int Cell = 96, Gap = 6, HandCardW = 72, HandCardH = 100;

        private readonly CardRenderer _renderer;
        private readonly Board _board;
        private readonly List<Card> _playerHand;
        private readonly List<Card> _oppHand;

        private Owner _turn = Owner.P1;
        private int _selected = -1;
        private string? _result;
        private int _boardX, _boardY;

        private static readonly Color P1Tint = new(90, 150, 214), P2Tint = new(206, 96, 80);

        public TriadMenu(CardRenderer renderer, ModConfig config, List<Card> playerDeck, List<Card> oppDeck)
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            _renderer = renderer;
            _board = new Board(config.RuleSame, config.RulePlus, config.RuleCombo, config.RuleElemental);
            _playerHand = new List<Card>(playerDeck);
            _oppHand = new List<Card>(oppDeck);
            _renderer.Prewarm(_playerHand.Concat(_oppHand));

            if (config.RuleElemental && config.ElementalCells > 0) AssignElementalCells(config.ElementalCells);

            int boardPx = 3 * Cell + 2 * Gap;
            width = boardPx + borderWidth * 2 + 220;
            height = boardPx + borderWidth * 2 + HandCardH * 2 + 60;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;
            _boardX = xPositionOnScreen + (width - boardPx) / 2;
            _boardY = yPositionOnScreen + HandCardH + 40;
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
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y)) { exitThisMenu(); return; }
            if (_result != null) { exitThisMenu(); return; }
            if (_turn != Owner.P1) return;

            // select a hand card
            for (int i = 0; i < _playerHand.Count; i++)
                if (HandRect(i, bottom: true).Contains(x, y)) { _selected = i; Game1.playSound("smallSelect"); return; }

            // place on an empty cell
            if (_selected < 0 || _selected >= _playerHand.Count) return;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (CellRect(r, c).Contains(x, y) && _board.Cells[r, c].Empty)
                    {
                        _board.Place(_playerHand[_selected], Owner.P1, r, c);
                        _playerHand.RemoveAt(_selected);
                        _selected = -1;
                        Game1.playSound("bigSelect");
                        AfterMove();
                        return;
                    }
        }

        private void AfterMove()
        {
            if (_board.IsFull()) { Finish(); return; }
            _turn = Owner.P2;
            OpponentMove();
            if (_board.IsFull()) { Finish(); return; }
            _turn = Owner.P1;
        }

        private void OpponentMove()
        {
            if (_oppHand.Count == 0) return;
            int bestScore = -1, bi = 0, br = 0, bc = 0;
            for (int i = 0; i < _oppHand.Count; i++)
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        if (_board.Cells[r, c].Empty)
                        {
                            int s = _board.EvaluatePlacement(_oppHand[i], Owner.P2, r, c);
                            if (s > bestScore) { bestScore = s; bi = i; br = r; bc = c; }
                        }
            _board.Place(_oppHand[bi], Owner.P2, br, bc);
            _oppHand.RemoveAt(bi);
            Game1.playSound("bigSelect");
        }

        private void Finish()
        {
            int p1 = _board.Count(Owner.P1) + _playerHand.Count;
            int p2 = _board.Count(Owner.P2) + _oppHand.Count;
            _result = p1 > p2 ? "Você venceu!" : p2 > p1 ? "Você perdeu…" : "Empate!";
            _result += $"  ({p1} × {p2})";
        }

        // ---- layout ----
        private Rectangle CellRect(int r, int c) => new(_boardX + c * (Cell + Gap), _boardY + r * (Cell + Gap), Cell, Cell);
        private Rectangle HandRect(int i, bool bottom)
        {
            var hand = bottom ? _playerHand : _oppHand;
            int total = hand.Count * (HandCardW + 6);
            int sx = xPositionOnScreen + (width - total) / 2;
            int y = bottom ? _boardY + 3 * (Cell + Gap) + 12 : yPositionOnScreen + 24;
            return new Rectangle(sx + i * (HandCardW + 6), y, HandCardW, HandCardH);
        }

        // ---- draw ----
        private void DrawCard(SpriteBatch b, Card card, Rectangle dest, Color? tint = null)
        {
            if (tint != null) b.Draw(Game1.staminaRect, new Rectangle(dest.X - 3, dest.Y - 3, dest.Width + 6, dest.Height + 6), tint.Value);
            b.Draw(_renderer.Get(card), dest, Color.White);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // board cells
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    var rect = CellRect(r, c);
                    var cell = _board.Cells[r, c];
                    Color slot = cell.Element switch
                    {
                        Season.Spring => new(79, 170, 69), Season.Summer => new(224, 168, 40),
                        Season.Fall => new(210, 120, 50), Season.Winter => new(90, 165, 205), _ => new(64, 46, 28),
                    };
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), rect.X, rect.Y, rect.Width, rect.Height, slot * 0.6f, 1f, false);
                    if (!cell.Empty)
                        DrawCard(b, cell.Card!, new Rectangle(rect.X + 4, rect.Y + 2, rect.Width - 8, rect.Height - 4),
                                 cell.Owner == Owner.P1 ? P1Tint : P2Tint);
                }

            // hands
            for (int i = 0; i < _oppHand.Count; i++)
                DrawCard(b, _oppHand[i], HandRect(i, false), P2Tint);
            for (int i = 0; i < _playerHand.Count; i++)
            {
                var rect = HandRect(i, true);
                if (i == _selected) rect = new Rectangle(rect.X, rect.Y - 10, rect.Width, rect.Height);
                DrawCard(b, _playerHand[i], rect, P1Tint);
            }

            // status
            string status = _result ?? (_turn == Owner.P1 ? "Seu turno" : "Vez do oponente");
            b.DrawString(Game1.dialogueFont, status,
                new Vector2(xPositionOnScreen + 40, yPositionOnScreen + height - 44), Game1.textColor);
            string score = $"{_board.Count(Owner.P1)} × {_board.Count(Owner.P2)}";
            b.DrawString(Game1.dialogueFont, score,
                new Vector2(xPositionOnScreen + width - 140, yPositionOnScreen + height - 44), Game1.textColor);

            base.draw(b);
            drawMouse(b);
        }
    }
}
