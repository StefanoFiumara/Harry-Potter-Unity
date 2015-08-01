using HarryPotterUnity.Cards.Generic;
using HarryPotterUnity.Cards.Generic.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class Unicorn : GenericCreature, IPersistentCard {

        public new void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }
    }
}
