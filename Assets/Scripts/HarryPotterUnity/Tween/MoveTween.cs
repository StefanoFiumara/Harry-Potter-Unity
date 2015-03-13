using HarryPotterUnity.Cards.Generic;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    class MoveTween : ITweenObject
    {
        
        private readonly GameObject _target;
        private readonly Vector3 _position;
        private readonly float _time;
        private readonly float _delay;
        private readonly bool _flip;
        private readonly TweenQueue.RotationType _rotate;
        private readonly GenericCard.CardStates _stateAfterAnimation;

        public MoveTween(GameObject target, Vector3 position, float time, float delay, bool flip, TweenQueue.RotationType rotate, GenericCard.CardStates stateAfterAnimation)
        {
            _target = target;
            _position = position;
            _time = time;
            _delay = delay;
            _flip = flip;
            _rotate = rotate;
            _stateAfterAnimation = stateAfterAnimation;
        }


        public float CompletionTime
        {
            get { return _time + _delay; }
        }

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
            var cardRotation = _target.transform.localRotation.eulerAngles;
            var targetFlip = _flip ? (cardRotation.y > 20f ? 0f : 180f) : cardRotation.y;

            var targetRotate = 0f;
            switch (_rotate)
            {
                case TweenQueue.RotationType.Rotate90:
                    targetRotate = 270f;
                    break;
                case TweenQueue.RotationType.Rotate180:
                    targetRotate = 180f;
                    break;
            }

            if (_flip) _target.GetComponent<GenericCard>().SwitchFlipState();

            iTween.RotateTo(_target, iTween.Hash("time", _time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
    }
}
