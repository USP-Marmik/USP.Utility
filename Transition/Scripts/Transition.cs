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


            public void Play(TweenCallback callback, float interval = 1F)
            {
                  Cancel();
                  sequence = DOTween.Sequence()
                        .AppendCallback(Initialize)
                        .Append(IntroTween)
                        .AppendCallback(callback)
                        .AppendInterval(interval)
                        .Append(ExitTween)
                        .SetLink(gameObject)
                        .SetRecyclable(true)
                        .OnComplete(OnFinish)
                        .Play();
            }
            public void Cancel()
            {
                  if (sequence != null && sequence.IsActive()) sequence.Kill(false);
                  sequence = null;
            }

            protected abstract void Initialize();
            protected virtual void OnFinish() { }
      }
}