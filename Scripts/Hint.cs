using DG.Tweening;

using UnityEngine;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            private Tween visibilityTween, moveTween;

            [Header("• T W E E N   S E T T I N G S")]
            public float visibilityDuration = 0.2F;
            public Ease visibilityEase = Ease.OutBack;

            public float moveDuration = 0.5F;
            public Ease moveEase = Ease.InOutSine;


            private void OnEnable()
            {
                  visibilityTween = transform.DOScale(Vector2.one, visibilityDuration)
                        .SetEase(visibilityEase)
                        .OnKill(() =>
                        {
                              visibilityTween = null;
                              transform.localScale = Vector2.zero;
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
            public void Show(Vector3 from, Vector3 to)
            {
                  transform.position = from;

                  moveTween?.Kill(false);
                  moveTween = transform.DOMove(to, moveDuration).SetEase(moveEase).OnComplete(Hide).OnKill(() => moveTween = null);

                  visibilityTween.PlayForward();
            }
            public void Hide()
            {
                  visibilityTween.PlayBackwards();
            }
      }
}