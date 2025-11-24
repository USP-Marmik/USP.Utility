using DG.Tweening;

using UnityEngine;

namespace USP.Utility
{
      [DisallowMultipleComponent]
      public abstract class Transition : MonoBehaviour
      {
            private Sequence sequence;

            protected abstract void Initialize();
            protected abstract Tween Intro { get; }
            protected abstract Tween Outro { get; }


            protected virtual void OnDisable() => Cancel();

            public void Play(TweenCallback midpoint, float hold = 0F, TweenCallback complete = null)
            {
                  Cancel();
                  Initialize();

                  sequence = DOTween.Sequence();
                  sequence.Append(Intro).AppendCallback(midpoint);
                  if (hold > 0F) sequence.AppendInterval(hold);
                  sequence.Append(Outro).SetLink(gameObject).SetRecyclable(true).Play().OnComplete(complete);
            }
            public void Cancel()
            {
                  if (sequence == null || !sequence.IsActive()) return;

                  sequence.Kill(false);
                  sequence = null;
            }
      }
}