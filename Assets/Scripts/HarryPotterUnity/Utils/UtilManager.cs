using System.Collections;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public class TweenObject
    {
        public GameObject Target;
        public Vector3 Position;
        public iTween.EaseType EaseType;
        public float Time;
        public float Delay;
        public bool Flip;
        public bool Rotate;
        public GenericCard.CardStates StateAfterAnimation;
    }

    public static class UtilManager {

        public const int PreviewLayer = 9;
        public const int CardLayer = 10;
        public const int ValidChoiceLayer = 11;
        public const int IgnoreRaycastLayer = 2;
        public const int DeckLayer = 12;

        public static byte NetworkIdCounter;
        public static readonly List<GenericCard> AllCards = new List<GenericCard>(); 

        public static Camera PreviewCamera;
        public static readonly Vector3 DefaultPreviewCameraPos = new Vector3(-400, 255, -70);
        public static readonly Queue<TweenObject> TweenQueue = new Queue<TweenObject>();

        private static bool _tweenQueueRunning;

        public static void AddTweenToQueue(GenericCard target, Vector3 position, float time, float delay, GenericCard.CardStates stateAfterAnimation, bool flip, bool rotate, iTween.EaseType easeType = iTween.EaseType.EaseInOutSine)
        {
            var newTween = new TweenObject
            {
                Target = target.gameObject,
                Position = position,
                Time = time,
                Delay = delay,
                EaseType = easeType,
                StateAfterAnimation = stateAfterAnimation,
                Flip = flip,
                Rotate = rotate
            };

            TweenQueue.Enqueue(newTween);

            if (_tweenQueueRunning) return;

            _tweenQueueRunning = true;
            StaticCoroutine.DoCoroutine(RunTweenQueue());
        }

        private static IEnumerator RunTweenQueue()
        {
            while (true)
            {
                if (TweenQueue.Count == 0)
                {
                    yield return null;
                }
                else
                {
                    var tween = TweenQueue.Dequeue();

                    iTween.MoveTo(tween.Target, iTween.Hash("time", tween.Time,
                        "delay", tween.Delay,
                        "position", tween.Position,
                        "easetype", tween.EaseType,
                        "islocal", true,
                        "oncomplete", "SwitchState",
                        "oncompletetarget", tween.Target,
                        "oncompleteparams", tween.StateAfterAnimation
                        ));

                    if (tween.Flip || tween.Rotate)
                    {
                        RotateAndFlipCard(tween.Target, tween.Time, tween.Flip, tween.Rotate);
                    }

                    yield return new WaitForSeconds(tween.Time + tween.Delay);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static void TweenCardToPosition(GenericCard card, Vector3 cardPosition, GenericCard.CardStates stateAfterAnimation, float tweenDelay = 0f, iTween.EaseType easeType = iTween.EaseType.EaseInOutSine)
        {
            iTween.MoveTo(card.gameObject, iTween.Hash("time", 0.5f,
                "position", cardPosition,
                "easetype", easeType,
                "islocal", true,
                "delay", tweenDelay,
                "oncomplete", "SwitchState",
                "oncompletetarget", card.gameObject,
                "oncompleteparams", stateAfterAnimation
                ));

            iTween.ScaleTo(card.gameObject, iTween.Hash("x", 1, "y", 1, "time", 0.5f));
        }

        private static void RotateAndFlipCard(GameObject card, float time, bool flip, bool rotate)
        {
            var cardRotation = card.transform.localRotation.eulerAngles;
            var targetFlip = flip ? (cardRotation.y > 20f ? 0f : 180f) : cardRotation.y;
            var targetRotate = rotate ? (cardRotation.z > 20f ? 0f : 270f) : cardRotation.z;

            if(flip) card.GetComponent<GenericCard>().SwitchFlipState();

            iTween.RotateTo(card, iTween.Hash("time", time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
        public static void DisableCards(List<GenericCard> cards)
        {
            cards.ForEach(card => card.Disable());
        }

        public static void EnableCards(List<GenericCard> cards)
        {
            cards.ForEach(card => card.Enable());
        }
    }
}
