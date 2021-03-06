﻿using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class PorcupineRobe : BaseItem
    {
        private readonly List<BaseCard> _enemyCreatures = new List<BaseCard>();

        public override void OnInPlayBeforeTurnAction()
        {
            foreach (var creature in this._enemyCreatures.Cast<BaseCreature>())
            {
                creature.TakeDamage(1);
            }

            this._enemyCreatures.Clear();
        }

        public override void OnEnterInPlayAction()
        {
            this.Player.OnDamageTakenEvent += this.CountCreatureDamage;
        }

        public override void OnExitInPlayAction()
        {
            this.Player.OnDamageTakenEvent -= this.CountCreatureDamage;
        }

        private void CountCreatureDamage(BaseCard source, int amount)
        {
            if (source.Type == Type.Creature)
            {
                this._enemyCreatures.Add(source);
            }
        }
    }
}