using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Game
{
    [UsedImplicitly]
    public class Deck : MonoBehaviour
    {
        private List<GenericCard> Cards { get; set; }

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
            Cards = new List<GenericCard>(cardList);

            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
            
            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i] = Instantiate(Cards[i]);
                Cards[i].transform.parent = transform;
                Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                Cards[i].transform.position += i * Vector3.back * 0.2f;

                Cards[i].Player = _player;

                Cards[i].NetworkId = UtilManager.NetworkIdCounter++;
                UtilManager.AllCards.Add(Cards[i]);
            }
        }
	
        public GenericCard TakeTopCard()
        {
            if (Cards.Count <= 0)
            {
                return null;
            }

            var card = Cards[Cards.Count - 1];
            Cards.RemoveAt(Cards.Count - 1);
            return card;
        }

        [UsedImplicitly]
        public void OnMouseUp()
        {
            if (Cards.Count <= 0 || !_player.CanUseActions()) return;
            if (!_player.IsLocalPlayer) return;

            DrawCard();
            _player.UseActions();
        }

        public void DrawCard()
        {
            var card = TakeTopCard();
            if (card == null)
            {
                //GameOver
                Debug.Log("Game Over");
                return;
            }

            _player.Hand.Add(card);
        }

        public void Shuffle()
        {
            for (var i = Cards.Count-1; i >= 0; i--)
            {
                var random = Random.Range(0, i);

                var temp = Cards[i];
                Cards[i] = Cards[random];
                Cards[random] = temp;

                var newZ = (transform.position.z + 16f) - i * 0.2f;

                var point1 = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y + 80, Cards[i].transform.position.z);
                var point2 = new Vector3(Cards[i].transform.position.x, Cards[i].transform.position.y, newZ);

                iTween.MoveTo(Cards[i].gameObject, iTween.Hash("time", 0.5f, 
                                                               "path", new[] {point1, point2}, 
                                                               "easetype", iTween.EaseType.EaseInOutSine, 
                                                               "delay", Random.Range(0f,1.5f))
                                                               );
            }
        }

        //Do we need these??
        /*
        public void Disable()
        {
            gameObject.layer = UtilManager.IgnoreRaycastLayer;
        }

        public void Enable()
        {
            gameObject.layer = UtilManager.DeckLayer;
        }
         * */
    }
}
