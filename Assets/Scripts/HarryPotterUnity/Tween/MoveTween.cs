﻿using HarryPotterUnity.Cards.Generic;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    class MoveTween : ITweenObject
    {
        
        private readonly GameObject _target;
        private readonly Vector3 _position;
        private readonly float _time;
        private readonly float _delay;
        private readonly GenericCard.FlipStates _flip;
        private readonly TweenQueue.RotationType _rotate;
        private readonly GenericCard.CardStates _stateAfterAnimation;

        public MoveTween(GameObject target, Vector3 position, float time, float delay, GenericCard.FlipStates flip, TweenQueue.RotationType rotate, GenericCard.CardStates stateAfterAnimation)
        {
            _target = target;
            _position = position;
            _time = time;
            _delay = delay;
            _flip = flip;
            _rotate = rotate;
            _stateAfterAnimation = stateAfterAnimation;

            WaitForCompletion = true;
        }


        public float CompletionTime
        {
            get { return _time + _delay; }
        }

        public bool WaitForCompletion { get; set; }

        public void ExecuteTween()
        {
            iTween.MoveTo(_target, iTween.Hash(
                        "time", _time,
                        "delay", _delay,
                        "position", _position,
                        "easetype", iTween.EaseType.EaseInOutSine,
                        "islocal", true,
                        "oncomplete", "SwitchState",
                        "oncompletetarget", _target,
                        "oncompleteparams", _stateAfterAnimation
                        ));

            RotateAndFlipCard();
        }

        private void RotateAndFlipCard()
        {
            float targetRotate = 0f;
            switch (_rotate)
            {
                case TweenQueue.RotationType.Rotate90:
                    targetRotate = 270f;
                    break;
                case TweenQueue.RotationType.Rotate180:
                    targetRotate = 180f;
                    break;
            }

            float targetFlip = 0f;
            switch (_flip)
            {
                    case GenericCard.FlipStates.FaceUp:
                        targetFlip = 0f;
                        break;
                    case GenericCard.FlipStates.FaceDown:
                        targetFlip = 180f;
                        break;
            }

            _target.GetComponent<GenericCard>().FlipState = _flip;

            iTween.RotateTo(_target, iTween.Hash("time", _time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
    }
}
