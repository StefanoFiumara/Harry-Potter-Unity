using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Deck : MonoBehaviour
    {
        private List<GenericCard> _cards;

        private Player _player;

        private readonly Vector2 _deckPositionOffset = new Vector2(-355f, -124f);

        [UsedImplicitly]
        public void Awake()
        {
            _player = transform.GetComponentInParent<Player>();

            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
        }

        public void InitDeck (IEnumerable<GenericCard> cardList)
        {
            _cards = new List<GenericCard>(cardList);

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
            
            for (var i = 0; i < _cards.Count; i++)
            {
                _cards[i] = Instantiate(_cards[i]);
                _cards[i].transform.parent = transform;
                _cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                _cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                _cards[i].transform.position += i * Vector3.back * 0.2f;

                _cards[i].Player = _player;

                _cards[i].NetworkId = UtilManager.NetworkIdCounter++;
                UtilManager.AllCards.Add(_cards[i]);
            }
        }
	
        public GenericCard TakeTopCard()
        {
            GenericCard card = null;

            if (_cards.Count > 0)
            {
                card = _cards[_cards.Count - 1];
                _cards.RemoveAt(_cards.Count - 1);
            }

            if (_cards.Count <= 0)
            {
                _player.DisableAllCards();
                _player.OppositePlayer.DisableAllCards();
                StartCoroutine(MultiplayerGameManager.WaitForGameOverMessage(_player));
            }

            _player.CardsLeftLabel.text = string.Format("Cards Left: {0}", _cards.Count);

            return card;
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if (!_player.IsLocalPlayer) return;
            if (_cards.Count <= 0 || !_player.CanUseActions()) return;
            if (!UtilManager.TweenQueue.TweenQueueIsEmpty) return;

            _player.MpGameManager.photonView.RPC("ExecuteDrawActionOnPlayer", PhotonTargets.All, _player.NetworkId);
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
            for (var i = _cards.Count-1; i >= 0; i--)
            {
                var random = Random.Range(0, i);

                var temp = _cards[i];
                _cards[i] = _cards[random];
                _cards[random] = temp;

                var newZ = (transform.position.z + 16f) - i * 0.2f;

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

        public IEnumerable<GenericCard> GetCardsOfType(GenericCard.CardTypes type, int amount)
        {
            //TODO: Randomize this
            return _cards.FindAll(card => card.CardType == type).Take(amount);
        }

        public void Remove(GenericCard card)
        {
            _cards.Remove(card);
            //TODO: Adjust spacing
        }

        public void Add(GenericCard card)
        {
            _cards.Insert(0, card);
            card.transform.parent = transform;

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 16f);
            cardPos.z -= _cards.IndexOf(card) * 0.2f;

            var shouldFlip = card.FlipState == GenericCard.FlipStates.FaceUp;

            UtilManager.TweenQueue.AddTweenToQueue(card, cardPos, 0.25f, GenericCard.CardStates.Discarded, shouldFlip, TweenQueue.RotationType.NoRotate);
        }
    }
}
