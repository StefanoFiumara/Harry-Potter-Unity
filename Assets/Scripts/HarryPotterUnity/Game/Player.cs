using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Cards.Interfaces;
using HarryPotterUnity.DeckGeneration;
using HarryPotterUnity.Enums;
using HarryPotterUnity.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityLogWrapper;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Player : MonoBehaviour {

        public Player OppositePlayer { get; set; }
        public Hand Hand { get; private set; }
        public Deck Deck { get; private set; }
        public InPlay InPlay { get; private set; }
        public Discard Discard { get; private set; }

        private readonly HashSet<LessonTypes> _lessonTypesInPlay = new HashSet<LessonTypes>(); 
        public HashSet<LessonTypes> LessonTypesInPlay
        {
            get
            {
                _lessonTypesInPlay.Clear();
                foreach (var providers in InPlay.Cards.OfType<ILessonProvider>())
                {
                    _lessonTypesInPlay.Add(providers.LessonType);
                }
                return _lessonTypesInPlay;
            }
        }
 
        public int AmountLessonsInPlay
        {
            get
            {
                return InPlay.Cards.OfType<ILessonProvider>().Sum(card => card.AmountLessonsProvided);
            }
        }

        public int CreatureDamageBuffer { private get; set; }

        private int ActionsAvailable { get; set; }

        public bool IsLocalPlayer { get; set; }
        public NetworkManager NetworkManager { get; set; }
        public byte NetworkId { get; set; }
        
        public delegate void OnTurnStartActions();
        public event OnTurnStartActions OnTurnStartEvent;

        public delegate void OnCardPlayedActions(BaseCard card, List<BaseCard> targets = null);
        public event OnCardPlayedActions OnCardPlayedEvent;

        public void OnCardPlayed(BaseCard card, List<BaseCard> targets = null)
        {
            if (OnCardPlayedEvent != null) OnCardPlayedEvent(card, targets);
        }

        public delegate void OnDamageTakenAction(BaseCard sourceCard, int amount);
        public event OnDamageTakenAction OnDamageTakenEvent;

        public void Awake()
        {
            ActionsAvailable = 0;

            Hand = transform.GetComponentInChildren<Hand>();
            Deck = transform.GetComponentInChildren<Deck>();
            InPlay = transform.GetComponentInChildren<InPlay>();
            Discard = transform.GetComponentInChildren<Discard>();
        }

        public void InitDeck(List<LessonTypes> selectedLessons)
        {
            Deck.InitDeck( DeckGenerator.GenerateDeck(selectedLessons), DeckGenerator.GetRandomStartingCharacter() );
        }

        public void UseActions(int amount = 1)
        {
            ActionsAvailable -= amount;
            
            if (ActionsAvailable <= 0) EndTurn(); 
        }
        
        public void AddActions(int amount)
        {
            ActionsAvailable += amount;
        }

        public void BeginTurn()
        {
            if (OnTurnStartEvent != null)
            {
                OnTurnStartEvent();
                OnTurnStartEvent = null;
            }

            foreach (var card in InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayBeforeTurnAction();
            }

            Deck.DrawCard();
            AddActions(2);
            if (ActionsAvailable < 1) ActionsAvailable = 1;

            //TODO: loop through creatures to send each source?
            //OppositePlayer.TakeDamage(null, DamagePerTurn);
            foreach (var creature in InPlay.GetCreaturesInPlay().Cast<BaseCreature>())
            {
                OppositePlayer.TakeDamage(creature, creature.DamagePerTurn);
            }

            //reset the damage buffer in case it was set last turn.
            OppositePlayer.CreatureDamageBuffer = 0;
        }

        private void EndTurn()
        {
            ActionsAvailable = 0;

            foreach (var card in InPlay.Cards.Cast<IPersistentCard>())
            {
                card.OnInPlayAfterTurnAction();
            }
            
            Hand.AdjustHandSpacing();
            OppositePlayer.BeginTurn();
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
        
        public void TakeDamage(BaseCard sourceCard, int amount)
        {
            if (amount <= 0) return;
            
            var cards = new List<BaseCard>();
            for (int i = 0; i < amount; i++)
            {
                //TODO: Only perform this check if the damage source is a creature!
                // OR check all the buffers based on damage source?? if(CanBeDamagedBySource(BaseCard source)) {
                if (sourceCard.Type == Type.Creature && CreatureDamageBuffer > 0)
                {
                    CreatureDamageBuffer--;
                    continue;
                }

                var card = Deck.TakeTopCard();

                if (card == null)
                {
                    Log.Write("Game Over");
                    break;
                }
                cards.Add(card);
            }

            Discard.AddAll(cards);

            if (OnDamageTakenEvent != null && sourceCard != null)
            {
                OnDamageTakenEvent(sourceCard, amount);
            }
        }
        
        public void DisableAllCards()
        {
            Deck.gameObject.layer = GameManager.IGNORE_RAYCAST_LAYER;
            GameManager.DisableCards(Hand.Cards);
            GameManager.DisableCards(InPlay.Cards);
        }

        public void EnableAllCards()
        {
            Deck.gameObject.layer = GameManager.DECK_LAYER;
            GameManager.EnableCards(Hand.Cards);
            GameManager.EnableCards(InPlay.Cards);
        }

        public void ClearHighlightComponent()
        {
            foreach (var card in Hand.Cards)
            {
                card.RemoveHighlight();
            }

            foreach (var card in InPlay.Cards)
            {
                card.RemoveHighlight();
            }

            foreach (var card in Discard.Cards)
            {
                card.RemoveHighlight();
            }
        }
    }
}
