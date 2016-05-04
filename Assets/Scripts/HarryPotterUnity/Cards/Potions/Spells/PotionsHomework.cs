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

        protected override void SpellAction(List<BaseCard> targets)
        {
            LessonsFound = 0;

            DamageAmount = 0;

            while (LessonsFound < LessonTargetCount)
            {
                int cardIndex = (Player.OppositePlayer.Deck.Cards.Count - 1) - DamageAmount;

                if (cardIndex < 0)
                {
                    break;
                }

                var card = Player.OppositePlayer.Deck.Cards[cardIndex];

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