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
        public ModConfig Config;

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
            Config = helper.ReadConfig<ModConfig>();
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayEnding += OnDayEnding;

            RegisterDebugCommands(helper);
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            _phoenixHealingAuraUsed = false;
            _phoenixPhoenixDownUsed = false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            RegisterConfigMenu();

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
                    CheckIfCactuarEssenceIsAttached();

                    // Shards
                    CheckIfCarbuncleShardIsAttached();
                    CheckIfIfritShardIsAttached();
                    CheckIfKirinShardIsAttached();
                    CheckIfLeviathanShardIsAttached();
                    CheckIfPhoenixShardIsAttached();
                    CheckIfRamuhShardIsAttached();
                    CheckIfShivaShardIsAttached();
                    CheckIfTitanShardIsAttached();
                    CheckIfCactuarShardIsAttached();

                    // Souls
                    CheckIfCarbuncleSoulIsAttached();
                    CheckIfIfritSoulIsAttached();
                    CheckIfKirinSoulIsAttached();
                    CheckIfLeviathanSoulIsAttached();
                    CheckIfPhoenixSoulIsAttached();
                    CheckIfRamuhSoulIsAttached();
                    CheckIfShivaSoulIsAttached();
                    CheckIfTitanSoulIsAttached();
                    CheckIfCactuarSoulIsAttached();

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
                Game1.player.health += (int)(Game1.player.maxHealth * Config.HealingAuraRecoveryRate);
                Game1.player.Stamina += (int)(Game1.player.MaxStamina * Config.HealingAuraRecoveryRate);
                Game1.currentLocation.playSound("healSound");
            }
        }

        private void PhoenixPhoenixDown()
        {
            if ((Game1.player.health <= 0 || Game1.player.stamina <= 0) && !_phoenixPhoenixDownUsed)
            {
                _phoenixPhoenixDownUsed = true;
                if (Game1.player.health <= (int)(Game1.player.maxHealth * Config.PhoenixDownHealthRecovery))
                {
                    Game1.player.health = (int) (Game1.player.maxHealth * Config.PhoenixDownHealthRecovery);
                }
                if (Game1.player.stamina <= (int)(Game1.player.MaxStamina * Config.PhoenixDownStaminaRecovery))
                {
                    Game1.player.Stamina = (int) (Game1.player.MaxStamina * Config.PhoenixDownStaminaRecovery);
                }
                Game1.currentLocation.playSound("healSound");
            }
        }

        private void LetterCallbackUpgradeAmphoraLevel2(Letter l)
        {
            UpgradeAmphora(ItemHelper.GetToolAmphoraId(), ItemHelper.GetToolAmphoraEchoesId(), 4);
            Game1.player.mailReceived.Add(l.Id);
        }

        private void LetterCallbackUpgradeAmphoraLevel3(Letter l)
        {
            UpgradeAmphora(ItemHelper.GetToolAmphoraEchoesId(), ItemHelper.GetToolAmphoraSpiritsId(), 10);
            Game1.player.mailReceived.Add(l.Id);
        }

        private static void UpgradeAmphora(string sourceToolId, string targetToolId, int newSlotCount)
        {
            for (var i = 0; i < Game1.player.Items.Count; i++)
            {
                if (Game1.player.Items[i] is not Tool oldTool || oldTool.ItemId != sourceToolId)
                    continue;

                var attachments = oldTool.attachments.ToList();
                for (int j = 0; j < oldTool.attachments.Length; j++)
                    oldTool.attachments[j] = null;

                var newTool = new GenericTool();
                newTool.ItemId = targetToolId;
                newTool.AttachmentSlotsCount = newSlotCount;
                Game1.player.Items[i] = newTool;

                foreach (var attachment in attachments)
                    ((Tool)Game1.player.Items[i]).attach(attachment);

                break;
            }
        }

        private void RegisterDebugCommands(IModHelper helper)
        {
            helper.ConsoleCommands.Add("ef_status", "Show Elemental Force mod status (amphora, buffs, daily flags).", (_, _) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }

                var hasAmphora = Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraId());
                var hasEchoes = Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraEchoesId());
                var hasSpirits = Game1.player.Items.ContainsId(ItemHelper.GetToolAmphoraSpiritsId());
                var amphoraLevel = hasSpirits ? "Spirits (Lv3)" : hasEchoes ? "Echoes (Lv2)" : hasAmphora ? "Base (Lv1)" : "None";

                Monitor.Log($"=== Elemental Force Status ===", LogLevel.Info);
                Monitor.Log($"Amphora: {amphoraLevel}", LogLevel.Info);
                Monitor.Log($"Healing Aura used today: {_phoenixHealingAuraUsed}", LogLevel.Info);
                Monitor.Log($"Phoenix Down used today: {_phoenixPhoenixDownUsed}", LogLevel.Info);
                Monitor.Log($"Regen Timer active: {_regenTimer != null}", LogLevel.Info);
                Monitor.Log($"Health: {Game1.player.health}/{Game1.player.maxHealth} | Stamina: {(int)Game1.player.stamina}/{Game1.player.MaxStamina}", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_buffs", "List all active Elemental Force buffs.", (_, _) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }

                var buffIds = new Dictionary<string, string>
                {
                    { BuffHelper.GetBuffHeatSpeedId(), "Heat Speed (Ifrit Essence)" },
                    { BuffHelper.GetBuffSavageIfritId(), "Savage Ifrit (Ifrit Shard)" },
                    { BuffHelper.GetBuffFireballId(), "Fireball (Ifrit Soul)" },
                    { BuffHelper.GetBuffSnowSpeedId(), "Snow Speed (Shiva Essence)" },
                    { BuffHelper.GetBuffBlizzardSlashId(), "Blizzard Slash (Shiva Shard)" },
                    { BuffHelper.GetBuffIceTombId(), "Ice Tomb (Shiva Soul)" },
                    { BuffHelper.GetBuffIronBodyId(), "Iron Body (Titan Essence)" },
                    { BuffHelper.GetBuffWrathPalmId(), "Wrath Palm (Titan Shard)" },
                    { BuffHelper.GetBuffEndlessStaminaId(), "Endless Stamina (Titan Soul)" },
                    { BuffHelper.GetBuffHeavyBodyId(), "Heavy Body (Leviathan Essence)" },
                    { BuffHelper.GetBuffRainWishId(), "Rain Wish (Leviathan Shard)" },
                    { BuffHelper.GetBuffDragonScaleId(), "Dragon Scale (Leviathan Soul)" },
                    { BuffHelper.GetBuffSunnySpeedId(), "Sunny Speed (Carbuncle Essence)" },
                    { BuffHelper.GetBuffCompanionProtectionId(), "Companion Protection (Carbuncle Shard)" },
                    { BuffHelper.GetBuffMirrorReflectionId(), "Mirror Reflection (Carbuncle Soul)" },
                    { BuffHelper.GetBuffImmunityBandId(), "Immunity Band (Kirin Essence)" },
                    { BuffHelper.GetBuffLuckDayId(), "Lucky Day (Kirin Shard)" },
                    { BuffHelper.GetBuffRegenBlessingId(), "Regen Blessing (Kirin Soul)" },
                    { BuffHelper.GetBuffFlashSpeedId(), "Flash Speed (Ramuh Essence)" },
                    { BuffHelper.GetBuffJoltingSwingId(), "Jolting Swing (Ramuh Shard)" },
                    { BuffHelper.GetBuffThunderCallerId(), "Thunder Caller (Ramuh Soul)" },
                    { BuffHelper.GetBuffHealingAuraId(), "Healing Aura (Phoenix Essence)" },
                    { BuffHelper.GetBuffExplosionId(), "Explosion (Phoenix Shard)" },
                    { BuffHelper.GetBuffPhoenixDownId(), "Phoenix Down (Phoenix Soul)" },
                    { BuffHelper.GetBuffWarySpeedAuxId(), "Wary Speed Aux (Cactuar Essence)" },
                    { BuffHelper.GetBuffWarySpeedId(), "Wary Speed (Cactuar Essence - Active)" },
                    { BuffHelper.GetBuffNeedlepointStrikesId(), "Needlepoint Strikes (Cactuar Shard)" },
                    { BuffHelper.GetBuffInitiativeMasterId(), "Initiative Master (Cactuar Soul)" },
                };

                Monitor.Log("=== Active Elemental Buffs ===", LogLevel.Info);
                var count = 0;
                foreach (var (id, name) in buffIds)
                {
                    if (Game1.player.buffs.IsApplied(id))
                    {
                        Monitor.Log($"  [ON] {name}", LogLevel.Info);
                        count++;
                    }
                }
                if (count == 0) Monitor.Log("  No elemental buffs active.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_give", "Give an elemental item. Usage: ef_give <elemental> <type>\n  elemental: ifrit, shiva, titan, leviathan, carbuncle, kirin, ramuh, phoenix, cactuar\n  type: essence, shard, soul", (_, args) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }
                if (args.Length < 2) { Monitor.Log("Usage: ef_give <elemental> <type>", LogLevel.Warn); return; }

                var elemental = args[0].ToLower();
                var type = args[1].ToLower();

                if (!Enum.TryParse<ElementalEnum>(elemental, true, out var elementalEnum) || elementalEnum == ElementalEnum.None)
                {
                    Monitor.Log($"Unknown elemental: {elemental}. Valid: ifrit, shiva, titan, leviathan, carbuncle, kirin, ramuh, phoenix, cactuar", LogLevel.Warn);
                    return;
                }

                var suffix = type switch
                {
                    "essence" => "Essence",
                    "shard" => "Shard",
                    "soul" => "Soul",
                    _ => ""
                };
                if (suffix == "")
                {
                    Monitor.Log($"Unknown type: {type}. Valid: essence, shard, soul", LogLevel.Warn);
                    return;
                }

                var itemId = $"{ModManifest.UniqueID}.CP_{elementalEnum}{suffix}";
                Game1.player.addItemByMenuIfNecessary(new Object(itemId, 1));
                Monitor.Log($"Added {elementalEnum} {suffix} to inventory.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_amphora", "Give or upgrade amphora. Usage: ef_amphora <level>\n  level: 1 (base), 2 (echoes), 3 (spirits)", (_, args) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }

                var level = args.Length > 0 && int.TryParse(args[0], out var l) ? l : 1;
                var (toolId, slots) = level switch
                {
                    3 => (ItemHelper.GetToolAmphoraSpiritsId(), 10),
                    2 => (ItemHelper.GetToolAmphoraEchoesId(), 4),
                    _ => (ItemHelper.GetToolAmphoraId(), 2)
                };

                var tool = new GenericTool { ItemId = toolId, AttachmentSlotsCount = slots };
                Game1.player.addItemByMenuIfNecessary(tool);
                Monitor.Log($"Added Amphora Lv{level} ({slots} slots) to inventory.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_reset", "Reset daily flags (Healing Aura, Phoenix Down).", (_, _) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }
                _phoenixHealingAuraUsed = false;
                _phoenixPhoenixDownUsed = false;
                Monitor.Log("Daily flags reset. Healing Aura and Phoenix Down can trigger again.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_heal", "Fully restore health and stamina.", (_, _) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }
                Game1.player.health = Game1.player.maxHealth;
                Game1.player.stamina = Game1.player.MaxStamina;
                Monitor.Log($"Health and stamina fully restored.", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_hurt", "Set health/stamina to a low value for testing. Usage: ef_hurt [percent]\n  percent: 1-99 (default: 10)", (_, args) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }
                var percent = args.Length > 0 && int.TryParse(args[0], out var p) ? Math.Clamp(p, 1, 99) : 10;
                Game1.player.health = (int)(Game1.player.maxHealth * percent / 100f);
                Game1.player.stamina = (int)(Game1.player.MaxStamina * percent / 100f);
                Monitor.Log($"Health and stamina set to {percent}% ({Game1.player.health}/{Game1.player.maxHealth} HP, {(int)Game1.player.stamina}/{Game1.player.MaxStamina} SP).", LogLevel.Info);
            });

            helper.ConsoleCommands.Add("ef_weather", "Set tomorrow's weather. Usage: ef_weather <type>\n  type: sun, rain, storm, snow, wind", (_, args) =>
            {
                if (!Context.IsWorldReady) { Monitor.Log("Save not loaded.", LogLevel.Warn); return; }
                if (args.Length < 1) { Monitor.Log("Usage: ef_weather <sun|rain|storm|snow|wind>", LogLevel.Warn); return; }

                var weather = args[0].ToLower() switch
                {
                    "sun" => "Sun",
                    "rain" => "Rain",
                    "storm" => "Storm",
                    "snow" => "Snow",
                    "wind" => "Wind",
                    _ => ""
                };
                if (weather == "")
                {
                    Monitor.Log($"Unknown weather: {args[0]}. Valid: sun, rain, storm, snow, wind", LogLevel.Warn);
                    return;
                }

                var locationWeather = Game1.netWorldState.Value.GetWeatherForLocation("Default");
                locationWeather.WeatherForTomorrow = weather;
                Monitor.Log($"Tomorrow's weather set to: {weather}", LogLevel.Info);
            });
        }

        private void RegisterConfigMenu()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // Ifrit
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Ifrit");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Fireball Chance (%)",
                tooltip: () => "Chance to cast fireball on melee attack",
                getValue: () => Config.FireballChancePercent, setValue: v => Config.FireballChancePercent = v,
                min: 10, max: 100, interval: 5);
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Fireball Damage",
                getValue: () => Config.FireballDamage, setValue: v => Config.FireballDamage = v,
                min: 10, max: 100, interval: 5);

            // Shiva
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Shiva");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Ice Tomb Freeze Chance (%)",
                tooltip: () => "Chance to freeze enemy on hit",
                getValue: () => Config.IceTombFreezeChancePercent, setValue: v => Config.IceTombFreezeChancePercent = v,
                min: 10, max: 100, interval: 5);

            // Carbuncle
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Carbuncle");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Companion Protection Chance (%)",
                tooltip: () => "Chance to avoid damage",
                getValue: () => Config.CompanionProtectionChancePercent, setValue: v => Config.CompanionProtectionChancePercent = v,
                min: 10, max: 80, interval: 5);

            // Kirin
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Kirin");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Regen Blessing Rate (%)",
                tooltip: () => "Percentage of max health/stamina restored per tick",
                getValue: () => (int)(Config.RegenBlessingRate * 100), setValue: v => Config.RegenBlessingRate = v / 100f,
                min: 1, max: 10);

            // Phoenix
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Phoenix");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Healing Aura Recovery (%)",
                tooltip: () => "Percentage of max health/stamina restored",
                getValue: () => (int)(Config.HealingAuraRecoveryRate * 100), setValue: v => Config.HealingAuraRecoveryRate = v / 100f,
                min: 10, max: 80, interval: 5);
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Phoenix Down Health (%)",
                tooltip: () => "Health percentage restored on revival",
                getValue: () => (int)(Config.PhoenixDownHealthRecovery * 100), setValue: v => Config.PhoenixDownHealthRecovery = v / 100f,
                min: 10, max: 80, interval: 5);
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Phoenix Down Stamina (%)",
                tooltip: () => "Stamina percentage restored on revival",
                getValue: () => (int)(Config.PhoenixDownStaminaRecovery * 100), setValue: v => Config.PhoenixDownStaminaRecovery = v / 100f,
                min: 10, max: 80, interval: 5);
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Explosion Damage",
                getValue: () => Config.ExplosionDamage, setValue: v => Config.ExplosionDamage = v,
                min: 5, max: 100, interval: 5);

            // Cactuar
            configMenu.AddSectionTitle(mod: ModManifest, text: () => "Cactuar");
            configMenu.AddNumberOption(mod: ModManifest, name: () => "Wary Speed Duration (ms)",
                tooltip: () => "Duration of speed boost when hit",
                getValue: () => Config.WarySpeedDurationMs, setValue: v => Config.WarySpeedDurationMs = v,
                min: 1000, max: 15000, interval: 1000);
        }

    }
}

public interface IGenericModConfigMenuApi
{
    void Register(IManifest mod, Action reset, Action save);
    void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);
    void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);
    void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string>? tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string>? formatValue = null, string? fieldId = null);
}
