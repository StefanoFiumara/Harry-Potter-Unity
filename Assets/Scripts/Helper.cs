using UnityEngine;
using System.Collections;
using CardStates = GenericCard.CardStates;

public class Helper {

    public static void TweenCardToPosition(Transform card, Vector3 cardPosition, CardStates stateAfterAnimation, float tweenDelay = 0f, iTween.EaseType easeType = iTween.EaseType.easeInOutSine)
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
        iTween.RotateTo(card.gameObject, iTween.Hash("time", 0.5f,
                                                         "z", 270f,
                                                         "easetype", iTween.EaseType.easeInOutSine,
                                                         "islocal", true
                                                         ));
    }
}
