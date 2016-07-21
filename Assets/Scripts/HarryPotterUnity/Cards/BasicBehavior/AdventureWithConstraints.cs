using System.Collections.Generic;
using HarryPotterUnity.Cards.PlayerConstraints;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.BasicBehavior
{
    public abstract class AdventureWithConstraints : BaseAdventure
    {
        [Header("Adventure Settings")]
        [SerializeField, UsedImplicitly]
        protected List<IPlayerConstraint> _constraints = new List<IPlayerConstraint>();

        protected abstract void AddConstraints();

        protected override void Start()
        {
            base.Start();
            AddConstraints();
        }

        public override void OnEnterInPlayAction()
        {
            foreach (var constraint in _constraints)
            {
                Player.OppositePlayer.Constraints.Add(constraint);
            }
        }

        public override void OnExitInPlayAction()
        {
            foreach (var constraint in _constraints)
            {
                Player.OppositePlayer.Constraints.Remove(constraint);
            }
        }
    }
}