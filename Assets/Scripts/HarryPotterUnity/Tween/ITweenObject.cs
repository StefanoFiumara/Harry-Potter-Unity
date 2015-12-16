using HarryPotterUnity.Cards;

namespace HarryPotterUnity.Tween
{
    public interface ITweenObject
    {
        float CompletionTime { get; }
        float TimeUntilNextTween { get; }

        /// <summary>
        /// The TweenSource is a card that will be highlighted on the board while the tween is being executed
        /// </summary>
        BaseCard TweenSource { get; }

        void ExecuteTween();
    }
}
