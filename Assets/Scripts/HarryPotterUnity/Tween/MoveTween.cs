using System;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    public class MoveTween : ITweenObject
    {
        
        public GameObject Target { get; set; }
        public Vector3 Position { get; set; }
        public float Time { get; set; }
        public float Delay { get; set; }
        public FlipState Flip { get; set; }
        public TweenRotationType Rotate { get; set; }

        public float TimeUntilNextTween { get; set; }

        public Action OnCompleteCallback { private get; set; }

        public float CompletionTime
        {
            get { return Time + Delay; }
        }
        
        public void ExecuteTween()
        {
            iTween.MoveTo(Target, iTween.Hash(
                        "time", Time,
                        "delay", Delay,
                        "position", Position,
                        "easetype", iTween.EaseType.EaseInOutSine,
                        "islocal", true,
                        "oncomplete", OnCompleteCallback
                        ));

            RotateAndFlipCard();
        }

        private void RotateAndFlipCard()
        {
            float targetRotate = 0f;
            switch (Rotate)
            {
                case TweenRotationType.Rotate90:
                    targetRotate = 270f;
                    break;
                case TweenRotationType.Rotate180:
                    targetRotate = 180f;
                    break;
            }

            float targetFlip = 0f;
            switch (Flip)
            {
                    case FlipState.FaceUp:
                        targetFlip = 0f;
                        break;
                    case FlipState.FaceDown:
                        targetFlip = 180f;
                        break;
            }

            Target.GetComponent<BaseCard>().FlipState = Flip;

            iTween.RotateTo(Target, iTween.Hash("time", Time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
    }
}
