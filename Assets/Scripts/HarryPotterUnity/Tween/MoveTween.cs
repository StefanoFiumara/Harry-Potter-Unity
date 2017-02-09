using System;
using System.Collections;
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
            get { return this.Time + this.Delay; }
        }
        
        public void ExecuteTween()
        {
            var args = iTween.Hash(
                "time", this.Time,
                "delay", this.Delay,
                "position", this.Position,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true);

            if (this.OnCompleteCallback != null)
            {
                args["oncomplete"] = this.OnCompleteCallback;
            }

            iTween.MoveTo(this.Target, args);

            this.RotateAndFlipCard();
        }

        private void RotateAndFlipCard()
        {
            float targetRotate = 0f;
            switch (this.Rotate)
            {
                case TweenRotationType.Rotate90:
                    targetRotate = 270f;
                    break;
                case TweenRotationType.Rotate180:
                    targetRotate = 180f;
                    break;
            }

            float targetFlip = 0f;
            switch (this.Flip)
            {
                    case FlipState.FaceUp:
                        targetFlip = 0f;
                        break;
                    case FlipState.FaceDown:
                        targetFlip = 180f;
                        break;
            }

            this.Target.GetComponent<BaseCard>().FlipState = this.Flip;

            iTween.RotateTo(this.Target, iTween.Hash("time", this.Time,
                "y", targetFlip,
                "z", targetRotate,
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));
        }
    }
}
