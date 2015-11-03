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
        Creatures = 0, Charms, Transfiguration, Potions, Quidditch
    }

    public enum ClassificationTypes
    {
        CareOfMagicalCreatures, Charms, Transfiguration, Potions, Quidditch,
        Lesson,
        Character,
        Adventure
    }

    public enum FlipStates
    {
        FaceUp, FaceDown
    }
}
