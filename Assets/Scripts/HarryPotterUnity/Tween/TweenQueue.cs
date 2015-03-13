using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    public class TweenQueue
    {
        public enum RotationType
        {
            NoRotate, Rotate90, Rotate180
        }

        private Queue<ITweenObject> _queue;

        private bool _tweenQueueRunning;
        public bool TweenQueueIsEmpty { get; private set; }

        public TweenQueue()
        {
            _queue = new Queue<ITweenObject>();
            _tweenQueueRunning = false;
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
            while (true)
            {
                if (_queue.Count == 0)
                {
                    TweenQueueIsEmpty = true;
                    yield return null;
                }
                else
                {
                    var tween = _queue.Dequeue();
                    tween.ExecuteTween();
                    yield return new WaitForSeconds(tween.CompletionTime);
                }
            }
            // ReSharper disable once FunctionNeverReturns
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
