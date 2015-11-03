using System.Collections.Generic;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Cards
{
    public abstract class BaseSpell : BaseCard {

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        protected sealed override void OnClickAction(List<BaseCard> targets)
        {
            Enable();
            PreviewSpell();

            Player.Hand.Remove(this);
            Player.Discard.Add(this);

            SpellAction(targets);
        }

        protected abstract void SpellAction(List<BaseCard> targets);

        private void PreviewSpell()
        {
            State = State.Discarded;
            var rotate180 = Player.OppositePlayer.IsLocalPlayer ? TweenQueue.RotationType.Rotate180 : TweenQueue.RotationType.NoRotate;
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(gameObject, SpellOffset, 0.5f, 0f, FlipStates.FaceUp, rotate180, State, 0.6f));
        }
    }
}