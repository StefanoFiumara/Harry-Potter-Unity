using HarryPotterUnity.Cards.BasicBehavior;
using HarryPotterUnity.Enums;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    [UsedImplicitly]
    public class NimbusTwoThousandOne : NimbusTwoThousand
    {
       
        protected override void Start()
        {
            base.Start();
            _damageAmount = 3;
        }
    }
}