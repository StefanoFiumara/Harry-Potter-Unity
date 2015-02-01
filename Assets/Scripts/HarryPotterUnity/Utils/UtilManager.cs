using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.HarryPotterUnity.Cards;
using UnityEngine;

namespace Assets.Scripts.HarryPotterUnity.Utils
{
    public class UtilManager : MonoBehaviour {

        public const int PreviewLayer = 9;
        public const int CardLayer = 10;
        public const int ValidChoiceLayer = 11;
        public const int IgnoreRaycastLayer = 2;
        public const int DeckLayer = 12;

        public static Camera PreviewCamera;
        public static readonly Vector3 DefaultPreviewCameraPos = new Vector3(-400, 255, -70);
        public static Queue<TweenObject> TweenQueue = new Queue<TweenObject>();

        private static bool _tweenQueueRunning = false;

        public struct TweenObject
        {
            public GameObject Target;
            public Vector3 Position;
            public ITween.EaseType easeType;
            public float Time;
            public float Delay;
            public bool Flip;
            public bool Rotate;
            public GenericCard.CardStates StateAfterAnimation;
        }

        public static void AddTweenToQueue(GenericCard target, Vector3 position, float time, float delay, GenericCard.CardStates stateAfterAnimation, bool flip, bool rotate, ITween.EaseType easeType = ITween.EaseType.EaseInOutSine)
        {
            var newTween = new TweenObject
            {
                Target = target.gameObject,
                Position = position,
                Time = time,
                Delay = delay,
                easeType = easeType,
                StateAfterAnimation = stateAfterAnimation,
                Flip = flip,
                Rotate = rotate
            };


            TweenQueue.Enqueue(newTween);

            if (_tweenQueueRunning == false)
            {
                _tweenQueueRunning = true;
                StaticCoroutine.DoCoroutine(RunTweenQueue());
            }
        }

        private static IEnumerator RunTweenQueue()
        {
            //I might break the game here
            while (true)
            {
                if (TweenQueue.Count == 0)
                {
                    yield return null;
                }
                else
                {
                    var tween = TweenQueue.Dequeue();

                    ITween.MoveTo(tween.Target, ITween.Hash("time", tween.Time,
                        "delay", tween.Delay,
                        "position", tween.Position,
                        "easetype", tween.easeType,
                        "islocal", true,
                        "oncomplete", "SwitchState",
                        "oncompletetarget", tween.Target,
                        "oncompleteparams", tween.StateAfterAnimation
                        ));

                    if (tween.Flip) FlipCard(tween.Target, tween.Time);
                    if (tween.Rotate) RotateCard(tween.Target, tween.Time);

                    yield return new WaitForSeconds(tween.Time + tween.Delay);
                }
            }
        }

        public static void TweenCardToPosition(GenericCard card, Vector3 cardPosition, GenericCard.CardStates stateAfterAnimation, float tweenDelay = 0f, ITween.EaseType easeType = ITween.EaseType.EaseInOutSine)
        {
            ITween.MoveTo(card.gameObject, ITween.Hash("time", 0.5f,
                "position", cardPosition,
                "easetype", easeType,
                "islocal", true,
                "delay", tweenDelay,
                "oncomplete", "SwitchState",
                "oncompletetarget", card.gameObject,
                "oncompleteparams", stateAfterAnimation
                ));

            ITween.ScaleTo(card.gameObject, ITween.Hash("x", 1, "y", 1, "time", 0.5f));
        }

        private static void RotateCard(GameObject card, float time)
        {
            //set target based on current rotation, use 20f as an epsilon value for comparison
            Vector3 cardRotation = card.transform.localRotation.eulerAngles;
            float target = cardRotation.z > 20f ? 0f : 270f;
            ITween.RotateTo(card.gameObject, ITween.Hash("time", time,
                "z", target,
                "easetype", ITween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }

        private static void FlipCard(GameObject card, float time)
        {
            //set target based on current rotation, use 20f as an epsilon value for comparison
            Vector3 cardRotation = card.transform.localRotation.eulerAngles;
            float target = cardRotation.y > 20f ? 0f : 180f;
            ITween.RotateTo(card.gameObject, ITween.Hash("time", time,
                "y", target,
                "easetype", ITween.EaseType.EaseInOutSine,
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
