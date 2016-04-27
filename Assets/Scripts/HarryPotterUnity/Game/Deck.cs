using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using UnityEngine;
using UnityLogWrapper;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Game
{
    public class Deck : CardCollection
    {
        private Player _player;

        private readonly Vector2 _deckPositionOffset = new Vector2(-355f, -124f);
        
        public BaseCard StartingCharacter { get; private set; }

        public event Action<Player> OnDeckIsOutOfCards;

        private void OnDestroy()
        {
            OnDeckIsOutOfCards = null;
        }

        private void Awake()
        {
            _player = transform.GetComponentInParent<Player>();

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
        }

        public void Initialize (IEnumerable<BaseCard> cardList, BaseCard startingCharacter)
        {
            Cards = new List<BaseCard>(cardList);
            StartingCharacter = startingCharacter;
            
            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y);
            
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i] = Instantiate( Cards[i] );

                Cards[i].transform.parent = transform;
                Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                Cards[i].transform.position += i * Vector3.back * 0.2f;

                Cards[i].Player = _player;

                Cards[i].NetworkId = GameManager.NetworkIdCounter++;
                
                GameManager.AllCards.Add(Cards[i]);

                Cards[i].CurrentCollection = this;
            }
        }

        public void SpawnStartingCharacter()
        {
            StartingCharacter = Instantiate(StartingCharacter);
            StartingCharacter.CurrentCollection = this;

            StartingCharacter.transform.parent = transform;
            StartingCharacter.Player = _player;

            StartingCharacter.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));

            StartingCharacter.NetworkId = GameManager.NetworkIdCounter++;
            
            GameManager.AllCards.Add(StartingCharacter);

            _player.InPlay.Add(StartingCharacter);
        }

        public BaseCard TakeTopCard()
        {
            BaseCard card = null;

            if (Cards.Count > 0)
            {
                card = Cards[Cards.Count - 1];
                Cards.RemoveAt(Cards.Count - 1);
                card.RemoveHighlight();
            }

            if (Cards.Count <= 0)
            {
                GameOver();
            }
            
            return card;
        }
        
        private void GameOver()
        {
            //TODO: Refactor this logic to occur on player class by subscribing to event
            _player.DisableAllCards();
            _player.OppositePlayer.DisableAllCards();

            if (OnDeckIsOutOfCards != null)
            {
                OnDeckIsOutOfCards(_player);
            }
                
        }

        private void OnMouseUp()
        {
            if (!CanDrawCard()) return;

            GameManager.Network.RPC("ExecuteDrawActionOnPlayer", PhotonTargets.All, _player.NetworkId);
        }

        private void OnMouseOver()
        {
            if (CanDrawCard())
            {
                Cards[Cards.Count - 1].SetHighlight();
            }
        }

        private void OnMouseExit()
        {
            Cards[Cards.Count - 1].RemoveHighlight();
        }

        private bool CanDrawCard()
        {
            if (!_player.IsLocalPlayer) return false;

            return Cards.Count > 0 && _player.CanUseActions();
        }

        public void DrawCard()
        {
            var card = TakeTopCard();
            if (card != null)
            {
                _player.Hand.Add(card);
            }
        }

        public void Shuffle()
        {
            for (int i = Cards.Count-1; i >= 0; i--)
            {
                int random = Random.Range(0, i);

                var temp = Cards[i];
                Cards[i] = Cards[random];
                Cards[random] = temp;
            }

            GameManager.TweenQueue.AddTweenToQueue(new ShuffleDeckTween(Cards));
            
        }
        
        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);
        }

        /// <summary>
        /// Adds a card to the bottom of the deck
        /// </summary>
        public override void Add(BaseCard card)
        {
            MoveToThisCollection(card);

            Cards.Insert(0, card);

            card.transform.parent = transform;

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            cardPos.z -= Cards.IndexOf(card) * 0.2f;

            var tween = new MoveTween
            {
                Target = card.gameObject,
                Position = cardPos,
                Time = 0.25f,
                Flip = FlipState.FaceDown,
                Rotate = TweenRotationType.NoRotate,
                OnCompleteCallback = () => card.State = State.InDeck
            };

            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        /// <summary>
        /// Last card in the list with be at the bottom of the deck.
        /// </summary>
        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Add(card);
            }

            AdjustCardSpacing();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Remove(card);
            }

            AdjustCardSpacing();
        }
        
        private void AdjustCardSpacing()
        {
            var tween = new AsyncMoveTween
            {
                Targets = Cards,
                GetPosition = GetTargetPositionForCard
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card))
            {
                Log.Write("Card not found in CardCollection");
                return card.transform.localPosition;
            }

            int index = Cards.IndexOf(card);
            var result = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            result += index * Vector3.back * 0.2f;

            return result;
        }
    }
}
