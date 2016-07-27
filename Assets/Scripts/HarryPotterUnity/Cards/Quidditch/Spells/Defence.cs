using System.Collections.Generic;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Quidditch.Spells
{
    public class Defence : BaseSpell
    {
        protected override void SpellAction(List<BaseCard> targets)
        {
            //Creature, Spell, Item, Location, Match, Adventure, Character
            Player.TypeImmunity.Add(Type.Creature);
            Player.TypeImmunity.Add(Type.Spell);
            Player.TypeImmunity.Add(Type.Item);
            Player.TypeImmunity.Add(Type.Location);
            Player.TypeImmunity.Add(Type.Match);
            Player.TypeImmunity.Add(Type.Adventure);
            Player.TypeImmunity.Add(Type.Character);

            Player.OnNextTurnStartEvent += () =>
            {
                Player.TypeImmunity.Remove(Type.Creature);
                Player.TypeImmunity.Remove(Type.Spell);
                Player.TypeImmunity.Remove(Type.Item);
                Player.TypeImmunity.Remove(Type.Location);
                Player.TypeImmunity.Remove(Type.Match);
                Player.TypeImmunity.Remove(Type.Adventure);
                Player.TypeImmunity.Remove(Type.Character);
            };
        }
    }
}