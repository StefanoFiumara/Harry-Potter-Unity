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

    struct TweenObject
    {
        public GameObject target;
        public float time;
        public Vector3 position;
        public EaseType easeType;
        public float delay;
    }

    public static void AddTweenToQueue(GameObject target, Vector3 position, float time, float delay = 0f, EaseType easeType = EaseType.easeInOutSine)
    {
        TweenObject newTween;
        newTween.target = target;
        newTween.position = position;
        newTween.time = time;
        newTween.delay = delay;
        newTween.easeType = easeType;

        TweenQueue.Enqueue(newTween);

        if (TweenQueueRunning == false)
        {
           // StartCoroutine(RunTweenQueue());
        }
    }

    private static IEnumerator RunTweenQueue()
    {
        //I might break the game here
        while (true)
        {
            if (TweenQueue.Count == 0) yield return null;

            TweenObject tween = TweenQueue.Dequeue();

            iTween.MoveTo(tween.target, iTween.Hash("time", tween.time,
                                                   "position", tween.position,
                                                   "easetype", tween.easeType,
                                                   "islocal", true,
                                                   "delay", tween.delay
                                                   ));
            yield return new WaitForSeconds(tween.time + tween.delay);

        }
    }



    public static void TweenCardToPosition(Transform card, Vector3 cardPosition, CardStates stateAfterAnimation, float tweenDelay = 0f, EaseType easeType = iTween.EaseType.easeInOutSine)
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

    public static void RotateCard(Transform card)
    {
        //set target based on current rotation, use 20f as an epsilon value for comparison
        Vector3 cardRotation = card.localRotation.eulerAngles;
        float target = cardRotation.z > 20f ? 0f : 270f;
        iTween.RotateTo(card.gameObject, iTween.Hash("time", 0.5f,
                                                     "z", target,
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
