using DG.Tweening;

using UnityEngine;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            [Header("• T W E E N   S E T T I N G S")]
            public float visibilityDuration = 0.2F;
            public Ease visibilityEase = Ease.OutBack;
            public float moveDuration = 0.5F;
            public Ease moveEase = Ease.InOutSine;

            private Tween visibilityTween, moveTween;


            private void OnEnable()
            {
                  visibilityTween ??= transform.DOScale(Vector2.one, visibilityDuration).From(Vector2.zero)
                        .SetEase(visibilityEase)
                        .OnKill(() =>
                        {
                              transform.localScale = Vector2.zero;
                              visibilityTween = null;
                        })
                        .SetAutoKill(false)
                        .Pause();
            }
            private void OnDisable()
            {
                  visibilityTween?.Kill();
            }

            public void Show()
            {
                  visibilityTween.PlayForward();
            }
            public void Show(float delay)
            {
                  visibilityTween.Restart(true, delay);
            }
            public void Show(Vector3 from, Vector3 to)
            {
                  transform.position = from;

                  moveTween?.Kill(false);
                  moveTween = transform.DOMove(to, moveDuration).SetEase(moveEase).OnComplete(Hide).OnKill(() => moveTween = null);

                  Show();
            }
            public void Hide()
            {
                  visibilityTween.PlayBackwards();
            }
      }
}