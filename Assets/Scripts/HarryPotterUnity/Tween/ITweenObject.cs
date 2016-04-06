namespace HarryPotterUnity.Tween
{
    public interface ITweenObject
    {
        float CompletionTime { get; }
        float TimeUntilNextTween { get; }

        void ExecuteTween();
    }
}
