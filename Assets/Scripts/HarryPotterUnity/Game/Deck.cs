using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Deck : CardCollection
    {
        private Player _player;

        private readonly Vector2 _deckPositionOffset = new Vector2(-355f, -124f);
        private readonly Vector3 _outlinePosition = new Vector3(-359.5f, -125.9f, 16f);
        
        private GameObject _outline;

        public BaseCard StartingCharacter { get; private set; }

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);

            LoadOutlineComponent();
        }

        private void LoadOutlineComponent()
        {
            var resource = Resources.Load("DeckOutline");

            _outline = (GameObject) Instantiate(resource);
            _outline.transform.localPosition = _outlinePosition;
            _outline.transform.rotation = Quaternion.Euler(new Vector3(90f, 180f, _player.transform.rotation.eulerAngles.z));
            _outline.transform.parent = transform;
            _outline.SetActive(false);
        }

        public void InitDeck (IEnumerable<BaseCard> cardList, BaseCard startingCharacter)
        {
            Cards = new List<BaseCard>(cardList);
            StartingCharacter = startingCharacter;
            
            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y);
            
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i] = Instantiate(Cards[i]);
                Cards[i].transform.parent = transform;
                Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                Cards[i].transform.position += i * Vector3.back * 0.2f;

                Cards[i].Player = _player;

                Cards[i].NetworkId = GameManager._networkIdCounter++;
                
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

            StartingCharacter.NetworkId = GameManager._networkIdCounter++;
            
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
            }

            if (Cards.Count <= 0)
            {
                GameOver();
            }

            UpdateCardsLeftLabel();

            return card;
        }

        private void UpdateCardsLeftLabel()
        {
            _player.CardsLeftLabel.text = string.Format("Cards Left: {0}", Cards.Count);
        }

        private void GameOver()
        {
            if (!GameManager._gameInProgress) return;

            GameManager._gameInProgress = false;
            _player.DisableAllCards();
            _player.OppositePlayer.DisableAllCards();
            StartCoroutine(NetworkManager.WaitForGameOverMessage(_player));
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if (!CanDrawCard()) return;

            _player.NetworkManager.photonView.RPC("ExecuteDrawActionOnPlayer", PhotonTargets.All, _player.NetworkId);
        }

        [UsedImplicitly]
        private void OnMouseOver()
        {
            if (CanDrawCard())
            {
                _outline.SetActive(true);
            }
        }

        [UsedImplicitly]
        private void OnMouseExit()
        {
            _outline.SetActive(false);
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

                float newZ = (transform.position.z + 16f) - i * 0.2f;

                var point1 = Vector3.MoveTowards(Cards[i].transform.position, Camera.main.transform.position, 80f);
                point1.z = Cards[i].transform.position.z;

                var point2 = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y, newZ);

                iTween.MoveTo(Cards[i].gameObject, iTween.Hash("time", 0.5f, 
                                                               "path", new[] {point1, point2}, 
                                                               "easetype", iTween.EaseType.EaseInOutSine, 
                                                               "delay", Random.Range(0f,1.5f)
                                                               ));
            }
        }

        public IEnumerable<BaseCard> GetCardsOfType(Type type, int amount)
        {
            return Cards.FindAll(card => card.Type == type).Take(amount);
        }

        protected override void Remove(BaseCard card)
        {
            Cards.Remove(card);
            UpdateCardsLeftLabel();
        }

        public override void Add(BaseCard card)
        {
            MoveToThisCollection(card);

            Cards.Insert(0, card);
            
            UpdateCardsLeftLabel();

            card.transform.parent = transform;

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            cardPos.z -= Cards.IndexOf(card) * 0.2f;

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos,0.25f, 0f, FlipStates.FaceDown, RotationType.NoRotate, State.InDeck));
        }

        public override void AddAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Add(card);
            }

            AdjustCardSpacing();
        }

        public override void RemoveAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Remove(card);
            }

            AdjustCardSpacing();
        }
        
        private void AdjustCardSpacing()
        {
            GameManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(Cards, GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!Cards.Contains(card)) return card.transform.localPosition;

            int index = Cards.IndexOf(card);
            var result = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            result += index * Vector3.back * 0.2f;

            return result;
        }
    }
}
