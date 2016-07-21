using HarryPotterUnity.Enums;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.PlayerConstraints
{
    public class CannotPlayTypeConstraint : IPlayerConstraint
    {
        private readonly Type _disallowedType;

        public CannotPlayTypeConstraint(Type disallowedType)
        {
            _disallowedType = disallowedType;
        }
        public bool MeetsConstraint(BaseCard cardToPlay)
        {
            return cardToPlay.Type != _disallowedType;
        }
    }
}