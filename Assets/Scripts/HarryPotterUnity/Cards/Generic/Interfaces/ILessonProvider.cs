namespace HarryPotterUnity.Cards.Generic.Interfaces
{
    public interface ILessonProvider
    {
        Lesson.LessonTypes LessonType { get; }

        int AmountLessonsProvided { get; }
    }
}
