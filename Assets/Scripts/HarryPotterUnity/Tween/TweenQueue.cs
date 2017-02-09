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
            this._queue = new Queue<ITweenObject>();
            this._tweenQueueRunning = false;
            this._timeUntilNextTween = 0f;
            this.TweenQueueIsEmpty = true;
        }

        
        public void AddTweenToQueue(ITweenObject tweenObject)
        {
            this._queue.Enqueue(tweenObject);
            this.TweenQueueIsEmpty = false;

            if (this._tweenQueueRunning) return;

            this._tweenQueueRunning = true;
            StaticCoroutine.DoCoroutine(this.RunTweenQueue());
        }

        private IEnumerator RunTweenQueue()
        {
            while (this._tweenQueueRunning)
            {
                if (this._queue.Count == 0)
                {
                    this.TweenQueueIsEmpty = true;
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(this._timeUntilNextTween);
                    ITweenObject tween = this._queue.Dequeue();

                    tween.ExecuteTween();
                    this._timeUntilNextTween = tween.TimeUntilNextTween;

                    yield return new WaitForSeconds(tween.CompletionTime);
                }
            }
        }

        public void Reset()
        {
            this._queue = new Queue<ITweenObject>();
            this._tweenQueueRunning = false;
            this.TweenQueueIsEmpty = true;

            iTween.Stop();

            StaticCoroutine.Die();
        }
    }
}
