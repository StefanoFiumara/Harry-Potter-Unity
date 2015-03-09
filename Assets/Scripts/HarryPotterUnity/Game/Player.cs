using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
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

        [SerializeField]
        private int _actionsAvailable;

        public int CreaturesInPlay { get; set; }
        public int DamagePerTurn { get; set; }
        public int AmountLessonsInPlay { get; set; }

        public bool IsLocalPlayer { get; set; }
        public MultiplayerGameManager MpGameManager { get; set; }
        public byte NetworkId { get; set; }

        public Text ActionsLeftLabel { private get; set; }
        public Image TurnIndicator { private get; set; }
        public Text CardsLeftLabel { get; set; }

        private MultiplayerLobbyHudManager _multiplayerLobbyHudManager;

        [UsedImplicitly]
        public void Awake()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();
            _actionsAvailable = 0;
            AmountLessonsInPlay = 0;

            Hand = transform.GetComponentInChildren<Hand>();
            Deck = transform.GetComponentInChildren<Deck>();
            InPlay = transform.GetComponentInChildren<InPlay>();
            Discard = transform.GetComponentInChildren<Discard>();

            _multiplayerLobbyHudManager = GameObject.Find("MultiplayerLobbyHudManager").GetComponent<MultiplayerLobbyHudManager>();

            if (!_multiplayerLobbyHudManager) throw new Exception("MultiplayerGameManager could not find MultiplayerLobbyHudManager!");
        }

        public void InitDeck(List<Lesson.LessonTypes> selectedLessons)
        {
            Deck.InitDeck( DeckGenerator.GenerateDeck(selectedLessons) );
        }

        public void UseActions(int amount = 1)
        {
            _actionsAvailable -= amount;

            ActionsLeftLabel.text = string.Format("Actions Left: {0}", _actionsAvailable);

            if (_actionsAvailable > 0) return;
            _actionsAvailable = 0;
            
            TurnIndicator.gameObject.SetActive(false);

            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayAfterTurnAction());
            OppositePlayer.InitTurn();
        }

        private void AddActions(int amount)
        {
            _actionsAvailable += amount;
            ActionsLeftLabel.text = string.Format("Actions Left: {0}", _actionsAvailable);
        }

        public void InitTurn()
        {
            TurnIndicator.gameObject.SetActive(true);
            //BeforeTurnAction happens here
            InPlay.Cards.ForEach(card => ((IPersistentCard) card).OnInPlayBeforeTurnAction());

            Deck.DrawCard();
            AddActions(2);

            //Creatures do damage here
            OppositePlayer.TakeDamage(DamagePerTurn);
        }

        public bool CanUseActions(int amount = 1)
        {
            return _actionsAvailable >= amount;
        }

        public void DrawInitialHand()
        {
            //TODO: Needs cleanup
            for (var i = 0; i < 7; i++)
            {
                var card = Deck.TakeTopCard();
                var cardPosition = Hand.HandCardsOffset;

                var shrinkFactor = Hand.Cards.Count >= 12 ? 0.5f : 1f;

                cardPosition.x += Hand.Cards.Count * Hand.Spacing * shrinkFactor;
                cardPosition.z -= Hand.Cards.Count;

                Hand.Cards.Add(card);
                card.transform.parent = Hand.transform;

                UtilManager.AddTweenToQueue(card, cardPosition, 0.3f, 0f, GenericCard.CardStates.InHand, IsLocalPlayer, false);
            }       
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;

            for (var i = 0; i < amount; i++)
            {
                var card = Deck.TakeTopCard();

                if (card == null)
                {
                    //TODO: Show game over message here.
                    Debug.Log("Game Over");
                    break;
                }
                Discard.Add(card);
            }
        }

        public void UpdateLessonTypesInPlay()
        {
            LessonTypesInPlay = new List<Lesson.LessonTypes>();

            var lessons = InPlay.Cards.FindAll(card => card is Lesson);

            foreach (var genericCard in lessons)
            {
                var lessonCard = (Lesson) genericCard;
                if (!LessonTypesInPlay.Contains(lessonCard.LessonType))
                {
                    LessonTypesInPlay.Add(lessonCard.LessonType);
                }
            }
        }

        public void DisableAllCards()
        {
            Deck.gameObject.layer = UtilManager.IgnoreRaycastLayer;
            UtilManager.DisableCards(Hand.Cards);
            UtilManager.DisableCards(InPlay.Cards);
        }

        public void EnableAllCards()
        {
            Deck.gameObject.layer = UtilManager.DeckLayer;
            UtilManager.EnableCards(Hand.Cards);
            UtilManager.EnableCards(InPlay.Cards);
        }
    }
}
