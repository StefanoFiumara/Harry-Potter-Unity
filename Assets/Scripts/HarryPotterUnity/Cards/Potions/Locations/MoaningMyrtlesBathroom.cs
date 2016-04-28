using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Potions.Locations
{
    public class MoaningMyrtlesBathroom : BaseLocation
    {
        public override void OnInPlayBeforeTurnAction()
        {
            CheckCharacter(Player);
        }

        public override void OnInPlayAfterTurnAction()
        {
            CheckCharacter(Player.OppositePlayer);
        }

        private void CheckCharacter(Player p)
        {
            if (p.InPlay.Characters.Count >= 2)
            {
                p.AddActions(1);
            }
        }
    }
}