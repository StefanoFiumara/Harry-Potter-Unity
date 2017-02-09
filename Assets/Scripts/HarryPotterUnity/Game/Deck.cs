using System;
using System.Collections.Generic;
using System.Linq;
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

        public event Action<Player> OnDeckIsOutOfCardsEvent;

        private void OnDestroy()
        {
            this.OnDeckIsOutOfCardsEvent = null;
        }

        private void Awake()
        {
            this._player = this.transform.GetComponentInParent<Player>();

            var col = this.gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(this._deckPositionOffset.x, this._deckPositionOffset.y, 0f);
        }

        public void Initialize (IEnumerable<BaseCard> cardList, BaseCard startingCharacter)
        {
            this.Cards = new List<BaseCard>(cardList);
            this.StartingCharacter = startingCharacter;
            
            var cardPos = new Vector3(this._deckPositionOffset.x, this._deckPositionOffset.y);
            
            for (int i = 0; i < this.Cards.Count; i++)
            {
                this.Cards[i] = Instantiate(this.Cards[i] );

                this.Cards[i].transform.parent = this.transform;
                this.Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                this.Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, this._player.transform.rotation.eulerAngles.z));
                this.Cards[i].transform.position += i * Vector3.back * 0.2f;

                this.Cards[i].Player = this._player;

                this.Cards[i].NetworkId = GameManager.NetworkIdCounter++;
                
                GameManager.AllCards.Add(this.Cards[i]);

                this.Cards[i].CurrentCollection = this;
            }
        }

        public void SpawnStartingCharacter()
        {
            this.StartingCharacter = Instantiate(this.StartingCharacter);
            this.StartingCharacter.CurrentCollection = this;

            this.StartingCharacter.transform.parent = this.transform;
            this.StartingCharacter.Player = this._player;

            this.StartingCharacter.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, this._player.transform.rotation.eulerAngles.z));

            this.StartingCharacter.NetworkId = GameManager.NetworkIdCounter++;
            
            GameManager.AllCards.Add(this.StartingCharacter);

            this._player.InPlay.Add(this.StartingCharacter);
        }

        public BaseCard TakeTopCard()
        {
            BaseCard card = null;

            if (this.Cards.Count > 0)
            {
                card = this.Cards[this.Cards.Count - 1];
                this.Cards.RemoveAt(this.Cards.Count - 1);
                card.RemoveHighlight();
            }

            if (this.Cards.Count <= 0)
            {
                this.GameOver(); //TODO: call OnDeckIsOutOfCards event here
            }
            
            return card;
        }
        
        private void GameOver()
        {
            //TODO: Refactor this logic to occur on player class by subscribing to event
            this._player.DisableAllCards();
            this._player.OppositePlayer.DisableAllCards();

            if (this.OnDeckIsOutOfCardsEvent != null)
            {
                this.OnDeckIsOutOfCardsEvent(this._player);
            }
                
        }

        private void OnMouseUp()
        {
            if (!this.CanDrawCard()) return;

            GameManager.Network.RPC("ExecuteDrawActionOnPlayer", PhotonTargets.All, this._player.NetworkId);
        }

        private void OnMouseOver()
        {
            if (this.CanDrawCard())
            {
                this.Cards[this.Cards.Count - 1].SetHighlight();
            }
        }

        private void OnMouseExit()
        {
            if(this.Cards.Any()) this.Cards[this.Cards.Count - 1].RemoveHighlight();
        }

        private bool CanDrawCard()
        {
            if (!this._player.IsLocalPlayer) return false;

            return this.Cards.Count > 0 && this._player.CanUseActions();
        }

        public void DrawCard()
        {
            var card = this.TakeTopCard();
            if (card != null)
            {
                this._player.Hand.Add(card);
            }
        }

        public void Shuffle()
        {
            for (int i = this.Cards.Count-1; i >= 0; i--)
            {
                int random = Random.Range(0, i);

                var temp = this.Cards[i];
                this.Cards[i] = this.Cards[random];
                this.Cards[random] = temp;
            }

            GameManager.TweenQueue.AddTweenToQueue(new ShuffleDeckTween(this.Cards));
            
        }
        
        protected override void Remove(BaseCard card)
        {
            this.Cards.Remove(card);
        }

        /// <summary>
        /// Adds a card to the bottom of the deck
        /// </summary>
        public override void Add(BaseCard card)
        {
            this.MoveToThisCollection(card);

            this.Cards.Insert(0, card);

            card.transform.parent = this.transform;

            var cardPos = new Vector3(this._deckPositionOffset.x, this._deckPositionOffset.y, 16f);
            cardPos.z -= this.Cards.IndexOf(card) * 0.2f;

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
                this.Add(card);
            }

            this.AdjustCardSpacing();
        }

        protected override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                this.Remove(card);
            }

            this.AdjustCardSpacing();
        }
        
        private void AdjustCardSpacing()
        {
            var tween = new AsyncMoveTween
            {
                Targets = this.Cards,
                GetPosition = this.GetTargetPositionForCard
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!this.Cards.Contains(card))
            {
                Log.Write("Card not found in CardCollection");
                return card.transform.localPosition;
            }

            int index = this.Cards.IndexOf(card);
            var result = new Vector3(this._deckPositionOffset.x, this._deckPositionOffset.y, 16f);
            result += index * Vector3.back * 0.2f;

            return result;
        }
    }
}
