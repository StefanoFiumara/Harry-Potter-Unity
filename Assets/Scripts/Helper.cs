using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using CardStates = GenericCard.CardStates;
using EaseType = iTween.EaseType;

public class Helper : MonoBehaviour {

    public const int PREVIEW_LAYER = 9;
    public const int CARD_LAYER = 10;
    public const int VALID_CHOICE_LAYER = 11;
    public const int IGNORE_RAYCAST_LAYER = 2;
    public const int DECK_LAYER = 12;

    public static Camera PreviewCamera;
    public static readonly Vector3 DefaultPreviewCameraPos = new Vector3(-400, 255, -70);
    public static Queue<TweenObject> TweenQueue = new Queue<TweenObject>();

    private static bool TweenQueueRunning = false;

    public struct TweenObject
    {
        public GameObject target;
        public Vector3 position;
        public EaseType easeType;
        public float time;
        public float delay;
        public bool flip;
        public bool rotate;
        public CardStates stateAfterAnimation;
    }

    public static void AddTweenToQueue(GenericCard target, Vector3 position, float time, float delay, CardStates stateAfterAnimation, bool flip, bool rotate, EaseType easeType = EaseType.easeInOutSine)
    {
        TweenObject newTween = new TweenObject();

        newTween.target = target.gameObject;
        newTween.position = position;
        newTween.time = time;
        newTween.delay = delay;
        newTween.easeType = easeType;
        newTween.stateAfterAnimation = stateAfterAnimation;
        newTween.flip = flip;
        newTween.rotate = rotate;

        TweenQueue.Enqueue(newTween);

        if (TweenQueueRunning == false)
        {
            TweenQueueRunning = true;
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
                TweenObject tween = TweenQueue.Dequeue();

                iTween.MoveTo(tween.target, iTween.Hash("time", tween.time,
                                                        "delay", tween.delay,
                                                       "position", tween.position,
                                                       "easetype", tween.easeType,
                                                       "islocal", true,
                                                       "oncomplete", "SwitchState",
                                                       "oncompletetarget", tween.target,
                                                       "oncompleteparams", tween.stateAfterAnimation
                                                       ));

                if (tween.flip) FlipCard(tween.target, tween.time);
                if (tween.rotate) RotateCard(tween.target, tween.time);

                yield return new WaitForSeconds(tween.time + tween.delay);
            }
        }
    }

    public static void TweenCardToPosition(GenericCard card, Vector3 cardPosition, CardStates stateAfterAnimation, float tweenDelay = 0f, EaseType easeType = iTween.EaseType.easeInOutSine)
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

    private static void RotateCard(GameObject card, float time)
    {
        //set target based on current rotation, use 20f as an epsilon value for comparison
        Vector3 cardRotation = card.transform.localRotation.eulerAngles;
        float target = cardRotation.z > 20f ? 0f : 270f;
        iTween.RotateTo(card.gameObject, iTween.Hash("time", time,
                                                     "z", target,
                                                     "easetype", iTween.EaseType.easeInOutSine,
                                                     "islocal", true
                                                     ));
    }

    private static void FlipCard(GameObject card, float time)
    {
        //set target based on current rotation, use 20f as an epsilon value for comparison
        Vector3 cardRotation = card.transform.localRotation.eulerAngles;
        float target = cardRotation.y > 20f ? 0f : 180f;
        iTween.RotateTo(card.gameObject, iTween.Hash("time", time,
                                                     "y", target,
                                                     "easetype", iTween.EaseType.easeInOutSine,
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
