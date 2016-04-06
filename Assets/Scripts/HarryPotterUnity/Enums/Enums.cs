using System;

namespace HarryPotterUnity.Enums
{
    public enum State
    {
        InDeck, InHand, InPlay, Discarded
    }

    public enum Type
    {
        Lesson, Creature, Spell, Item, Location, Match, Adventure, Character
    }

    [Flags]
    public enum Tag
    {
        Unique      = 1 << 0,
        Healing     = 1 << 1,
        Wand        = 1 << 2,
        Cauldron    = 1 << 3,
        Broom       = 1 << 4
    }

    public enum Rarity
    {
        Common, Uncommon, Rare, UltraRare
    }

    public enum LessonTypes
    {
        Creatures, Charms, Transfiguration, Potions, Quidditch
    }


    public enum ClassificationTypes
    {
        CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch,
        Lesson,
        Character,
        Adventure
    }

    public enum FlipState
    {
        FaceUp, FaceDown
    }

    public enum TweenRotationType
    {
        NoRotate, Rotate90, Rotate180
    }

    public static class EnumExtensions
    {
        public static ClassificationTypes ToClassification(this LessonTypes type)
        {
            switch (type)
            {
                case LessonTypes.Creatures: return ClassificationTypes.CareOfMagicalCreatures;
                case LessonTypes.Charms: return ClassificationTypes.Charms;
                case LessonTypes.Transfiguration: return ClassificationTypes.Transfiguration;
                case LessonTypes.Quidditch: return ClassificationTypes.Quidditch;
                case LessonTypes.Potions: return ClassificationTypes.Potions;
                default:
                    throw new ArgumentException("Unable to map lesson type");
            }
        }

        public static bool IsTopRow(this Type type)
        {
            return type == Type.Item
                   || type == Type.Location
                   || type == Type.Adventure
                   || type == Type.Match;
        }
    }
}
