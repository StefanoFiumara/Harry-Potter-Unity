using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    public class AsyncMoveTween : ITweenObject
    {

        public List<BaseCard> Targets { get; set; }
        public Func<BaseCard, Vector3> GetPosition { get; set; }
        public float TimeUntilNextTween { get; set; }
        
        public float CompletionTime
        {
            get { return 0.2f; }
        }
        
        public void ExecuteTween()
        {
            foreach (var target in this.Targets)
            {
                iTween.MoveTo(target.gameObject, iTween.Hash(
                "time", 0.2f,
                "position", this.GetPosition(target),
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));    
            }
        }
    }
}
