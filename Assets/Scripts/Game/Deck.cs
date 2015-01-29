using System.Collections.Generic;
using Assets.Scripts.Cards;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class Deck : MonoBehaviour {

        public List<GenericCard> Cards; //TODO: Convert to auto-property after implementing deck generation?

        private Player _player;

        private readonly Vector2 _deckPositionOffset = new Vector2(-355f, -124f);
        
        public void Awake ()
        {
            _player = transform.GetComponentInParent<Player>();

            //instantiate cards into scene TODO: Maybe place this in Start() instead of Awake so it is executed after deck generation
            var cardPos = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
            
            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i] = (GenericCard)Instantiate(Cards[i]);
                Cards[i].transform.parent = transform;
                Cards[i].transform.localPosition = cardPos + Vector3.back * -16f;
                Cards[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, _player.transform.rotation.eulerAngles.z));
                Cards[i].transform.position += i * Vector3.back * 0.2f;

                Cards[i].Player = _player;
            }

            //Set the collider to the proper size and position
            if (gameObject.collider == null)
            {
                var col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(50f, 70f, 1f);
                col.center = new Vector3(_deckPositionOffset.x, _deckPositionOffset.y, 0f);
            }
        }
	
        public GenericCard TakeTopCard()
        {
            if (Cards.Count <= 0)
            {
                return null;
            }

            GenericCard card = Cards[Cards.Count - 1];
            Cards.RemoveAt(Cards.Count - 1);
            return card;
        }

        public void OnMouseUp()
        {
            if (Cards.Count <= 0 || !_player.CanUseAction()) return;

            DrawCard();
            _player.UseAction();
        }

        public void DrawCard()
        {
            var card = TakeTopCard();
            if (card == null)
            {
                //GameOver
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

                ITween.MoveTo(Cards[i].gameObject, ITween.Hash("time", 0.5f, 
                                                               "path", new[] {point1, point2}, 
                                                               "easetype", ITween.EaseType.EaseInOutSine, 
                                                               "delay", Random.Range(0f,1.5f))
                                                               );
            }
        }

        public void Disable()
        {
            gameObject.layer = Helper.IgnoreRaycastLayer;
        }

        public void Enable()
        {
            gameObject.layer = Helper.DeckLayer;
        }
    }
}
