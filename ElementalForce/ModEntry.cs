using ElementalForce.Elemental_Force_Code.buffs.carbuncle;
using ElementalForce.Elemental_Force_Code.helpers;
using ElementalForce.Elemental_Force_Code.buffs.ifrit;
using ElementalForce.Elemental_Force_Code.buffs.kirin;
using ElementalForce.Elemental_Force_Code.buffs.leviathan;
using ElementalForce.Elemental_Force_Code.buffs.phoenix;
using ElementalForce.Elemental_Force_Code.buffs.ramuh;
using ElementalForce.Elemental_Force_Code.buffs.shiva;
using ElementalForce.Elemental_Force_Code.buffs.titan;
using HarmonyLib;
using MailFrameworkMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ElementalForce
{
    public sealed class ModEntry : Mod
    {

        // TODO: patch the method Farmer.DoDamage() for Shiva buff effect
        // TODO: create a class for storing BuffIds
        // TODO: create the i18n strings for Wizard event conversation
        private bool _checkIfLocationHasChanged = false;
        private bool _checkIfEquipmentHasChanged = false;
        
        // Variables for Buffs states
        private bool _phoenixHealingAuraUsed = false;
        // End Variables for Buffs states
        
        public static ModEntry Instance;

        public string GetModId()
        {
            return ModManifest.UniqueID;
        }

        public string GetTextTranslation(string id)
        {
            return Helper.Translation.Get(id);
        }
        
        public void OnCheckIfEquipmentHasChanged()
        {
            _checkIfEquipmentHasChanged = true;
        }

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            var harmony = new Harmony(ModManifest.UniqueID); // Initialize Harmony with your mod's unique ID
            harmony.PatchAll(); // Apply all patches in your assembly
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            // helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        private void CheckIfIfritEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsIfritEssenceEquipped() && Game1.player.currentLocation.Name == "Desert")
            {
                // need to check shiva is attached before so it won't sum the buffs
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffSnowSpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffSnowSpeedId());
                
                Game1.player.applyBuff(
                    new HeatSpeedBuff(
                        currentSpeed: Game1.player.buffs.Speed
                    )
                );
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffHeatSpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffHeatSpeedId());
            }
        }
        
        private void CheckIfShivaEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsShivaEssenceEquipped() && Game1.IsWinter && Game1.player.currentLocation.Name != "Desert")
            {
                Game1.player.applyBuff(
                    new SnowSpeedBuff(
                        playerCurrentSpeed: Game1.player.buffs.Speed
                    )
                );
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffSnowSpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffSnowSpeedId());
            }
        }
        
        private void CheckIfTitanEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsTitanEssenceEquipped())
            {
                Game1.player.applyBuff(
                    new IronBodyBuff(
                        currentDefense: Game1.player.buffs.Defense
                    )
                );
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffIronBodyId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffIronBodyId());
            }
        }
        
        private void CheckIfLeviathanEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsLeviathanEssenceEquipped())
            {
                Game1.player.applyBuff(new HeavyBodyBuff());                
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffHeavyBodyId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffHeavyBodyId());
            }
        }
        
        private void CheckIfCarbuncleEssenceIsAttached()
        {
            var location = Game1.player.currentLocation;
            var isSunny = !Game1.IsRainingHere(location) && !Game1.IsLightningHere(location) && !Game1.IsGreenRainingHere(location);
            if (ToolAttachmentHelper.IsCarbuncleEssenceEquipped() && isSunny)
            {
                Game1.player.applyBuff(new SunnySpeedBuff(Game1.player.buffs.Speed));
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffSunnySpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffSunnySpeedId());
            }
        }
        
        private void CheckIfKirinEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsKirinEssenceEquipped())
            {
                Game1.player.applyBuff(new ImmunityBandBuff(Game1.player.buffs.Immunity));
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffImmunityBandId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffImmunityBandId());
            }
        }
        
        private void CheckIfRamuhEssenceIsAttached()
        {
            var location = Game1.player.currentLocation;
            var isStorming = Game1.IsLightningHere(location);
            
            if (ToolAttachmentHelper.IsRamuhEssenceEquipped() && isStorming)
            {
                Game1.player.applyBuff(new FlashSpeedBuff(Game1.player.buffs.Speed));
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffFlashSpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffFlashSpeedId());
            }
        }
        
        private void CheckIfPhoenixEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsPhoenixEssenceEquipped() && !_phoenixHealingAuraUsed)
            {
                Game1.player.applyBuff(new HealingAuraBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffHealingAuraId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffHealingAuraId());
            }
        }
        
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // Check if the player is interacting with your tool
            if (!Context.IsWorldReady || e.Button != SButton.MouseRight || e.Button != SButton.MouseLeft)
                return;
            
            Farmer player = Game1.player;

            if (player.CursorSlotItem == null)
            {
                return;
            }
            
            Monitor.Log($"player cursorSlotItem is {player.CursorSlotItem.DisplayName}", LogLevel.Info);
        }
        
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            _phoenixHealingAuraUsed = false;
            // todo: activate power for changing weather
        }
        
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // todo: load all mails
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.MagnusInvitationId, 
                    text: LettersHelper.GetText(LettersHelper.MagnusInvitationId),
                    condition: (l)=>LettersHelper.GetMagnusInvitationCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    whichBG: 2
                )
                {
                    CustomTextColor = Color.White,
                }
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.GaiaWelcomeId,
                    text: LettersHelper.GetText(LettersHelper.GaiaWelcomeId),
                    condition: l=>LettersHelper.GetWelcomeGaiaCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceKirinId(), 1) }
                )
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.DwarfGiftId,
                    text: LettersHelper.GetText(LettersHelper.DwarfGiftId),
                    condition: l=>LettersHelper.GetDwarfGiftCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceTitanId(), 1) }
                )
            );
            /* Commented until I create the events
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.EmilyAquamarineId,
                    text: LettersHelper.GetText(LettersHelper.EmilyAquamarineId),
                    condition: l=>LettersHelper.GetEmilyAquamarineCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceShivaId(), 1) }
                )
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.PennyFoundId,
                    text: LettersHelper.GetText(LettersHelper.PennyFoundId),
                    condition: l=>LettersHelper.GetPennyFoundCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceCarbuncleId(), 1) }
                )
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.SandyQiRoomId,
                    text: LettersHelper.GetText(LettersHelper.SandyQiRoomId),
                    condition: l=>LettersHelper.GetSandyQiRoomCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceIfritId(), 1) }
                )
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.WillyFishingId,
                    text: LettersHelper.GetText(LettersHelper.WillyFishingId),
                    condition: l=>LettersHelper.GetWillyFishingCondition(),
                    callback: (l)=>Game1.player.mailReceived.Add(l.Id),
                    items: new List<Item>() { new StardewValley.Object(ItemHelper.GetObjectEssenceLeviathanId(), 1) }
                )
            );
            */
        }
        
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                var isAmphora = Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId());
                if (isAmphora)
                {
                    var isHealthLow = Game1.player.health < Game1.player.maxHealth * 0.3;
                    var isStaminaLow = Game1.player.stamina < Game1.player.MaxStamina * 0.2;
                    if ((isHealthLow || isStaminaLow) && ToolAttachmentHelper.IsPhoenixEssenceEquipped() && !_phoenixHealingAuraUsed)
                    {
                        _phoenixHealingAuraUsed = true;
                        Game1.player.health += Game1.player.maxHealth * 4 / 10;
                        Game1.player.Stamina += Game1.player.MaxStamina * 4 / 10;
                        // TODO: play a recovery sound and add animation
                    }
                    if (_checkIfLocationHasChanged || _checkIfEquipmentHasChanged)
                    {
                        CheckIfIfritEssenceIsAttached();
                        CheckIfShivaEssenceIsAttached();
                        CheckIfTitanEssenceIsAttached();
                        CheckIfCarbuncleEssenceIsAttached();
                        CheckIfKirinEssenceIsAttached();
                        CheckIfLeviathanEssenceIsAttached();
                        CheckIfPhoenixEssenceIsAttached();
                        CheckIfRamuhEssenceIsAttached();
                    
                        _checkIfEquipmentHasChanged = false;
                        _checkIfLocationHasChanged = false;
                    }
                }
                else
                {
                    RemoveAllAmphoraBuffs();
                }
            }
        }
        
        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            _checkIfEquipmentHasChanged = true;
        }
        
        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            _checkIfLocationHasChanged = true;
        }

        private void RemoveAllAmphoraBuffs()
        {
            
        }
        
    }
}