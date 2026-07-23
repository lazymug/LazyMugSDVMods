using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using ValleyTriad.Data;
using ValleyTriad.Game;
using ValleyTriad.Models;
using ValleyTriad.Rendering;
using ValleyTriad.UI;

namespace ValleyTriad
{
    public class ModEntry : Mod
    {
        private ModConfig _config = null!;
        private readonly CardDatabase _cards = new();
        private CardRenderer _renderer = null!;
        private CollectionManager _collection = null!;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            _cards.Load(helper, Monitor);
            _renderer = new CardRenderer(helper, Monitor);
            _collection = new CollectionManager(helper, _cards);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += (_, _) => _collection.Load();
            helper.Events.Player.Warped += OnWarped;
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.ConsoleCommands.Add("vt_test", "Headless engine self-test.", (_, _) => SelfTest());
            helper.ConsoleCommands.Add("vt_open", "Open a practice match (uses your deck).", (_, _) => Practice());
            helper.ConsoleCommands.Add("vt_deck", "Open the collection / deck builder.", (_, _) => OpenDeck());
            helper.ConsoleCommands.Add("vt_grant", "Grant the starter pack (debug).", (_, _) => GrantStarter(true));
        }

        // ---------- config ----------
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcm == null) return;
            gmcm.Register(ModManifest, () => _config = new ModConfig(), () => Helper.WriteConfig(_config));
            string T(string k) => Helper.Translation.Get(k);
            gmcm.AddBoolOption(ModManifest, () => _config.EnableMod, v => _config.EnableMod = v, () => T("config.enablemod"));
            gmcm.AddSectionTitle(ModManifest, () => T("config.rules"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleSame, v => _config.RuleSame = v, () => T("config.same"));
            gmcm.AddBoolOption(ModManifest, () => _config.RulePlus, v => _config.RulePlus = v, () => T("config.plus"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleCombo, v => _config.RuleCombo = v, () => T("config.combo"));
            gmcm.AddBoolOption(ModManifest, () => _config.RuleElemental, v => _config.RuleElemental = v, () => T("config.elemental"));
            gmcm.AddNumberOption(ModManifest, () => _config.ElementalCells, v => _config.ElementalCells = v, () => T("config.elementalcells"), null, 0, 6, 1);
            gmcm.AddSectionTitle(ModManifest, () => T("config.stakes"));
            gmcm.AddTextOption(ModManifest, () => _config.Stakes.ToString(),
                v => _config.Stakes = Enum.TryParse<StakeMode>(v, out var m) ? m : StakeMode.Friendly,
                () => T("config.stakemode"), null, Enum.GetNames(typeof(StakeMode)));
        }

        // ---------- world integration ----------
        private static bool IsFriday() => Game1.dayOfMonth % 7 == 5;

        private void OnWarped(object? sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || !_config.EnableMod) return;
            if (e.NewLocation?.Name != "Saloon" || _collection.IntroSeen || !IsFriday()) return;
            if (e.NewLocation.characters.All(c => c.Name != "Abigail")) return;
            TriggerIntro();
        }

        private void TriggerIntro()
        {
            GrantStarter(false);
            _collection.IntroSeen = true;
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.starter"), HUDMessage.newQuest_type));
            var abby = Game1.getCharacterFromName("Abigail");
            if (abby != null)
            {
                abby.setNewDialogue(Helper.Translation.Get("intro.abigail"));
                Game1.drawDialogue(abby);
            }
        }

        private void GrantStarter(bool announce)
        {
            foreach (string id in Opponents.StarterPack) _collection.Add(id);
            if (announce) Game1.addHUDMessage(new HUDMessage("Starter pack granted.", HUDMessage.achievement_type));
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !_config.EnableMod) return;
            if (!e.Button.IsActionButton()) return;
            if (!_collection.IntroSeen || Game1.currentLocation?.Name != "Saloon" || !IsFriday()) return;

            // only when the hand is empty, so gifting/other interactions still work normally
            if (Game1.player.ActiveObject != null) return;
            var ch = Game1.currentLocation.isCharacterAtTile(e.Cursor.GrabTile);
            if (ch is NPC npc && npc.IsVillager)
            {
                Helper.Input.Suppress(e.Button);
                ShowChallenge(npc);
            }
        }

        private void ShowChallenge(NPC npc)
        {
            var answers = new[]
            {
                new Response("vt_yes", Helper.Translation.Get("challenge.yes")),
                new Response("vt_no", Helper.Translation.Get("challenge.no")),
            };
            Game1.currentLocation.createQuestionDialogue(
                Helper.Translation.Get("challenge.question", new { name = npc.displayName }),
                answers, (_, key) => { if (key == "vt_yes") StartMatch(npc); });
        }

        private void StartMatch(NPC npc)
        {
            _collection.EnsureDeck();
            if (!_collection.HasValidDeck())
            {
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.builddeck"), HUDMessage.error_type));
                return;
            }
            int hearts = Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
            var settings = MakeSettings(_collection.DeckCards(), Opponents.BuildDeck(_cards, hearts), Opponents.AiSkill(hearts), npc.displayName);
            settings.OnComplete = result => ApplyResult(result, npc);
            Game1.activeClickableMenu = new TriadMenu(_renderer, settings);
        }

        private void ApplyResult(MatchResult result, NPC npc)
        {
            switch (result.Outcome)
            {
                case Outcome.Win:
                    if (result.GainedCardId != null)
                    {
                        _collection.Add(result.GainedCardId);
                        var c = _cards.Get(result.GainedCardId);
                        Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.won", new { card = c?.NameKey != null ? Helper.Translation.Get(c.NameKey).ToString() : result.GainedCardId }), HUDMessage.achievement_type));
                    }
                    Game1.player.changeFriendship(80, npc);
                    break;
                case Outcome.Loss:
                    if (result.LostCardId != null)
                    {
                        _collection.Remove(result.LostCardId);
                        Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("hud.lost"), HUDMessage.error_type));
                    }
                    break;
            }
        }

        private MatchSettings MakeSettings(List<Card> player, List<Card> opp, int aiSkill, string oppName) => new()
        {
            PlayerDeck = player,
            OppDeck = opp,
            Stakes = _config.Stakes,
            SuddenDeathRounds = _config.SuddenDeathRounds,
            AiSkill = aiSkill,
            RuleSame = _config.RuleSame,
            RulePlus = _config.RulePlus,
            RuleCombo = _config.RuleCombo,
            RuleElemental = _config.RuleElemental,
            ElementalCells = _config.ElementalCells,
            OpponentDisplay = oppName,
        };

        // ---------- commands ----------
        private void OpenDeck()
        {
            if (!Context.IsWorldReady) { Monitor.Log("Load a save first.", LogLevel.Warn); return; }
            Game1.activeClickableMenu = new DeckMenu(_renderer, _collection, _cards);
        }

        private void Practice()
        {
            if (!Context.IsWorldReady) { Monitor.Log("Load a save first.", LogLevel.Warn); return; }
            _collection.EnsureDeck();
            List<Card> player = _collection.HasValidDeck() ? _collection.DeckCards() : _cards.All.Take(5).ToList();
            var opp = Opponents.BuildDeck(_cards, 5);
            var settings = MakeSettings(player, opp, 2, "Praticando");
            settings.OnComplete = r => Monitor.Log($"Practice result: {r.Outcome}", LogLevel.Info);
            Game1.activeClickableMenu = new TriadMenu(_renderer, settings);
        }

        private void SelfTest()
        {
            var board = new Board(_config.RuleSame, _config.RulePlus, _config.RuleCombo, _config.RuleElemental);
            Card? p2 = _cards.Get("parsnip"), p1 = _cards.Get("pumpkin");
            if (p1 == null || p2 == null) { Monitor.Log("Card data missing.", LogLevel.Error); return; }
            board.Place(p2, Owner.P2, 0, 1);
            var captured = board.Place(p1, Owner.P1, 1, 1);
            Monitor.Log($"Captured: {captured.Count} (expect 1). P1={board.Count(Owner.P1)} P2={board.Count(Owner.P2)} (expect 2/0).",
                captured.Count == 1 && board.Count(Owner.P1) == 2 ? LogLevel.Info : LogLevel.Warn);
        }
    }
}
