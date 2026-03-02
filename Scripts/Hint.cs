using DG.Tweening;

using UnityEngine;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            [Header("• T W E E N   S E T T I N G S")]
            public float visibilityDuration = 0.2F;
            public Ease visibilityEase = Ease.OutBack;

            private Tween visibilityTween;


            private void Start()
            {
                  visibilityTween = transform.DOScale(Vector2.one, visibilityDuration).From(Vector2.zero)
                        .SetEase(visibilityEase)
                        .SetAutoKill(false)
                        .OnKill(() => visibilityTween = null)
                        .SetRecyclable(false)
                        .Pause();
            }
            private void OnDestroy()
            {
                  visibilityTween?.Kill();
            }
            private void OnDisable()
            {
                  transform.localScale = Vector2.zero;
            }

            public void Show() => visibilityTween.PlayForward();
            public void Show(float delay) => visibilityTween.Restart(true, delay);
            public void Hide() => visibilityTween.SmoothRewind();
      }
}