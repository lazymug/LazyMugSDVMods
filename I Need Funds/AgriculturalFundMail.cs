using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace INeedFunds
{
    public class AgriculturalFundMail : LetterViewerMenu
    {
        private readonly IModHelper helper;
        private Rectangle acceptButtonRect;

        public AgriculturalFundMail(string mail, IModHelper helper)
            : base(mail)
        {
            this.helper = helper;
            // Calculate the position and dimensions of the "Accept Terms" button
            // based on the mail message content and layout.
            // This is just an example, adjust as needed for your specific layout.
            int buttonX = xPositionOnScreen + width / 2 - 50;
            int buttonY = yPositionOnScreen + height - 100;
            acceptButtonRect = new Rectangle(buttonX, buttonY, 100, 50);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // Check if click is on the "Accept Terms" button
            if (acceptButtonRect.Contains(x, y))
            {
                // Trigger subscription logic
                ModEntry.instance.Data.IsSubscribed = true; // Update subscription status
                Game1.player.Money += ModEntry.instance.Data.LoanAmount; // Grant loan amount
                Game1.addHUDMessage(new HUDMessage(helper.Translation.Get("inf.mail.subscribed"), HUDMessage.newQuest_type)); // Confirmation message
                ModEntry.instance.Helper.Data.WriteSaveData("inf_data", ModEntry.instance.Data); // Save updated data
                exitThisMenu(); // Close the mail message
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // Draw the "Accept Terms" button (optional)
            // You can use a custom texture or draw a simple rectangle.
            // This is just an example, adjust as needed.
            b.Draw(Game1.staminaRect, acceptButtonRect, Color.White);
            Utility.drawTextWithShadow(b, helper.Translation.Get("inf.mail.accept"), Game1.smallFont, new Vector2(acceptButtonRect.X + 10, acceptButtonRect.Y + 10), Color.Black);
        }
    }
}