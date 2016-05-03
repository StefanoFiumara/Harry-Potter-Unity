using System.Collections.Generic;
using HarryPotterUnity.Cards.Interfaces;

namespace HarryPotterUnity.Cards.Potions.Spells
{
    public class PotionsHomework : BaseSpell, IDamageSpell
    {
        public int DamageAmount { get; set; }

        private int LessonsFound { get; set; }

        protected int LessonTargetCount { private get; set; }

        protected override void Start()
        {
            base.Start();
            LessonTargetCount = 1;
        }

        //TODO: Test this
        protected override void SpellAction(List<BaseCard> targets)
        {
            LessonsFound = 0;

            DamageAmount = 1;

            while (LessonsFound < LessonTargetCount)
            {
                if (Player.OppositePlayer.Deck.Cards.Count - DamageAmount < 0)
                {
                    break;
                }

                var card = Player.OppositePlayer.Deck.Cards[Player.OppositePlayer.Deck.Cards.Count - DamageAmount];

                if (card is BaseLesson)
                {
                    LessonsFound++;
                }

                DamageAmount++;
            }

            Player.OppositePlayer.TakeDamage(this, DamageAmount);
        }
    }
}