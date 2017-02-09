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
            this.LessonTargetCount = 1;
        }

        protected override void SpellAction(List<BaseCard> targets)
        {
            this.LessonsFound = 0;

            this.DamageAmount = 0;

            while (this.LessonsFound < this.LessonTargetCount)
            {
                int cardIndex = (this.Player.OppositePlayer.Deck.Cards.Count - 1) - this.DamageAmount;

                if (cardIndex < 0)
                {
                    break;
                }

                var card = this.Player.OppositePlayer.Deck.Cards[cardIndex];

                if (card is BaseLesson)
                {
                    this.LessonsFound++;
                }

                this.DamageAmount++;
            }

            this.Player.OppositePlayer.TakeDamage(this, this.DamageAmount);
        }
    }
}