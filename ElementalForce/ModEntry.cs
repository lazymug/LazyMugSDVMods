using ElementalForce.Elemental_Force_Code;
using ElementalForce.Elemental_Force_Code.buffs.cactuar;
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
        private bool _checkIfLocationHasChanged = false;
        private bool _checkIfEquipmentHasChanged = false;

        // Variables for Buffs states
        private bool _phoenixHealingAuraUsed = false;
        private bool _phoenixPhoenixDownUsed = false;

        private RegenBlessingTimer? _regenTimer;

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
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
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
                    _regenTimer?.Tick();
                    if (_checkIfLocationHasChanged)
                    {
                        _regenTimer?.UpdateInterval(Game1.player.currentLocation.Name == "SkullCave"
                            ? BuffConstants.RegenIntervalSkullCavernMs
                            : BuffConstants.RegenIntervalDefaultMs);
                    }
                }
                if (_checkIfLocationHasChanged || _checkIfEquipmentHasChanged)
                {
                    var location = Game1.player.currentLocation;
                    var locationName = location?.Name ?? "";

                    // Essences
                    CheckIfIfritEssenceIsAttached(locationName);
                    CheckIfShivaEssenceIsAttached(locationName);
                    CheckIfTitanEssenceIsAttached();
                    CheckIfCarbuncleEssenceIsAttached(location);
                    CheckIfKirinEssenceIsAttached();
                    CheckIfLeviathanEssenceIsAttached();
                    CheckIfPhoenixEssenceIsAttached();
                    CheckIfRamuhEssenceIsAttached(location);

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

        private static void ApplyOrRemoveBuff(bool isEquipped, string buffId, Func<Buff> createBuff, Action? onRemove = null)
        {
            if (isEquipped)
            {
                Game1.player.applyBuff(createBuff());
            }
            else
            {
                if (Game1.player.buffs.IsApplied(buffId))
                {
                    Game1.player.buffs.Remove(buffId);
                    onRemove?.Invoke();
                }
            }
        }

        private void CheckIfIfritEssenceIsAttached(string locationName)
        {
            var isEquipped = ToolAttachmentHelper.IsIfritEssenceEquipped() && locationName == "Desert";
            if (isEquipped)
            {
                // need to check shiva is attached before so it won't sum the buffs
                if (Game1.player.buffs.IsApplied(BuffHelper.GetBuffSnowSpeedId()))
                    Game1.player.buffs.Remove(BuffHelper.GetBuffSnowSpeedId());
            }
            ApplyOrRemoveBuff(isEquipped, BuffHelper.GetBuffHeatSpeedId(),
                () => new HeatSpeedBuff(currentSpeed: Game1.player.buffs.Speed));
        }

        private void CheckIfIfritShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsIfritShardEquipped(), BuffHelper.GetBuffSavageIfritId(),
                () => new SavageIfritBuff());
        }

        private void CheckIfIfritSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsIfritSoulEquipped(), BuffHelper.GetBuffFireballId(),
                () => new FireballBuff());
        }

        private void CheckIfShivaEssenceIsAttached(string locationName)
        {
            ApplyOrRemoveBuff(
                ToolAttachmentHelper.IsShivaEssenceEquipped() && Game1.IsWinter && locationName != "Desert",
                BuffHelper.GetBuffSnowSpeedId(),
                () => new SnowSpeedBuff(playerCurrentSpeed: Game1.player.buffs.Speed));
        }

        private void CheckIfShivaShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsShivaShardEquipped(), BuffHelper.GetBuffBlizzardSlashId(),
                () => new BlizzardSlashBuff());
        }

        private void CheckIfShivaSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsShivaSoulEquipped(), BuffHelper.GetBuffIceTombId(),
                () => new IceTombBuff());
        }

        private void CheckIfTitanEssenceIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsTitanEssenceEquipped(), BuffHelper.GetBuffIronBodyId(),
                () => new IronBodyBuff(currentDefense: Game1.player.buffs.Defense));
        }

        private void CheckIfTitanShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsTitanShardEquipped(), BuffHelper.GetBuffWrathPalmId(),
                () => new WrathPalmBuff());
        }

        private void CheckIfTitanSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsTitanSoulEquipped(), BuffHelper.GetBuffEndlessStaminaId(),
                () => new EndlessStaminaBuff(Game1.player.maxStamina.Value));
        }

        private void CheckIfLeviathanEssenceIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsLeviathanEssenceEquipped(), BuffHelper.GetBuffHeavyBodyId(),
                () => new HeavyBodyBuff());
        }

        private void CheckIfLeviathanShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsLeviathanShardEquipped(), BuffHelper.GetBuffRainWishId(),
                () => new RainWishBuff());
        }

        private void CheckIfLeviathanSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsLeviathanSoulEquipped(), BuffHelper.GetBuffDragonScaleId(),
                () => new DragonScaleBuff());
        }

        private void CheckIfCarbuncleEssenceIsAttached(GameLocation? location)
        {
            var isSunny = location != null && !Game1.IsRainingHere(location) && !Game1.IsLightningHere(location) && !Game1.IsGreenRainingHere(location);
            ApplyOrRemoveBuff(
                ToolAttachmentHelper.IsCarbuncleEssenceEquipped() && isSunny,
                BuffHelper.GetBuffSunnySpeedId(),
                () => new SunnySpeedBuff(Game1.player.buffs.Speed));
        }

        private void CheckIfCarbuncleShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsCarbuncleShardEquipped(), BuffHelper.GetBuffCompanionProtectionId(),
                () => new CompanionProtectionBuff());
        }

        private void CheckIfCarbuncleSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsCarbuncleSoulEquipped(), BuffHelper.GetBuffMirrorReflectionId(),
                () => new MirrorReflectionBuff());
        }

        private void CheckIfKirinEssenceIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsKirinEssenceEquipped(), BuffHelper.GetBuffImmunityBandId(),
                () => new ImmunityBandBuff(Game1.player.buffs.Immunity));
        }

        private void CheckIfKirinShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsKirinShardEquipped(), BuffHelper.GetBuffLuckDayId(),
                () => new LuckyDayBuff(Game1.player.LuckLevel));
        }

        private void CheckIfKirinSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsKirinSoulEquipped(), BuffHelper.GetBuffRegenBlessingId(),
                () => new RegenBlessingBuff(),
                onRemove: () => _regenTimer = null);
        }

        private void CheckIfRamuhEssenceIsAttached(GameLocation? location)
        {
            var isStorming = location != null && Game1.IsLightningHere(location);
            ApplyOrRemoveBuff(
                ToolAttachmentHelper.IsRamuhEssenceEquipped() && isStorming,
                BuffHelper.GetBuffFlashSpeedId(),
                () => new FlashSpeedBuff(Game1.player.buffs.Speed));
        }

        private void CheckIfRamuhShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsRamuhShardEquipped(), BuffHelper.GetBuffJoltingSwingId(),
                () => new JoltingSwingBuff());
        }

        private void CheckIfRamuhSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsRamuhSoulEquipped(), BuffHelper.GetBuffThunderCallerId(),
                () => new ThunderCallerBuff());
        }

        private void CheckIfPhoenixEssenceIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsPhoenixEssenceEquipped(), BuffHelper.GetBuffHealingAuraId(),
                () => new HealingAuraBuff());
        }

        private void CheckIfPhoenixShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsPhoenixShardEquipped(), BuffHelper.GetBuffExplosionId(),
                () => new ExplosionBuff());
        }

        private void CheckIfPhoenixSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsPhoenixSoulEquipped(), BuffHelper.GetBuffPhoenixDownId(),
                () => new PhoenixDownBuff());
        }

        private void CheckIfCactuarEssenceIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsCactuarEssenceEquipped(), BuffHelper.GetBuffWarySpeedAuxId(),
                () => new WarySpeedAuxBuff());
        }

        private void CheckIfCactuarShardIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsCactuarShardEquipped(), BuffHelper.GetBuffNeedlepointStrikesId(),
                () => new NeedlepointStrikesBuff());
        }

        private void CheckIfCactuarSoulIsAttached()
        {
            ApplyOrRemoveBuff(ToolAttachmentHelper.IsCactuarSoulEquipped(), BuffHelper.GetBuffInitiativeMasterId(),
                () => new InitiativeMasterBuff());
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
            var isHealthLow = Game1.player.health < Game1.player.maxHealth * BuffConstants.HealingAuraHealthThreshold;
            var isStaminaLow = Game1.player.stamina < Game1.player.MaxStamina * BuffConstants.HealingAuraStaminaThreshold;
            if ((isHealthLow || isStaminaLow) && !_phoenixHealingAuraUsed)
            {
                _phoenixHealingAuraUsed = true;
                Game1.player.health += (int)(Game1.player.maxHealth * BuffConstants.HealingAuraRecoveryRate);
                Game1.player.Stamina += (int)(Game1.player.MaxStamina * BuffConstants.HealingAuraRecoveryRate);
                Game1.currentLocation.playSound("healSound");
            }
        }

        private void PhoenixPhoenixDown()
        {
            if ((Game1.player.health <= 0 || Game1.player.stamina <= 0) && !_phoenixPhoenixDownUsed)
            {
                _phoenixPhoenixDownUsed = true;
                if (Game1.player.health <= (int)(Game1.player.maxHealth * BuffConstants.PhoenixDownHealthRecovery))
                {
                    Game1.player.health = (int) (Game1.player.maxHealth * BuffConstants.PhoenixDownHealthRecovery);
                }
                if (Game1.player.stamina <= (int)(Game1.player.MaxStamina * BuffConstants.PhoenixDownStaminaRecovery))
                {
                    Game1.player.Stamina = (int) (Game1.player.MaxStamina * BuffConstants.PhoenixDownStaminaRecovery);
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

                var oldTool = (Tool) Game1.player.Items[i];
                var attachments = oldTool.attachments.ToList();

                for (int j = 0; j < oldTool.attachments.Length; j++)
                {
                    oldTool.attachments[j] = null;
                }

                var newTool = new GenericTool();
                newTool.ItemId = ItemHelper.GetToolAmphoraEchoesId();
                newTool.AttachmentSlotsCount = 4;

                Game1.player.Items[i] = newTool;

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
