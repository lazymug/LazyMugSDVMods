using ElementalForce.Elemental_Force_Code;
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
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ElementalForce
{
    public sealed class ModEntry : Mod
    {

        // TODO: patch the method Farmer.DoDamage() for Shiva buff effect
        private bool _checkIfLocationHasChanged = false;
        private bool _checkIfEquipmentHasChanged = false;
        
        // Variables for Buffs states
        private bool _phoenixHealingAuraUsed = false;
        private bool _phoenixPhoenixDownUsed = false;

        private RegenBlessingTimer? _regenTimer;
        private int _regenIntervalDefault = 42000;
        private int _regenIntervalSkullCavern = 27000;
        
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
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }
        
        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            _phoenixHealingAuraUsed = false;
            _phoenixPhoenixDownUsed = false;
        }
        
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
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
                    id: LettersHelper.GaiaAmphoraShardId,
                    text: LettersHelper.GetText(LettersHelper.GaiaAmphoraShardId),
                    condition: l=>LettersHelper.GetGaiaAmphoraShardCondition(),
                    callback: LetterCallbackUpgradeAmphoraLevel2
                )
            );
            MailRepository.SaveLetter(
                new Letter(
                    id: LettersHelper.GaiaAmphoraSoulId,
                    text: LettersHelper.GetText(LettersHelper.GaiaAmphoraSoulId),
                    condition: l=>LettersHelper.GetGaiaAmphoraSoulCondition(),
                    callback: LetterCallbackUpgradeAmphoraLevel3
                )
            );
        }
        
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            var isAmphora = Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId()) ||
                            Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraEchoesId()) ||
                            Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraSpiritsId());
            if (isAmphora)
            {
                // Phoenix Essence logic
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffHealingAuraId()) && !_phoenixHealingAuraUsed)
                {
                    PhoenixHealingAura();
                }
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffPhoenixDownId()) && !_phoenixPhoenixDownUsed)
                {
                    PhoenixPhoenixDown();
                }
                // Kirin Soul logic
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffRegenBlessingId()))
                {
                    KirinRegenBlessing();
                    if (_checkIfLocationHasChanged)
                    {
                        _regenTimer?.UpdateInterval(Game1.player.currentLocation.Name == "SkullCave"
                            ? _regenIntervalSkullCavern
                            : _regenIntervalDefault);
                    }
                }
                if (_checkIfLocationHasChanged || _checkIfEquipmentHasChanged)
                {
                    // Essences
                    CheckIfIfritEssenceIsAttached();
                    CheckIfShivaEssenceIsAttached();
                    CheckIfTitanEssenceIsAttached();
                    CheckIfCarbuncleEssenceIsAttached();
                    CheckIfKirinEssenceIsAttached();
                    CheckIfLeviathanEssenceIsAttached();
                    CheckIfPhoenixEssenceIsAttached();
                    CheckIfRamuhEssenceIsAttached();
                    
                    // Shards
                    CheckIfCarbuncleShardIsAttached();
                    CheckIfIfritShardIsAttached();
                    CheckIfKirinShardIsAttached();
                    CheckIfLeviathanShardIsAttached();
                    CheckIfPhoenixShardIsAttached();
                    CheckIfRamuhShardIsAttached();
                    CheckIfShivaShardIsAttached();
                    CheckIfTitanShardIsAttached();
                    
                    // Souls
                    CheckIfCarbuncleSoulIsAttached();
                    CheckIfIfritSoulIsAttached();
                    CheckIfKirinSoulIsAttached();
                    CheckIfLeviathanSoulIsAttached();
                    CheckIfPhoenixSoulIsAttached();
                    CheckIfRamuhSoulIsAttached();
                    CheckIfShivaSoulIsAttached();
                    CheckIfTitanSoulIsAttached();
                    
                    // Disable checks
                    _checkIfEquipmentHasChanged = false;
                    _checkIfLocationHasChanged = false;
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

        private void CheckIfIfritShardIsAttached()
        {
            if (ToolAttachmentHelper.IsIfritShardEquipped())
            {
                Game1.player.applyBuff(new SavageIfritBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffSavageIfritId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffSavageIfritId());
            }
        }
        
        private void CheckIfIfritSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsIfritSoulEquipped())
            {
                Game1.player.applyBuff(new FireballBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffFireballId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffFireballId());
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
        
        private void CheckIfShivaShardIsAttached()
        {
            if (ToolAttachmentHelper.IsShivaShardEquipped())
            {
                Game1.player.applyBuff(new BlizzardSlashBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffBlizzardSlashId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffBlizzardSlashId());
            }
        }
        
        private void CheckIfShivaSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsShivaSoulEquipped())
            {
                Game1.player.applyBuff(new IceTombBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffIceTombId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffIceTombId());
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
        
        private void CheckIfTitanShardIsAttached()
        {
            if (ToolAttachmentHelper.IsTitanShardEquipped())
            {
                Game1.player.applyBuff(new WrathPalmBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffWrathPalmId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffWrathPalmId());
            }
        }
        
        private void CheckIfTitanSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsTitanSoulEquipped())
            {
                Game1.player.applyBuff(new EndlessStaminaBuff(Game1.player.maxStamina.Value));
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffEndlessStaminaId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffEndlessStaminaId());
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
        
        private void CheckIfLeviathanShardIsAttached()
        {
            if (ToolAttachmentHelper.IsLeviathanShardEquipped())
            {
                Game1.player.applyBuff(new RainWishBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffRainWishId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffRainWishId());
            }
        }
        
        private void CheckIfLeviathanSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsLeviathanSoulEquipped())
            {
                Game1.player.applyBuff(new DragonScaleBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffDragonScaleId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffDragonScaleId());
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

        private void CheckIfCarbuncleShardIsAttached()
        {
            if (ToolAttachmentHelper.IsCarbuncleShardEquipped())
            {
                Game1.player.applyBuff(new CompanionProtectionBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffCompanionProtectionId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffCompanionProtectionId());
            }
        }
        
        private void CheckIfCarbuncleSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsCarbuncleSoulEquipped())
            {
                Game1.player.applyBuff(new MirrorReflectionBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffMirrorReflectionId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffMirrorReflectionId());
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

        private void CheckIfKirinShardIsAttached()
        {
            if (ToolAttachmentHelper.IsKirinShardEquipped())
            {
                Game1.player.applyBuff(new LuckyDayBuff(Game1.player.LuckLevel));
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffLuckDayId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffLuckDayId());
            }
        }

        private void CheckIfKirinSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsKirinSoulEquipped())
            {
                Game1.player.applyBuff(new RegenBlessingBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffRegenBlessingId()))
                {
                    Game1.player.buffs.Remove(BuffHelper.GetBuffRegenBlessingId());
                    _regenTimer = null;
                }
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
        
        private void CheckIfRamuhShardIsAttached()
        {
            if (ToolAttachmentHelper.IsRamuhShardEquipped())
            {
                Game1.player.applyBuff(new JoltingSwingBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffJoltingSwingId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffJoltingSwingId());
            }
        }
        
        private void CheckIfRamuhSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsRamuhSoulEquipped())
            {
                Game1.player.applyBuff(new ThunderCallerBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffThunderCallerId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffThunderCallerId());
            }
        }
        
        private void CheckIfPhoenixEssenceIsAttached()
        {
            if (ToolAttachmentHelper.IsPhoenixEssenceEquipped())
            {
                Game1.player.applyBuff(new HealingAuraBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffHealingAuraId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffHealingAuraId());
            }
        }
        
        private void CheckIfPhoenixShardIsAttached()
        {
            if (ToolAttachmentHelper.IsPhoenixShardEquipped())
            {
                Game1.player.applyBuff(new ExplosionBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffExplosionId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffExplosionId());
            }
        }
        
        private void CheckIfPhoenixSoulIsAttached()
        {
            if (ToolAttachmentHelper.IsPhoenixSoulEquipped())
            {
                Game1.player.applyBuff(new PhoenixDownBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffPhoenixDownId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffPhoenixDownId());
            }
        }

        private void KirinRegenBlessing()
        {
            if (_regenTimer != null)
            {
                return;
            }

            _regenTimer = new RegenBlessingTimer();
        }

        private void PhoenixHealingAura()
        {
            var isHealthLow = Game1.player.health < Game1.player.maxHealth * 0.3;
            var isStaminaLow = Game1.player.stamina < Game1.player.MaxStamina * 0.2;
            if ((isHealthLow || isStaminaLow) && !_phoenixHealingAuraUsed)
            {
                _phoenixHealingAuraUsed = true;
                Game1.player.health += Game1.player.maxHealth * 4 / 10;
                Game1.player.Stamina += Game1.player.MaxStamina * 4 / 10;
                Game1.currentLocation.playSound("healSound");
            }
        }
        
        private void PhoenixPhoenixDown()
        {
            var isHealthLow = Game1.player.health <= 0;
            var isStaminaLow = Game1.player.stamina <= 0;
            if ((Game1.player.health <= 0 || Game1.player.stamina <= 0) && !_phoenixPhoenixDownUsed)
            {
                _phoenixPhoenixDownUsed = true;
                if (Game1.player.health <= (int)(Game1.player.maxHealth * 0.3))
                {
                    Game1.player.health = (int) (Game1.player.maxHealth * 0.3);
                }
                if (Game1.player.stamina <= (int)(Game1.player.MaxStamina * 0.2))
                {
                    Game1.player.Stamina = (int) (Game1.player.MaxStamina * 0.2);
                }
                Game1.currentLocation.playSound("healSound");
            }
        }
        
        private void LetterCallbackUpgradeAmphoraLevel2(Letter l)
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] == null || Game1.player.Items[i] is not Tool) continue;
                if (((Tool)Game1.player.Items[i]).ItemId != ItemHelper.GetToolAmphoraId()) continue;
                var attachments = ((Tool) Game1.player.Items[i]).attachments;
                Game1.player.Items[i] = new GenericTool();
                Game1.player.Items[i].ItemId = ItemHelper.GetToolAmphoraEchoesId();
                foreach (var attachment in attachments)
                {
                    ((Tool)Game1.player.Items[i]).attach(attachment);
                }
                break;
            }

            Game1.player.mailReceived.Add(l.Id);
        }
        
        private void LetterCallbackUpgradeAmphoraLevel3(Letter l)
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] == null || Game1.player.Items[i] is not Tool) continue;
                if (((Tool)Game1.player.Items[i]).ItemId != ItemHelper.GetToolAmphoraEchoesId()) continue;
                var attachments = ((Tool) Game1.player.Items[i]).attachments;
                Game1.player.Items[i] = new GenericTool();
                Game1.player.Items[i].ItemId = ItemHelper.GetToolAmphoraSpiritsId();
                foreach (var attachment in attachments)
                {
                    ((Tool)Game1.player.Items[i]).attach(attachment);
                }
                break;
            }

            Game1.player.mailReceived.Add(l.Id);
        }
        
    }
}