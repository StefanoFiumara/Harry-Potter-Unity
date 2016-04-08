using System.Linq;
using UnityEngine;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Creatures
{
    public class WhompingWillow : BaseCreature
    {
        public override void OnInPlayBeforeTurnAction()
        {
            if (Player.OppositePlayer.InPlay.Items.Any())
            {
                var item =
                    Player.OppositePlayer.InPlay.Items.Skip(Random.Range(0, Player.OppositePlayer.InPlay.Items.Count))
                        .First();

                Player.OppositePlayer.Discard.Add(item);
            }
        }
    }
}