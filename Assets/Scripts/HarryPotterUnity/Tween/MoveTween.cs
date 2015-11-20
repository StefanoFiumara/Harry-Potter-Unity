using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    class MoveTween : ITweenObject
    {
        
        private readonly GameObject _target;
        private readonly Vector3 _position;
        private readonly float _time;
        private readonly float _delay;
        private readonly float _timeUntilNextTween;
        private readonly FlipState _flip;
        private readonly TweenRotationType _rotate;
        private readonly State _stateAfterAnimation;

        public MoveTween(GameObject target, Vector3 position, float time, float delay, FlipState flip, TweenRotationType rotate, State stateAfterAnimation, float timeUntilNextTween = 0f)
        {
            _target = target;
            _position = position;
            _time = time;
            _delay = delay;
            _flip = flip;
            _rotate = rotate;
            _stateAfterAnimation = stateAfterAnimation;
            _timeUntilNextTween = timeUntilNextTween;
        }


        public float CompletionTime
        {
            get { return _time + _delay; }
        }

        public float TimeUntilNextTween
        {
            get { return _timeUntilNextTween; }
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
            float targetRotate = 0f;
            switch (_rotate)
            {
                case TweenRotationType.Rotate90:
                    targetRotate = 270f;
                    break;
                case TweenRotationType.Rotate180:
                    targetRotate = 180f;
                    break;
            }

            float targetFlip = 0f;
            switch (_flip)
            {
                    case FlipState.FaceUp:
                        targetFlip = 0f;
                        break;
                    case FlipState.FaceDown:
                        targetFlip = 180f;
                        break;
            }

            _target.GetComponent<BaseCard>().FlipState = _flip;

            iTween.RotateTo(_target, iTween.Hash("time", _time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
    }
}
