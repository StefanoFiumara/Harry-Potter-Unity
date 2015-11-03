using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Deck : MonoBehaviour
    {
        private List<BaseCard> _cards;

        private BaseCard _startingCharacter;

        private Player _player;

        private readonly Vector2 _deckPositionOffset = new Vector2(-355f, -124f);
        private readonly Vector3 _outlinePosition = new Vector3(-359.5f, -125.9f, 16f);
        
        private GameObject _outline;

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
            _cards = new List<BaseCard>(cardList);
            _startingCharacter = startingCharacter;
            
            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y);
            
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i] = Instantiate(_cards[i]);
                _cards[i].transform.parent = transform;
                _cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                _cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                _cards[i].transform.position += i * Vector3.back * 0.2f;

                _cards[i].Player = _player;

                _cards[i].NetworkId = GameManager._networkIdCounter++;
                GameManager.AllCards.Add(_cards[i]);
            }
        }

        public void SpawnStartingCharacter()
        {
            _startingCharacter = Instantiate(_startingCharacter);

            _startingCharacter.transform.parent = transform;
            _startingCharacter.Player = _player;

            _startingCharacter.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));

            _startingCharacter.NetworkId = GameManager._networkIdCounter++;
            

            GameManager.AllCards.Add(_startingCharacter);

            _player.InPlay.Add(_startingCharacter);
        }

        public BaseCard TakeTopCard()
        {
            BaseCard card = null;

            if (_cards.Count > 0)
            {
                card = _cards[_cards.Count - 1];
                _cards.RemoveAt(_cards.Count - 1);
            }

            if (_cards.Count <= 0)
            {
                GameOver();
            }

            _player.CardsLeftLabel.text = string.Format("Cards Left: {0}", _cards.Count);

            return card;
        }
        
        private void GameOver()
        {
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

            return _cards.Count > 0 && _player.CanUseActions();
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
            for (int i = _cards.Count-1; i >= 0; i--)
            {
                int random = Random.Range(0, i);

                var temp = _cards[i];
                _cards[i] = _cards[random];
                _cards[random] = temp;

                float newZ = (transform.position.z + 16f) - i * 0.2f;

                var point1 = Vector3.MoveTowards(_cards[i].transform.position, Camera.main.transform.position, 80f);
                point1.z = _cards[i].transform.position.z;

                var point2 = new Vector3(_cards[i].transform.position.x, _cards[i].transform.position.y, newZ);

                iTween.MoveTo(_cards[i].gameObject, iTween.Hash("time", 0.5f, 
                                                               "path", new[] {point1, point2}, 
                                                               "easetype", iTween.EaseType.EaseInOutSine, 
                                                               "delay", Random.Range(0f,1.5f)
                                                               ));
            }
        }

        public IEnumerable<BaseCard> GetCardsOfType(Type type, int amount)
        {
            //TODO: Randomize this
            return _cards.FindAll(card => card.Type == type).Take(amount);
        }

        public void Remove(BaseCard card)
        {
            _cards.Remove(card);
        }

        private void Add(BaseCard card)
        {
            _cards.Insert(0, card);
            card.transform.parent = transform;

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            cardPos.z -= _cards.IndexOf(card) * 0.2f;

            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(card.gameObject, cardPos,0.25f, 0f, FlipStates.FaceDown, TweenQueue.RotationType.NoRotate, State.InDeck));
        }

        public void AddAll(IEnumerable<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                Add(card);
            }

            AdjustCardSpacing();
        }

        private void AdjustCardSpacing()
        {
            GameManager.TweenQueue.AddTweenToQueue(new AsyncMoveTween(_cards, GetTargetPositionForCard));
        }

        private Vector3 GetTargetPositionForCard(BaseCard card)
        {
            if (!_cards.Contains(card))
            {
                Debug.LogError("GetTargetPositionForCard cannot find given card in deck.");
                return Vector3.zero;
            }

            int index = _cards.IndexOf(card);
            var result = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            result += index * Vector3.back * 0.2f;

            return result;
        }
    }
}
