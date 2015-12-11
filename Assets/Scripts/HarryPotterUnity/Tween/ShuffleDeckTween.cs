using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HarryPotterUnity.Tween
{
    public class ShuffleDeckTween : ITweenObject
    {
        private readonly IEnumerable<BaseCard> _cards;
        
        public float CompletionTime { get { return 2f; } }
        public float TimeUntilNextTween { get { return 0f; } }

        public ShuffleDeckTween(IEnumerable<BaseCard> cards, Func<BaseCard, Vector3> getTargetPosition)
        {
            _cards = cards;
        }

        public void ExecuteTween()
        {
            foreach (var card in _cards)
            {
                int cardIndex = ((IList<BaseCard>) _cards).IndexOf(card);
                var targetPosition = new Vector3( card.transform.position.x, 
                                                  card.transform.position.y, 
                                                 (card.transform.parent.position.z + 16f) - cardIndex * 0.2f);

                var midPoint = Vector3.MoveTowards(card.transform.position, Camera.main.transform.position, 80f);
                midPoint.z = card.transform.position.z;
                
                iTween.MoveTo(card.gameObject, iTween.Hash("time", 0.5f,
                                                           "path", new[] {midPoint, targetPosition},
                                                           "easetype", iTween.EaseType.EaseInOutSine,
                                                           "delay", Random.Range(0f, 1.5f)
                                                           ));
            }
        }
    }
}
