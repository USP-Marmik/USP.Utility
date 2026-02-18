using DG.Tweening;

using UnityEngine;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            private Tween visibilityTween;
            private Tweener moveTween;

            [Header("• T W E E N   S E T T I N G S")]
            public float visibilityDuration = 0.2F;
            public Ease visibilityEase = Ease.OutBack;
            [Space(2F)]
            public float moveDelay = 0.4F;
            public float moveDuration = 1F;
            public Ease moveEase = Ease.InOutExpo;

            [field: SerializeField] public Vector3 Target { get; set; }


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

                  moveTween = transform.DOMove(Target, moveDuration)
                        .SetEase(moveEase)
                        .SetDelay(moveDelay)
                        .OnComplete(Hide)
                        .OnKill(() => moveTween = null)
                        .SetAutoKill(false)
                        .Pause();
            }
            private void OnDisable()
            {
                  visibilityTween?.Kill();
                  moveTween?.Kill();
            }

            public void Show(Vector2 startPosition)
            {
                  visibilityTween.PlayForward();

                  moveTween.ChangeStartValue((Vector3) startPosition);
                  moveTween.Restart();
            }
            public void Hide()
            {
                  visibilityTween.PlayBackwards();

                  moveTween.Pause();
            }
      }
}