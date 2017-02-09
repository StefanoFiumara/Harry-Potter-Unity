using System.Collections.Generic;
using HarryPotterUnity.Cards;
using HarryPotterUnity.Enums;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    public class FlipCardsTween : ITweenObject
    {
        
        public List<GameObject> Targets { get; set; }
        public float TimeUntilNextTween { get; set; }
        public FlipState Flip { get; set; }
        
        public float CompletionTime
        {
            get { return 0.3f; }
        }
        
        public void ExecuteTween()
        {
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

            foreach (var target in this.Targets)
            {
                target.GetComponent<BaseCard>().FlipState = this.Flip;

                iTween.RotateTo(target, iTween.Hash("time", this.CompletionTime,
                    "y", targetFlip,
                    "easetype", iTween.EaseType.EaseInOutSine,
                    "islocal", true
                    ));
            }
            
        }
    }
}
