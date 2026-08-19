using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using ValleyTriad.Data;
using ValleyTriad.Models;
using ValleyTriad.Rendering;

namespace ValleyTriad.UI
{
    /// <summary>Collection album + deck builder (scrollable). Pick 5 (max 1 Legendary, 2 Rare). Saves on close.</summary>
    public class DeckMenu : IClickableMenu
    {
        // The card texture is 368x512, so a 92x128 cell is an exact 4x downscale — that keeps the
        // pixel art crisp instead of the blurry non-integer shrink a smaller cell would force.
        private const int ColW = 92, ColH = 128, ColGap = 10, Cols = 7;
        private const int SlotW = 108, SlotH = 148;
        private const float CardAspect = 92f / 128f;
        private int _visRows;

        private readonly CardRenderer _renderer;
        private readonly CollectionManager _coll;
        private readonly CardDatabase _db;
        private readonly List<(Card card, int count)> _owned;
        private readonly List<string> _deck;
        private int _scrollRow;
        private int _gridX, _gridY, _slotY, _capsY;

        public DeckMenu(CardRenderer renderer, CollectionManager coll, CardDatabase db)
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            _renderer = renderer; _coll = coll; _db = db;
            _owned = coll.Owned().OrderBy(t => t.card.Tier).ThenBy(t => t.card.NameKey).ToList();
            _deck = new List<string>(coll.DeckIds);
            _renderer.Prewarm(_owned.Select(t => t.card));

            int gridW = Cols * (ColW + ColGap) - ColGap;
            int deckRowW = Deck.Size * (SlotW + 10) - 10;
            int contentW = Math.Max(gridW, deckRowW);

            // Show only as many rows as the collection actually needs, capped by what fits on
            // screen — so a small collection gets a snug menu instead of a mostly empty one.
            int chrome = borderWidth * 2 + 64 + 26 + SlotH + 44;
            int rowsNeeded = Math.Max(1, (int)Math.Ceiling(_owned.Count / (float)Cols));
            int rowsThatFit = Math.Max(1, (Game1.uiViewport.Height - 40 - chrome + ColGap) / (ColH + ColGap));
            _visRows = Math.Min(rowsNeeded, rowsThatFit);
            int viewportH = _visRows * (ColH + ColGap) - ColGap;

            width = contentW + borderWidth * 2 + 40;
            height = chrome + viewportH;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;

            _gridX = xPositionOnScreen + (width - gridW) / 2;
            _gridY = yPositionOnScreen + borderWidth + 56;
            _slotY = _gridY + viewportH + 26;
            _capsY = _slotY + SlotH + 12;
        }

        private int MaxScroll => Math.Max(0, (int)Math.Ceiling(_owned.Count / (float)Cols) - _visRows);
        private int InDeck(string id) => _deck.Count(d => d == id);
        private int TierInDeck(Tier t) => _deck.Count(id => _db.Get(id)?.Tier == t);

        private bool CanAdd(Card c)
        {
            if (_deck.Count >= Deck.Size) return false;
            if (InDeck(c.Id) >= _coll.Count(c.Id)) return false;
            if (c.Tier == Tier.Legendary && TierInDeck(Tier.Legendary) >= Deck.MaxLegendary) return false;
            if (c.Tier == Tier.Rare && TierInDeck(Tier.Rare) >= Deck.MaxRare) return false;
            return true;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            _scrollRow = Math.Clamp(_scrollRow - Math.Sign(direction), 0, MaxScroll);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y)) { exitThisMenu(); return; }

            for (int i = 0; i < _owned.Count; i++)
            {
                var rect = CollRect(i);
                if (rect.HasValue && rect.Value.Contains(x, y))
                {
                    if (CanAdd(_owned[i].card)) { _deck.Add(_owned[i].card.Id); Game1.playSound("smallSelect"); }
                    else Game1.playSound("cancel");
                    return;
                }
            }
            for (int i = 0; i < _deck.Count; i++)
                if (SlotRect(i).Contains(x, y)) { _deck.RemoveAt(i); Game1.playSound("trashcan"); return; }
        }

        protected override void cleanupBeforeExit() { _coll.SetDeck(_deck); base.cleanupBeforeExit(); }

        private Rectangle? CollRect(int i)
        {
            int row = i / Cols - _scrollRow, col = i % Cols;
            if (row < 0 || row >= _visRows) return null;
            return new Rectangle(_gridX + col * (ColW + ColGap), _gridY + row * (ColH + ColGap), ColW, ColH);
        }
        private Rectangle SlotRect(int i)
        {
            int total = Deck.Size * (SlotW + 10) - 10;
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (SlotW + 10), _slotY, SlotW, SlotH);
        }
        private static Rectangle Fit(Rectangle box)
        {
            int h = box.Height, w = (int)(h * CardAspect);
            if (w > box.Width) { w = box.Width; h = (int)(w / CardAspect); }
            return new Rectangle(box.X + (box.Width - w) / 2, box.Y + (box.Height - h) / 2, w, h);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            b.DrawString(Game1.smallFont, "Coleção", new Vector2(xPositionOnScreen + borderWidth + 12, yPositionOnScreen + borderWidth + 6), Game1.textColor);
            if (MaxScroll > 0)
            {
                string hint = "Role para ver mais ▲▼";
                var hs = Game1.smallFont.MeasureString(hint);
                b.DrawString(Game1.smallFont, hint, new Vector2(xPositionOnScreen + width - borderWidth - 12 - hs.X, yPositionOnScreen + borderWidth + 6), Game1.textColor * 0.7f);
            }

            for (int i = 0; i < _owned.Count; i++)
            {
                var maybe = CollRect(i);
                if (maybe is not Rectangle rect) continue;
                var (card, count) = _owned[i];
                bool exhausted = InDeck(card.Id) >= count;
                b.Draw(_renderer.Get(card), Fit(rect), Color.White * (exhausted ? 0.35f : 1f));
                int left = count - InDeck(card.Id);
                // count badge: dark pill in the corner so it stays readable over the card art
                string tag = $"x{left}";
                var ts = Game1.tinyFont.MeasureString(tag);
                var pill = new Rectangle(rect.X + rect.Width - (int)ts.X - 10, rect.Y + rect.Height - (int)ts.Y - 6, (int)ts.X + 8, (int)ts.Y + 2);
                b.Draw(Game1.staminaRect, pill, new Color(28, 20, 12) * 0.78f);
                b.DrawString(Game1.tinyFont, tag, new Vector2(pill.X + 4, pill.Y), left > 0 ? Color.White : Color.Gray);
            }

            for (int i = 0; i < Deck.Size; i++)
            {
                var rect = SlotRect(i);
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), rect.X, rect.Y, rect.Width, rect.Height, Color.White, 1f, false);
                if (i < _deck.Count)
                {
                    var c = _db.Get(_deck[i]);
                    // draw at the same exact 4x downscale as the grid, centred in the slot
                    if (c != null)
                        b.Draw(_renderer.Get(c), new Rectangle(rect.X + (rect.Width - ColW) / 2, rect.Y + (rect.Height - ColH) / 2, ColW, ColH), Color.White);
                }
            }

            string caps = $"Deck {_deck.Count}/5    Lendária {TierInDeck(Tier.Legendary)}/{Deck.MaxLegendary}    Rara {TierInDeck(Tier.Rare)}/{Deck.MaxRare}    (clique uma carta para adicionar · clique um slot para remover)";
            var sz = Game1.smallFont.MeasureString(caps);
            float scale = Math.Min(1f, (width - borderWidth * 2 - 16) / sz.X);
            b.DrawString(Game1.smallFont, caps, new Vector2(xPositionOnScreen + (width - sz.X * scale) / 2, _capsY), Game1.textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            base.draw(b);
            drawMouse(b);
        }
    }
}
