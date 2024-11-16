using INeedFunds.Mail;
using INeedFunds.Model;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace INeedFunds
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        private static Mod Instance { get; set; }
        
        private BankAccount account = new BankAccount();
        
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }
        
        public static string Translation(string key) => Instance.Helper.Translation.Get(key);
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Ensure the player is in Joja Mart and presses an interaction key
            if (!Context.IsWorldReady || Game1.currentLocation == null)
                return;

            if (Game1.currentLocation.Name.Equals("Town") && e.Button == SButton.MouseLeft)
            {
                var tile = e.Cursor.Tile;
            
                // Check if the player clicked on the ATM tile (example coordinates)
                if (tile.X == 38 && tile.Y == 58) // Adjust these coordinates for your ATM
                {
                    OpenATMMenu();
                }
            }
        }
        
        private void OpenATMMenu()
        {
            // Game1.activeClickableMenu = new ATMMenu(); // Replace ATMMenu with your custom menu class
            Monitor.Log("ATM Menu opened!", LogLevel.Info);
        }
        
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            LoadData();
        }
        
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            SaveData();
        }
        
        private void SaveData()
        {
            Helper.Data.WriteSaveData("BankAccount", account);
        }

        private void LoadData()
        {
            account = Helper.Data.ReadSaveData<BankAccount>("BankAccount") ?? new BankAccount();
        }
    }
}