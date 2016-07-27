using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class PorcupineRobe : BaseItem
    {
        private readonly List<BaseCard> _enemyCreatures = new List<BaseCard>();

        public override void OnInPlayBeforeTurnAction()
        {
            foreach (var creature in _enemyCreatures.Cast<BaseCreature>())
            {
                creature.TakeDamage(1);
            }

            _enemyCreatures.Clear();
        }

        public override void OnEnterInPlayAction()
        {
            Player.OnDamageTakenEvent += CountCreatureDamage;
        }

        public override void OnExitInPlayAction()
        {
            Player.OnDamageTakenEvent -= CountCreatureDamage;
        }

        private void CountCreatureDamage(BaseCard source, int amount)
        {
            if (source.Type == Type.Creature)
            {
                _enemyCreatures.Add(source);
            }
        }
    }
}