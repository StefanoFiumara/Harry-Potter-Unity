using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    public class TweenQueue
    {
        private Queue<ITweenObject> _queue;

        private bool _tweenQueueRunning;

        private float _timeUntilNextTween;

        public bool TweenQueueIsEmpty { get; private set; }

        public TweenQueue()
        {
            _queue = new Queue<ITweenObject>();
            _tweenQueueRunning = false;
            _timeUntilNextTween = 0f;
            TweenQueueIsEmpty = true;
        }

        
        public void AddTweenToQueue(ITweenObject tweenObject)
        {
            _queue.Enqueue(tweenObject);
            TweenQueueIsEmpty = false;

            if (_tweenQueueRunning) return;

            _tweenQueueRunning = true;
            StaticCoroutine.DoCoroutine(RunTweenQueue());
        }

        private IEnumerator RunTweenQueue()
        {
            while (_tweenQueueRunning)
            {
                if (_queue.Count == 0)
                {
                    TweenQueueIsEmpty = true;
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(_timeUntilNextTween);
                    ITweenObject tween = _queue.Dequeue();

                    if(tween.TweenSource != null) tween.TweenSource.SetHighlight();
                    tween.ExecuteTween();
                    _timeUntilNextTween = tween.TimeUntilNextTween;
                    yield return new WaitForSeconds(tween.CompletionTime);
                    if (tween.TweenSource != null) tween.TweenSource.RemoveHighlight();

                }
            }
        }

        public void Reset()
        {
            _queue = new Queue<ITweenObject>();
            _tweenQueueRunning = false;
            TweenQueueIsEmpty = true;
            StaticCoroutine.Die();
        }
    }
}
