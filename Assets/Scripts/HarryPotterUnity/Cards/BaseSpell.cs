using System.Collections.Generic;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Game;
using HarryPotterUnity.Tween;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseSpell : BaseCard
    {
        private static readonly Vector3 _spellOffset = new Vector3(0f, 0f, -400f);

        protected sealed override void OnPlayFromHandAction(List<BaseCard> targets)
        {
            Enable();
            PreviewSpell();
            
            Player.Discard.Add(this);

            SpellAction(targets);
        }

        protected abstract void SpellAction(List<BaseCard> targets);

        private void PreviewSpell()
        {
            State = State.Discarded;
            var rotateType = Player.OppositePlayer.IsLocalPlayer ? TweenRotationType.Rotate180 : TweenRotationType.NoRotate;

            var tween = new MoveTween
            {
                Target = gameObject,
                Position = _spellOffset,
                Time = 0.5f,
                Flip = FlipState.FaceUp,
                Rotate = rotateType,
                TimeUntilNextTween = 0.6f
            };
            GameManager.TweenQueue.AddTweenToQueue(tween);
        }

        protected sealed override Type GetCardType()
        {
            return Type.Spell;
        }
    }
}