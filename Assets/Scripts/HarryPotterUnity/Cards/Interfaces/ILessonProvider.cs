using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Interfaces
{
    public interface ILessonProvider
    {
        LessonTypes LessonType { get; }

        int AmountLessonsProvided { get; set; }
    }
}
