using HarryPotterUnity.Game;

namespace HarryPotterUnity.Cards.Potions.Locations
{
    public class MoaningMyrtlesBathroom : BaseLocation
    {
        public override void OnInPlayBeforeTurnAction()
        {
            this.CheckCharacter(this.Player);
        }

        public override void OnInPlayAfterTurnAction()
        {
            this.CheckCharacter(this.Player.OppositePlayer);
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