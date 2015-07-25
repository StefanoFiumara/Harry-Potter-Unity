namespace HarryPotterUnity.Tween
{
    public interface ITweenObject
    {
        float CompletionTime { get; }

        void ExecuteTween();
    }
}
