using DG.Tweening;

using UnityEngine;

namespace USP
{
      [DisallowMultipleComponent]
      public abstract class Transition : MonoBehaviour
      {
            private Sequence sequence;

            protected abstract Tween IntroTween { get; }
            protected abstract Tween ExitTween { get; }


            protected virtual void OnDisable()
            {
                  if (sequence != null && sequence.IsActive()) sequence.Kill(false);
                  sequence = null;
            }

            public void Play(TweenCallback midpoint, float hold, TweenCallback finish = null)
            {
                  OnDisable();
                  Initialize();
                  sequence = DOTween.Sequence()
                        .Append(IntroTween).AppendCallback(midpoint).AppendInterval(hold).Append(ExitTween)
                        .SetLink(gameObject).SetRecyclable(true).Play()
                        .OnComplete(finish);
            }

            protected abstract void Initialize();
      }
}