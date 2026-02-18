using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer))]
      public class Icon : MonoBehaviour, IPointerDownHandler
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Piece piece;

            [Header("• T W E E N   S E T T I N G S")]
            public float collapseTweenDuration = 0.5F;
            public Ease collapseTweenEase = Ease.InBack;
            private Tween collapseTween;
            [Space(2F)]
            public float punchTarget = 0.2F;
            public float punchTweenDuration = 0.25F;
            private Tween punchTween;

            [Header("• E V E N T S")]
            public UnityEvent OnCollapse;
            public UnityEvent OnExpand;
            public UnityEvent OnHide;


            private void Reset()
            {
                  piece = GetComponentInChildren<Piece>();
            }
            private void OnEnable()
            {
                  piece.enabled = true;

                  piece.Selected += Collapse;
                  piece.Canceled += Expand;
                  piece.Attached += Hide;

                  collapseTween = transform.DOScale(Vector2.zero, collapseTweenDuration)
                        .SetEase(collapseTweenEase)
                        .SetAutoKill(false)
                        .OnKill(() => collapseTween = null)
                        .Pause();

                  punchTween = transform.DOPunchScale(Vector2.one * punchTarget, punchTweenDuration, 1, 1F).SetRelative().SetAutoKill(false).OnKill(() => punchTween = null).Pause();
            }
            private void OnDisable()
            {
                  piece.Selected -= Collapse;
                  piece.Canceled -= Expand;
                  piece.Attached -= Hide;

                  piece.enabled = false;

                  collapseTween?.Kill();
                  punchTween?.Kill();
            }

            public void OnPointerDown(PointerEventData _)
            {
                  if (!piece.Interactable) punchTween.Restart();
            }

            private void Collapse()
            {
                  collapseTween.Restart();
                  OnCollapse.Invoke();
            }
            private void Expand()
            {
                  collapseTween.PlayBackwards();
                  OnExpand.Invoke();
            }
            private void Hide()
            {
                  gameObject.SetActive(false);
                  OnHide.Invoke();
            }
      }
}