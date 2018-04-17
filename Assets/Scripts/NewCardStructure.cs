using System;
using System.Collections;
using System.Collections.Generic;

namespace HPTCG
{
    public enum RarityType
    {
        Common, Uncommon, Rare, UltraRare
    }

    public enum ClassificationType
    {
        CareOfMagicalCreatures,
        Charms,
        Transfiguration,
        Potions,
        Quidditch,
        Lesson,
        Character,
        Adventure
    }

    public enum LessonTypes
    {
        Creatures, Charms, Transfiguration, Potions, Quidditch
    }

    [Flags]
    public enum Tag
    {
        Unique = 1 << 0,
        Healing = 1 << 1,
        Wand = 1 << 2,
        Cauldron = 1 << 3,
        Broom = 1 << 4,
        Plant = 1 << 5,
        Owl = 1 << 6
    }

    public interface ICard
    {
        int Id { get; }

        string Name { get; }
        string Description { get; }

        RarityType Rarity { get; }
        ClassificationType Classification { get; }

        bool HasTag(Tag t);
        
        int ActionCost { get; }

        bool IsPlayable(GameState gameState);
    }

    public interface ILessonProvider
    {
        int AmountProvided { get; }
        LessonTypes LessonType { get; } 
    }

    public class Player
    {
        public ICard StartingCharacter { get; }

        public List<ICard> Deck { get; }
        public List<ICard> Hand { get; }
        public List<ICard> Board { get; }
        public List<ICard> Discard { get; }
    }

    public class GameState
    {
        public Player Player1 { get; }
        public Player Player2 { get; }
    }
    
}
