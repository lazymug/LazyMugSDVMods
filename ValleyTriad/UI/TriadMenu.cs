using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ValleyTriad.UI
{
    /// <summary>
    /// Placeholder match menu — draws the 3×3 board frame and closes on click.
    /// The real board interaction, card rendering and AI are the next milestone.
    /// </summary>
    public class TriadMenu : IClickableMenu
    {
        private const int BoardCells = 3;
        private const int CellSize = 96;
        private const int Gap = 8;

        public TriadMenu()
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            int boardPx = BoardCells * CellSize + (BoardCells - 1) * Gap;
            width = boardPx + IClickableMenu.borderWidth * 2 + 64;
            height = boardPx + IClickableMenu.borderWidth * 2 + 96;
            xPositionOnScreen = (Game1.uiViewport.Width - width) / 2;
            yPositionOnScreen = (Game1.uiViewport.Height - height) / 2;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (upperRightCloseButton != null && upperRightCloseButton.containsPoint(x, y))
                exitThisMenu();
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            string title = "Valley Triad";
            Vector2 tsize = Game1.dialogueFont.MeasureString(title);
            b.DrawString(Game1.dialogueFont, title,
                new Vector2(xPositionOnScreen + (width - tsize.X) / 2, yPositionOnScreen + 40), Game1.textColor);

            int boardPx = BoardCells * CellSize + (BoardCells - 1) * Gap;
            int ox = xPositionOnScreen + (width - boardPx) / 2;
            int oy = yPositionOnScreen + 96;
            for (int r = 0; r < BoardCells; r++)
                for (int c = 0; c < BoardCells; c++)
                {
                    var rect = new Rectangle(ox + c * (CellSize + Gap), oy + r * (CellSize + Gap), CellSize, CellSize);
                    IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                        rect.X, rect.Y, rect.Width, rect.Height, Color.White, 1f, false);
                }

            base.draw(b);
            drawMouse(b);
        }
    }
}
