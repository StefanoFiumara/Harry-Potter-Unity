namespace HarryPotterUnity.Tween
{
    public interface ITweenObject
    {
        float CompletionTime { get; }

        bool WaitForCompletion { get; set; }

        void ExecuteTween();
    }
}
