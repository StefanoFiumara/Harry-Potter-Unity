using System.Linq;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class WhompingWillow : BaseCreature
    {
        public override void OnInPlayBeforeTurnAction()
        {
            if (this.Player.OppositePlayer.InPlay.Items.Any())
            {
                var item = this.Player.OppositePlayer.InPlay.Items.Skip(Random.Range(0, this.Player.OppositePlayer.InPlay.Items.Count))
                        .First();

                this.Player.OppositePlayer.Discard.Add(item);
            }
        }
    }
}