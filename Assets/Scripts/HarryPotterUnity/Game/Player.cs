using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.Cards.PlayerConstraints;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;
using Type = HarryPotterUnity.Enums.Type;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Player : MonoBehaviour
    {
        public Player OppositePlayer { get; set; }
        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public InPlay InPlay { get; private set; }
        public Discard Discard { get; private set; }

        public List<BaseCard> AllCards
        {
            get { return this.Hand.Cards.Concat(this.Deck.Cards).Concat(this.InPlay.Cards).Concat(this.Discard.Cards).ToList(); }
        }

        private readonly HashSet<LessonTypes> _lessonTypesInPlay = new HashSet<LessonTypes>(); 
        public HashSet<LessonTypes> LessonTypesInPlay
        {
            get
            {
                this._lessonTypesInPlay.Clear();
                foreach (var providers in this.InPlay.Cards.OfType<ILessonProvider>())
                {
                    this._lessonTypesInPlay.Add(providers.LessonType);
                }
                return this._lessonTypesInPlay;
            }
        }
 
        public int AmountLessonsInPlay
        {
            get
            {
                return this.InPlay.Cards.OfType<ILessonProvider>().Sum(card => card.AmountLessonsProvided);
            }
        }

        public HashSet<Type> TypeImmunity { get; private set; }

        public int ActionsAvailable { get; private set; }

        public bool IsLocalPlayer { get; set; }
        public byte NetworkId { get; set; }
        public List<IPlayerConstraint> Constraints { get; private set; }

        public delegate void TurnEvent();
        public delegate void CardPlayedEvent(BaseCard card, List<BaseCard> targets = null);
        public delegate void DamageTakenEvent(BaseCard source, int amount);

        public event TurnEvent OnNextTurnStartEvent;
        public event TurnEvent OnStartTurnEvent;
        public event TurnEvent OnEndTurnEvent;
        public event CardPlayedEvent OnCardPlayedEvent;
        public event DamageTakenEvent OnDamageTakenEvent;

        public void OnDestroy()
        {
            this.OnNextTurnStartEvent = null;
            this.OnStartTurnEvent = null;
            this.OnEndTurnEvent = null;
            this.OnCardPlayedEvent = null;
            this.OnDamageTakenEvent = null;
        }

        public void Awake()
        {
            this.ActionsAvailable = 0;

            this.Hand = this.transform.GetComponentInChildren<Hand>();
            this.Deck = this.transform.GetComponentInChildren<Deck>();
            this.InPlay = this.transform.GetComponentInChildren<InPlay>();
            this.Discard = this.transform.GetComponentInChildren<Discard>();

            this.TypeImmunity = new HashSet<Type>();
            this.Constraints = new List<IPlayerConstraint>();
        }

        public void InitDeck(List<LessonTypes> selectedLessons)
        {
            List<BaseCard> cards;
            BaseCard startingCharacter;

            if (GameManager.DebugModeEnabled)
            {
                var prebuiltCards = GameManager.GetPlayerTestDeck(this.NetworkId);
                cards = DeckGenerator.GenerateDeck(prebuiltCards, selectedLessons);
                startingCharacter = GameManager.GetPlayerTestCharacter(this.NetworkId);
            }
            else
            {
                cards = DeckGenerator.GenerateDeck(selectedLessons);
                startingCharacter = DeckGenerator.GetRandomCharacter();
            }

            this.Deck.Initialize(cards, startingCharacter);
        }

        /// <summary>
        /// Remember to always call this function after the card's effect,
        /// otherwise the action will be used before the card activates.
        /// possibly beginning the next turn and causing a chain of actions to occur
        /// such as drawing the opponent's card and dealing his creature's damage before your card's effect activates.
        /// </summary>
        /// <param name="amount"></param>
        public void UseActions(int amount = 1)
        {
            this.ActionsAvailable -= amount;
            if (this.ActionsAvailable <= 0) this.EndTurn(); 
        }
        
        public void AddActions(int amount)
        {
            this.ActionsAvailable += amount;
        }

        public void InvokeCardPlayedEvent(BaseCard card, List<BaseCard> targets = null)
        {
            if (this.OnCardPlayedEvent != null) this.OnCardPlayedEvent(card, targets);
        }

        public void BeginTurn()
        {
            if (this.OnNextTurnStartEvent != null)
            {
                this.OnNextTurnStartEvent();
                this.OnNextTurnStartEvent = null;
            }

            if (this.OnStartTurnEvent != null)
            {
                this.OnStartTurnEvent();
            }

            foreach (var card in this.InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayBeforeTurnAction();
            }

            
             //TODO: some adventures prevent this step from happening, need a check of some kind.
            this.Deck.DrawCard();

            this.AddActions(2);
            if (this.ActionsAvailable < 1)
            {
                this.ActionsAvailable = 1;
            }

            foreach (var creature in this.InPlay.Creatures.Cast<BaseCreature>())
            {
                this.OppositePlayer.TakeDamage(creature, creature.DamagePerTurn);
            }
        }

        private void EndTurn()
        {
            if (this.OnEndTurnEvent != null)
            {
                this.OnEndTurnEvent();
            }
            this.ActionsAvailable = 0;

            foreach (var card in this.InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayAfterTurnAction();
            }

            this.Hand.AdjustHandSpacing();
            this.OppositePlayer.BeginTurn();
        }

        public bool CanUseActions(int amount = 1)
        {
            return this.ActionsAvailable >= amount;
        }

        public void DrawInitialHand()
        {
            for (int i = 0; i < 7; i++)
            {
                var card = this.Deck.TakeTopCard();
                this.Hand.Add(card, preview: false, adjustSpacing: false);
            }       
        }
        
        public void TakeDamage(BaseCard sourceCard, int amount)
        {
            if (amount <= 0) return;
            
            var cards = new List<BaseCard>();

            for (int i = 0; i < amount; i++)
            {
                if (this.TypeImmunity.Contains(sourceCard.Type))
                {
                    continue;
                }

                var card = this.Deck.TakeTopCard();

                if (card == null)
                {
                    break;
                }
                cards.Add(card);
            }

            this.Discard.AddAll(cards);

            if (this.OnDamageTakenEvent != null && sourceCard != null)
            {
                this.OnDamageTakenEvent(sourceCard, cards.Count); 
            }
        }
        
        public void DisableAllCards()
        {
            this.Deck.gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            GameManager.DisableCards(this.Hand.Cards);
            GameManager.DisableCards(this.InPlay.Cards);
        }

        public void EnableAllCards()
        {
            this.Deck.gameObject.layer = GameManager.DECK_LAYER;
            GameManager.EnableCards(this.Hand.Cards);
            GameManager.EnableCards(this.InPlay.Cards);
        }

        public void ClearHighlightComponent()
        {
            foreach (var card in this.Hand.Cards)
            {
                card.RemoveHighlight();
            }

            foreach (var card in this.InPlay.Cards)
            {
                card.RemoveHighlight();
            }

            foreach (var card in this.Discard.Cards)
            {
                card.RemoveHighlight();
            }
        }
    }
}
