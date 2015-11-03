using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface ILessonProvider
    {
        LessonTypes LessonType { get; }

        int AmountLessonsProvided { get; }
    }
}
