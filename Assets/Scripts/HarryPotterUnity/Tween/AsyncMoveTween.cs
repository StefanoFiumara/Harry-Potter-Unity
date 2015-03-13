using System;
using System.Collections.Generic;
using HarryPotterUnity.Cards.Generic;
using UnityEngine;

namespace HarryPotterUnity.Tween
{
    class AsyncMoveTween : ITweenObject
    {

        private readonly List<GenericCard> _targets;
        private readonly Func<GenericCard, Vector3> _getPosition; 

        public AsyncMoveTween(List<GenericCard> targets, Func<GenericCard, Vector3> getPositionAction)
        {
            _targets = targets;
            _getPosition = getPositionAction;
        }

        public float CompletionTime
        {
            get { return 0.5f; }
        }

        public void ExecuteTween()
        {
            foreach (var target in _targets)
            {
                iTween.MoveTo(target.gameObject, iTween.Hash(
                "time", 0.5f,
                "position", _getPosition(target),
                "easetype", iTween.EaseType.EaseInOutSine,
                "islocal", true
                ));    
            }
        }
    }
}
