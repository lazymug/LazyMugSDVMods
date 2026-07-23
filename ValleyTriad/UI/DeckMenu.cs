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
    /// <summary>Collection album + deck builder: pick 5 (max 1 Legendary, 2 Rare). Saves on close.</summary>
    public class DeckMenu : IClickableMenu
    {
        private const int ColW = 60, ColH = 84, ColGap = 8, Cols = 6;
        private const int SlotW = 72, SlotH = 100;

        private readonly CardRenderer _renderer;
        private readonly CollectionManager _coll;
        private readonly CardDatabase _db;
        private readonly List<(Card card, int count)> _owned;
        private readonly List<string> _deck;

        public DeckMenu(CardRenderer renderer, CollectionManager coll, CardDatabase db)
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            _renderer = renderer; _coll = coll; _db = db;
            _owned = coll.Owned().OrderBy(t => t.card.Tier).ThenBy(t => t.card.Id).ToList();
            _deck = new List<string>(coll.DeckIds);
            _renderer.Prewarm(_owned.Select(t => t.card));

            width = Cols * (ColW + ColGap) + borderWidth * 2 + 32;
            height = 720;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;
        }

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

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y)) { SaveAndExit(); return; }

            for (int i = 0; i < _owned.Count; i++)
                if (CollRect(i).Contains(x, y))
                {
                    if (CanAdd(_owned[i].card)) { _deck.Add(_owned[i].card.Id); Game1.playSound("smallSelect"); }
                    else Game1.playSound("cancel");
                    return;
                }
            for (int i = 0; i < _deck.Count; i++)
                if (SlotRect(i).Contains(x, y)) { _deck.RemoveAt(i); Game1.playSound("trashcan"); return; }
        }

        protected override void cleanupBeforeExit() { _coll.SetDeck(_deck); base.cleanupBeforeExit(); }
        private void SaveAndExit() { exitThisMenu(); }

        private Rectangle CollRect(int i)
        {
            int gx = xPositionOnScreen + borderWidth, gy = yPositionOnScreen + 100;
            int r = i / Cols, c = i % Cols;
            return new Rectangle(gx + c * (ColW + ColGap), gy + r * (ColH + ColGap), ColW, ColH);
        }
        private Rectangle SlotRect(int i)
        {
            int total = Deck.Size * (SlotW + 8);
            int sx = xPositionOnScreen + (width - total) / 2;
            return new Rectangle(sx + i * (SlotW + 8), yPositionOnScreen + height - SlotH - 70, SlotW, SlotH);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            b.DrawString(Game1.dialogueFont, "Coleção", new Vector2(xPositionOnScreen + borderWidth, yPositionOnScreen + 48), Game1.textColor);

            for (int i = 0; i < _owned.Count; i++)
            {
                var (card, count) = _owned[i];
                var rect = CollRect(i);
                bool exhausted = InDeck(card.Id) >= count;
                b.Draw(_renderer.Get(card), rect, Color.White * (exhausted ? 0.4f : 1f));
                b.DrawString(Game1.smallFont, $"x{count - InDeck(card.Id)}",
                    new Vector2(rect.X + rect.Width - 26, rect.Y + rect.Height - 22), Game1.textColor);
            }

            // deck slots
            for (int i = 0; i < Deck.Size; i++)
            {
                var rect = SlotRect(i);
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), rect.X, rect.Y, rect.Width, rect.Height, Color.White, 1f, false);
                if (i < _deck.Count)
                {
                    var c = _db.Get(_deck[i]);
                    if (c != null) b.Draw(_renderer.Get(c), new Rectangle(rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8), Color.White);
                }
            }

            string caps = $"Deck {_deck.Count}/5   ·   Lendária {TierInDeck(Tier.Legendary)}/{Deck.MaxLegendary}   ·   Rara {TierInDeck(Tier.Rare)}/{Deck.MaxRare}";
            var sz = Game1.smallFont.MeasureString(caps);
            b.DrawString(Game1.smallFont, caps, new Vector2(xPositionOnScreen + (width - sz.X) / 2, yPositionOnScreen + height - 52), Game1.textColor);

            base.draw(b);
            drawMouse(b);
        }
    }
}
