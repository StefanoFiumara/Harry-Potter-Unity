using System.Collections.Generic;
using HarryPotterUnity.Tween;
using HarryPotterUnity.Utils;
using UnityEngine;

namespace HarryPotterUnity.Cards.Generic
{
    public abstract class GenericSpell : GenericCard {

        private static readonly Vector3 SpellOffset = new Vector3(0f, 0f, -400f);

        protected sealed override void OnClickAction(List<GenericCard> targets)
        {
            Enable();
            PreviewSpell();

            Player.Hand.Remove(this);
            Player.Discard.Add(this);

            SpellAction(targets);
        }

        protected abstract void SpellAction(List<GenericCard> targets);

        private void PreviewSpell()
        {
            State = CardStates.Discarded;
            var rotate180 = Player.OppositePlayer.IsLocalPlayer ? TweenQueue.RotationType.Rotate180 : TweenQueue.RotationType.NoRotate;
            GameManager.TweenQueue.AddTweenToQueue(new MoveTween(gameObject, SpellOffset, 0.5f, 0f, FlipStates.FaceUp, rotate180, State, 0.6f));
        }
    }
}