using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuralMagazineCompetition.Enums;
using RuralMagazineCompetition.Helpers;
using RuralMagazineCompetition.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;

namespace RuralMagazineCompetition
{
    internal sealed class ModEntry: Mod
    {
        public static Mod Instance { get; private set; }
        private RuralMagazine _ruralMagazine;
        private IMailHelper _mailHelper;
        
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveCreated += OnSaveCreated;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button == SButton.C)
            {
                // var context = MenuData.Edibles();
                // Game1.activeClickableMenu = _viewEngine.CreateMenuFromAsset(
                //     "Mods/TestMod/Views/ScrollingItemGrid",
                //     context);
                
                
                Game1.activeClickableMenu = new ExpandableMenu();
                // Show the RuralMagazineUI
                // if (_ruralMagazineUi == null)
                // {
                //     return;
                // }
                // _ruralMagazineUi.ShowCompetitionUi();
            }
        }
        
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            // call the RuralMagazine.UpdateShippedItems method to track the items shipped that day.
            var items = Game1.getFarm().getShippingBin(Game1.player);
            if (items != null)
            {
                _ruralMagazine.UpdateShippedItems(items, Game1.season);
            }
            // check day if 28 send mail and calculate prize
            if (Game1.dayOfMonth == 28)
            {
                // todo: calculate prize
                // todo: send mail with prize
                if (Game1.season == Season.Winter)
                {
                    _ruralMagazine.Refresh();
                }
                // todo: send mail with items to be valuated on next month
                // MailHelper.SendMailWithItemsToBeValuatedOnNextSeason();
            }
        }
        
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // initialize the RuralMagazine instance and send the introductory mail to the player.
            Monitor.Log("=== OnGameLaunched (init) ===", LogLevel.Warn);
            _ruralMagazine = new RuralMagazine();
            Monitor.Log("RuralMagazine object initialized!", LogLevel.Warn);
            _mailHelper = new MailHelper();
            Monitor.Log("MailHelper object initialized!", LogLevel.Warn);
            Monitor.Log("=== OnGameLaunched (end) ===", LogLevel.Warn);
        }
        
        private void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
        {
            Monitor.Log("=== OnSaveCreated (init) ===", LogLevel.Warn);
            _mailHelper.Initialize(Game1.uniqueIDForThisGame.ToString());
            _mailHelper.SendMailOfCompetitionPresentation(Game1.player);
            // MailHelper.SendMailWithItemsToBeValuatedOnNextSeason();
            Monitor.Log("=== OnSaveCreated (end) ===", LogLevel.Warn);
        }
        
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("=== OnSaveLoaded (init) ===", LogLevel.Warn);
            // todo: set items on rural magazine class
            _mailHelper.Initialize(Game1.uniqueIDForThisGame.ToString());
            if (Game1.dayOfMonth == 1 && Game1.season == Season.Spring && Game1.year == 1)
            {
                _mailHelper.SendMailOfCompetitionPresentation(Game1.player);
            }
            Monitor.Log("=== OnSaveLoaded (end) ===", LogLevel.Warn);
        }
    }
    
    /**
     * Remove below
     */
    public class ExpandableMenu : IClickableMenu
    {
        private readonly List<ExpandableCategory> categories;
        private const int MenuWidth = 600;
        private const int MenuHeight = 500;
        private const int HeaderHeight = 50;
        private const int ItemHeight = 30;
        
        // Scrolling logic
        private int scrollOffset;
        private const int ScrollSpeed = 30;
        private Rectangle ScrollArea;

        public ExpandableMenu()
            : base(Game1.uiViewport.Width / 2 - MenuWidth / 2,
                   Game1.uiViewport.Height / 2 - MenuHeight / 2,
                   MenuWidth, MenuHeight)
        {
            var cropItems = ItemData.Items.Where(item => item.Value.Category == CompetitionCategoryEnum.Crops && item.Value.AvailableSeasons.Contains(Season.Spring))
                .Select(item => item.Key) // Select only the item names (keys)
                .ToList();
            var animalProduce = ItemData.Items.Where(item => item.Value.Category == CompetitionCategoryEnum.AnimalProduce)
                .Select(item => item.Key) // Select only the item names (keys)
                .ToList();
            var minerals = ItemData.Items.Where(item => item.Value.Category == CompetitionCategoryEnum.Minerals)
                .Select(item => item.Key)
                .ToList();
            
            var cropItemsData = ItemRegistry.ItemTypes
                .Single(type => type.Identifier == ItemRegistry.type_object)
                .GetAllIds()
                .Select(id => ItemRegistry.GetDataOrErrorItem(id))
                .Where(data => cropItems.Contains(data.ItemId))
                .ToList();
            var animalProduceData = ItemRegistry.ItemTypes
                .Single(type => type.Identifier == ItemRegistry.type_object)
                .GetAllIds()
                .Select(id => ItemRegistry.GetDataOrErrorItem(id))
                .Where(data => animalProduce.Contains(data.ItemId))
                .ToList();
            var mineralsData = ItemRegistry.ItemTypes
                .Single(type => type.Identifier == ItemRegistry.type_object)
                .GetAllIds()
                .Select(id => ItemRegistry.GetDataOrErrorItem(id))
                .Where(data => minerals.Contains(data.ItemId))
                .ToList();
            categories = new List<ExpandableCategory>
            {
                new ExpandableCategory("Crop", cropItemsData),
                new ExpandableCategory("Animal Produce", animalProduceData),
                new ExpandableCategory("Minerals", mineralsData),
                // new ExpandableCategory("Forage", new List<string> { "Wild Horseradish", "Daffodil", "Leek", "Dandelion" }),
                // new ExpandableCategory("Artisan Goods", new List<string> { "Wine", "Cheese", "Honey" }),
                // new ExpandableCategory("Fish", new List<string> { "Salmon", "Tuna", "Carp", "Lobster" })
            };
            
            // Define scrollable area
            ScrollArea = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height - 40);
        }

        public override void draw(SpriteBatch b)
        {
            // Draw the background
            IClickableMenu.drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

            // Enable scissor testing for the scrollable area
            RasterizerState originalState = b.GraphicsDevice.RasterizerState;
            b.End();
            RasterizerState scissorState = new RasterizerState { ScissorTestEnable = true };
            b.GraphicsDevice.RasterizerState = scissorState;

            Rectangle previousScissorRect = b.GraphicsDevice.ScissorRectangle;
            b.GraphicsDevice.ScissorRectangle = new Rectangle(ScrollArea.X, ScrollArea.Y, ScrollArea.Width, ScrollArea.Height);

            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, scissorState, null);

            // Draw scrollable content
            int currentY = yPositionOnScreen + 20 - scrollOffset;
            foreach (var category in categories)
            {
                category.Draw(b, xPositionOnScreen + 20, currentY, width - 40);
                currentY += category.GetTotalHeight(HeaderHeight, ItemHeight);
            }

            b.End();
            b.GraphicsDevice.ScissorRectangle = previousScissorRect;
            b.GraphicsDevice.RasterizerState = originalState;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null);

            // Draw the scroll bar (if necessary)
            DrawScrollBar(b);

            // Draw the mouse cursor
            drawMouse(b);
        }
        
        public override void receiveScrollWheelAction(int direction)
        {
            int totalContentHeight = CalculateTotalContentHeight();
            if (totalContentHeight > height - 40)
            {
                scrollOffset -= direction > 0 ? ScrollSpeed : -ScrollSpeed;
                scrollOffset = MathHelper.Clamp(scrollOffset, 0, totalContentHeight - height + 40);
            }
        }
        
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // Handle click events on the categories
            int currentY = yPositionOnScreen + 20 - scrollOffset;
            foreach (var category in categories)
            {
                if (category.HandleLeftClick(x, y, xPositionOnScreen + 20, currentY, width - 40, HeaderHeight))
                {
                    if (playSound) Game1.playSound("coin");
                    break;
                }
                currentY += category.GetTotalHeight(HeaderHeight, ItemHeight);
            }
        }

        private int CalculateTotalContentHeight()
        {
            int totalHeight = 0;
            foreach (var category in categories)
            {
                totalHeight += category.GetTotalHeight(HeaderHeight, ItemHeight);
            }
            return totalHeight;
        }

        private void DrawScrollBar(SpriteBatch b)
        {
            int totalContentHeight = CalculateTotalContentHeight();
            if (totalContentHeight <= height - 40)
                return;

            int scrollBarHeight = (int)((float)(height - 40) / totalContentHeight * (height - 40));
            int scrollBarY = yPositionOnScreen + (int)((float)scrollOffset / totalContentHeight * (height - 40));

            IClickableMenu.drawTextureBox(b, xPositionOnScreen + width - 20, scrollBarY, 16, scrollBarHeight, Color.Gray);
        }
    }

    // Category Class with Expandable Logic
    public class ExpandableCategory
    {
        public string Name { get; }
        public List<ParsedItemData> Items { get; }
        private bool isExpanded;

        public ExpandableCategory(string name, List<ParsedItemData> items)
        {
            Name = name;
            Items = items;
            isExpanded = false;
        }

        public void Draw(SpriteBatch b, int x, int y, int width)
        {
            // Draw header box
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, 50, Color.White, 1f, false);

            // Draw header text
            SpriteText.drawString(b, Name, x + 10, y + 10);

            if (isExpanded)
            {
                int currentY = y + 50;
                foreach (var item in Items)
                {
                    // Calculate the source rectangle for the sprite
                    Rectangle sourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.SpriteIndex, 16, 16);

                    // Draw the item sprite
                    b.Draw(Game1.objectSpriteSheet, new Vector2(x + 20, currentY), sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.9f);

                    // Optional: Draw the item's name beside the sprite (if needed)
                    b.DrawString(Game1.dialogueFont, item.DisplayName, new Vector2(x + 60, currentY), Color.Black);
                    currentY += 40;
                }
            }
        }

        public int GetTotalHeight(int headerHeight, int itemHeight)
        {
            return isExpanded ? headerHeight + Items.Count * itemHeight : headerHeight;
        }

        public bool HandleLeftClick(int mouseX, int mouseY, int x, int y, int width, int headerHeight)
        {
            Rectangle headerBounds = new Rectangle(x, y, width, headerHeight);
            if (headerBounds.Contains(mouseX, mouseY))
            {
                isExpanded = !isExpanded;
                return true;
            }
            return false;
        }
    }
}