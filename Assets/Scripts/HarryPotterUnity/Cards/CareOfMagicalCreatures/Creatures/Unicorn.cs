using HarryPotterUnity.Cards.Interfaces;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    [UsedImplicitly]
    public class Unicorn : BaseCreature, IPersistentCard {

        public new void OnInPlayBeforeTurnAction()
        {
            Player.AddActions(1);
        }
    }
}
