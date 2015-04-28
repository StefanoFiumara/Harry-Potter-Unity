using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Creatures
{
    [UsedImplicitly]
    public class Unicorn : GenericCreature, IPersistentCard {

        public new void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }
    }
}
