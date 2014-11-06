using UnityEngine;
using System.Collections;
using CardStates = GenericCard.CardStates;

public class Helper {

    public static void TweenCardToPosition(Transform card, Vector3 cardPosition, CardStates stateAfterAnimation, float tweenDelay = 0f, float tweenTime = 0.5f, bool rotate = true)
    {
        iTween.MoveTo(card.gameObject, iTween.Hash("time", tweenTime,
                                                   "position", cardPosition,
                                                   "easetype", iTween.EaseType.easeInOutSine,
                                                   "islocal", true,
                                                   "delay", tweenDelay,
                                                   "oncomplete", "SwitchState",
                                                   "oncompletetarget", card.gameObject,
                                                   "oncompleteparams", stateAfterAnimation
                                                   ));

        if (rotate)
        {
            iTween.RotateTo(card.gameObject, iTween.Hash("time", tweenTime,
                                                         "z", 270f,
                                                         "easetype", iTween.EaseType.easeInOutSine,
                                                         "islocal", true
                                                         ));
        }

        iTween.ScaleTo(card.gameObject, iTween.Hash("x", 1, "y", 1, "time", tweenTime));
    }
}
