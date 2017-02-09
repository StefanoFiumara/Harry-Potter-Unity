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
            this._disallowedType = disallowedType;
        }
        public bool MeetsConstraint(BaseCard cardToPlay)
        {
            return cardToPlay.Type != this._disallowedType;
        }
    }
}