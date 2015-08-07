using System;
using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using HarryPotterUnity.UI;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Player : MonoBehaviour {

        public Player OppositePlayer { get; set; }
        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public InPlay InPlay { get; private set; }
        public Discard Discard { get; private set; }

        public List<Lesson.LessonTypes> LessonTypesInPlay { get; private set; }

        public int CreaturesInPlay { get; set; }
        public int DamagePerTurn { get; set; }
        public int DamageBuffer { private get; set; }
        public int AmountLessonsInPlay { get; set; }
        private int ActionsAvailable { get; set; }

        public bool IsLocalPlayer { get; set; }
        public NetworkManager NetworkManager { get; set; }
        public byte NetworkId { get; set; }

        public Text ActionsLeftLabel { private get; set; }
        public Image TurnIndicator { private get; set; }
        public RectTransform EndGamePanel { private get; set; }
        public Text CardsLeftLabel { get; set; }


        private HudManager _hudManager;

        [UsedImplicitly]
        public void Awake()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();
            ActionsAvailable = 0;
            AmountLessonsInPlay = 0;

            Hand = transform.GetComponentInChildren<Hand>();
            Deck = transform.GetComponentInChildren<Deck>();
            InPlay = transform.GetComponentInChildren<InPlay>();
            Discard = transform.GetComponentInChildren<Discard>();

            _hudManager = FindObjectOfType<HudManager>();

            if (!_hudManager) throw new Exception("MultiplayerGameManager could not find MultiplayerLobbyHudManager!");
        }

        public void InitDeck(List<Lesson.LessonTypes> selectedLessons)
        {
            Deck.InitDeck( DeckGenerator.GenerateDeck(selectedLessons) );
        }

        public void UseActions(int amount = 1)
        {
            ActionsAvailable -= amount;

            ActionsLeftLabel.text = string.Format("Actions Left: {0}", ActionsAvailable);

            if (ActionsAvailable > 0) return;
            ActionsAvailable = 0;
            
            TurnIndicator.gameObject.SetActive(false);

            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayAfterTurnAction());

            Hand.AdjustHandSpacing();

            OppositePlayer.InitTurn();
        }

        public void AddActions(int amount)
        {
            ActionsAvailable += amount;
            ActionsLeftLabel.text = string.Format("Actions Left: {0}", ActionsAvailable);
        }

        public void InitTurn(bool firstTurn = false)
        {
            TurnIndicator.gameObject.SetActive(true);
            
            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayBeforeTurnAction());

            Deck.DrawCard();

            AddActions(2);

            if (ActionsAvailable < 1) ActionsAvailable = 1;

            if( !firstTurn ) _hudManager.ToggleSkipActionButton();

            //Creatures do damage here
            OppositePlayer.TakeDamage(DamagePerTurn);

            //reset the damage buffer in case it was set last turn.
            OppositePlayer.DamageBuffer = 0;
        }

        public bool CanUseActions(int amount = 1)
        {
            return ActionsAvailable >= amount;
        }

        public void DrawInitialHand()
        {
            for (int i = 0; i < 7; i++)
            {
                var card = Deck.TakeTopCard();
                Hand.Add(card, preview: false, adjustSpacing: false);
            }       
        }

        public void ShowGameOverLoseMessage()
        {
            var titleLabel = EndGamePanel.FindChild("Title").GetComponent<Text>();
            var messageLabel = EndGamePanel.FindChild("Message").GetComponent<Text>();

            titleLabel.text = "Sorry!";
            messageLabel.text = "Your Opponent Defeated You!";

            EndGamePanel.gameObject.SetActive(true);
        }

        public void ShowGameOverWinMesssage()
        {
            var titleLabel = EndGamePanel.FindChild("Title").GetComponent<Text>();
            var messageLabel = EndGamePanel.FindChild("Message").GetComponent<Text>();

            titleLabel.text = "Congratulations!";
            messageLabel.text = "You won the game!";

            EndGamePanel.gameObject.SetActive(true);
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            var cards = new List<GenericCard>();

            for (int i = 0; i < amount; i++)
            {
                if (DamageBuffer > 0)
                {
                    DamageBuffer--;
                    continue;
                }

                var card = Deck.TakeTopCard();

                if (card == null)
                {
                    Debug.Log("Game Over");
                    break;
                }
                cards.Add(card);
            }

            Discard.AddAll(cards);
        }

        public void UpdateLessonTypesInPlay()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();

            var lessonProviders = InPlay.Cards.FindAll(card => card is ILessonProvider).Cast<ILessonProvider>();

            foreach (var lessonProvider in lessonProviders.Where(provider => !LessonTypesInPlay.Contains(provider.LessonType)))
            {
                LessonTypesInPlay.Add(lessonProvider.LessonType);
            }
        }

        public void DisableAllCards()
        {
            Deck.gameObject.layer = GameManager.IgnoreRaycastLayer;
            GameManager.DisableCards(Hand.Cards);
            GameManager.DisableCards(InPlay.Cards);
        }

        public void EnableAllCards()
        {
            Deck.gameObject.layer = GameManager.DeckLayer;
            GameManager.EnableCards(Hand.Cards);
            GameManager.EnableCards(InPlay.Cards);
        }
    }
}
