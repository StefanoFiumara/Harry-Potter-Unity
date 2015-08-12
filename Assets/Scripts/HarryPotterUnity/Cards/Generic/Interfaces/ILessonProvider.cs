public enum LessonTypes
{
    Creatures = 0, Charms, Transfiguration, Potions, Quidditch
}

namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface ILessonProvider
    {
        LessonTypes LessonType { get; }

        int AmountLessonsProvided { get; }
    }
}
