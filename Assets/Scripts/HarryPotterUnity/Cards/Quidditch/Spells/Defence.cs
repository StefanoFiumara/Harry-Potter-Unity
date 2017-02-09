using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class Defence : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            //Creature, Spell, Item, Location, Match, Adventure, Character
            this.Player.TypeImmunity.Add(Type.Creature);
            this.Player.TypeImmunity.Add(Type.Spell);
            this.Player.TypeImmunity.Add(Type.Item);
            this.Player.TypeImmunity.Add(Type.Location);
            this.Player.TypeImmunity.Add(Type.Match);
            this.Player.TypeImmunity.Add(Type.Adventure);
            this.Player.TypeImmunity.Add(Type.Character);

            this.Player.OnNextTurnStartEvent += () =>
            {
                this.Player.TypeImmunity.Remove(Type.Creature);
                this.Player.TypeImmunity.Remove(Type.Spell);
                this.Player.TypeImmunity.Remove(Type.Item);
                this.Player.TypeImmunity.Remove(Type.Location);
                this.Player.TypeImmunity.Remove(Type.Match);
                this.Player.TypeImmunity.Remove(Type.Adventure);
                this.Player.TypeImmunity.Remove(Type.Character);
            };
        }
    }
}