using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      public class Hint : MonoBehaviour
      {
            private enum TransitionType { Scale, Fade }

            [Header("- A N I M A T I O N   S E T T I N G S")]
            [SerializeField] private TransitionType transitionType;
            public float visibilityDuration = 0.2F;
            public Ease visibilityEase = Ease.OutBack;

            [Header("- E V E N T S")]
            public UnityEvent OnShow;
            public UnityEvent OnHide;

            private Tween visibilityTween, delayedCall;

            public bool IsVisible { get; private set; }


            private void OnEnable()
            {
                  switch (transitionType)
                  {
                        case TransitionType.Scale:
                              visibilityTween = transform.DOScale(Vector2.one, visibilityDuration).From(Vector2.zero);
                              break;

                        case TransitionType.Fade:
                              var renderer = GetComponent<SpriteRenderer>();
                              visibilityTween = renderer.DOFade(1F, visibilityDuration).From(0F);
                              break;
                  }
                  visibilityTween.SetEase(visibilityEase)
                        .SetAutoKill(false)
                        .OnKill(() =>
                        {
					transform.localScale = Vector3.zero;
					IsVisible = false;

                              visibilityTween = null;
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
                  OnShow.Invoke();
            }
            public void Show(float delay)
            {
                  visibilityTween.Restart(true, delay);

                  delayedCall?.Kill(false);
                  delayedCall = DOVirtual.DelayedCall(delay, OnShow.Invoke).OnComplete(() => IsVisible = true).OnKill(() => delayedCall = null);
            }
            public void Hide()
            {
                  delayedCall?.Kill(false);

                  IsVisible = false;
                  visibilityTween.PlayBackwards();
                  OnHide.Invoke();
            }
      }
}
