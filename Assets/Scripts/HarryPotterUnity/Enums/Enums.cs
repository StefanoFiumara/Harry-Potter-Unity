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

    //TODO: Add [Flags] attribute and proper values
    public enum Tag
    {
        Unique, Healing, Wand, Cauldron, Broom
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
    }
}
