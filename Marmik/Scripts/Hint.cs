using DG.Tweening;
using UnityEngine;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            private enum TransitionType { Scale, Fade }

            [Header("- A N I M A T I O N   S E T T I N G S")]
            [SerializeField] private TransitionType transitionType;
            [Space(2F)]
            public float visibilityDuration = 0.25F;
            public Ease visibilityEase = Ease.Linear;

            private Tween visibilityTween, delayedCall;
            private Vector3 originalScale;

            [field: SerializeField] public bool IsVisible { get; private set; }


            private void Awake()
            {
                  originalScale = transform.localScale;
            }
            private void OnEnable()
            {
                  switch (transitionType)
                  {
                        case TransitionType.Scale:
                              visibilityTween = transform.DOScale(originalScale, visibilityDuration).From(Vector2.zero);
                              break;

                        case TransitionType.Fade:
                              if (TryGetComponent(out SpriteRenderer renderer))
                              {
                                    visibilityTween = renderer.DOFade(1F, visibilityDuration).From(0F);
                              }
                              else
                              {
                                    Debug.LogWarning($"No {typeof(SpriteRenderer).Name} component found on {name}. Disabling the hint.");
                                    enabled = false;
                                    return;
                              }
                              break;
                  }
                  visibilityTween
                        .SetEase(visibilityEase)
                        .SetAutoKill(false)
                        .OnKill(() =>
                        {
                              visibilityTween = null;
                              IsVisible = false;
                        })
                        .Pause();
            }
            private void OnDisable()
            {
                  visibilityTween?.Kill();
            }

            public void Show()
            {
                  delayedCall?.Kill(false);

                  IsVisible = true;
                  visibilityTween.PlayForward();
            }
            public void Show(float delay)
            {
                  delayedCall?.Kill(false);
                  delayedCall = DOVirtual.DelayedCall(delay, Show).OnKill(() => delayedCall = null);
            }
            public void Hide()
            {
                  delayedCall?.Kill(false);

                  visibilityTween.PlayBackwards();
                  IsVisible = false;
            }
      }
}
