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
        protected readonly List<IPlayerConstraint> _constraints = new List<IPlayerConstraint>();

        protected abstract void AddConstraints();

        protected override void Start()
        {
            base.Start();
            this.AddConstraints();
        }

        public override void OnEnterInPlayAction()
        {
            foreach (var constraint in this._constraints)
            {
                this.Player.OppositePlayer.Constraints.Add(constraint);
            }
        }

        public override void OnExitInPlayAction()
        {
            foreach (var constraint in this._constraints)
            {
                this.Player.OppositePlayer.Constraints.Remove(constraint);
            }
        }
    }
}