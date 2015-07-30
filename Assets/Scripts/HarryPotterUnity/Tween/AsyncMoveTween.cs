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
        private readonly float _timeUntilNextTween;

        public AsyncMoveTween(List<GenericCard> targets, Func<GenericCard, Vector3> getPositionFunction, float timeUntilNextTween = 0f)
        {
            _targets = targets;
            _getPosition = getPositionFunction;
            _timeUntilNextTween = timeUntilNextTween;
        }

        public float CompletionTime
        {
            get { return 0.5f; }
        }

        public float TimeUntilNextTween
        {
            get { return _timeUntilNextTween; }
        }

        public void ExecuteTween()
        {
            foreach (var target in _targets)
            {
                //iTween.Stop(target.gameObject);

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
