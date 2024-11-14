using System;
using GenericModConfigMenu;
using INeedFunds.Mail;
using INeedFunds.Model;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using SpaceShared;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace INeedFunds
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static ModEntry instance;
        
        private SaveModel _data = new SaveModel();
        
        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu != null)
            {
                var config = Helper.ReadConfig<ModConfig>();
                configMenu.Register(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
                configMenu.AddNumberOption(ModManifest, () => config.LoanAmount, (int val) => config.LoanAmount = val, () => "Loan Amount", () => "The initial amount of the loan.", 10000, 100000);
                configMenu.AddNumberOption(ModManifest, () => (int)(config.LoanRate * 1000), (int val) => config.LoanRate = ((double) val) / 1000, () => "Loan rate", () => "The rate used to calculate the amount to be paid weekly. (This value will be divided by 1000)", 1000, 1200);
            }
            
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
        }
        
        public SaveModel Data => _data;


        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            
            
        }
        
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            
        }
        
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Helper.Data.WriteSaveData("inf_data", _data);
        }
        
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            _data = Helper.Data.ReadSaveData<SaveModel>("inf_data") ?? new SaveModel();
            if (_data.IsSubscribed)
            {
                return;
            }
            string content = Helper.Translation.Get("inf.mail.content");
            content = string.Format(content, Game1.player.Name, _data.LoanAmount, _data.LoanRate);
            MailRepository.SaveLetter(
                new Letter(
                    Ids.SubscribeAgriculturalFundsId
                    ,content
                    ,(l)=>!Game1.player.mailReceived.Contains(l.Id)
                    ,(l)=>Game1.player.mailReceived.Add(l.Id)
                )
            );

            /*
            string content = Helper.Translation.Get("inf.mail.content");
            content = string.Format(content, Game1.player.Name, _data.LoanAmount, _data.LoanRate);

            // Create an instance of AgriculturalFundMail
            var agriculturalFundMail = new AgriculturalFundMail(content, Helper);

            // Add the mail to the mailbox
            Game1.mailbox.Add(agriculturalFundMail.mailMessage);
            */
        }
    }
}