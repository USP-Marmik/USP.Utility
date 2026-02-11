using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer))]
      public class Icon : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Piece piece;

            [Header("• C O N F I G U R A T I O N   -   I C O N")]
            public float collapseDuration = 0.5F;
            public Ease collapseEase = Ease.InBack;

            [Header("• E V E N T S")]
            public UnityEvent OnCollapse;
            public UnityEvent OnExpand;
            public UnityEvent OnHide;

            private new Transform transform;
            private Tween collapseTween;


            private void Reset()
            {
                  piece = GetComponentInChildren<Piece>();
            }
            private void Awake()
            {
                  transform = base.transform;
            }
            private void OnEnable()
            {
                  piece.enabled = true;

                  piece.Selected += Collapse;
                  piece.Canceled += Expand;
                  piece.Attached += Hide;

                  collapseTween = transform.DOScale(Vector2.zero, collapseDuration)
                        .SetEase(collapseEase)
                        .SetAutoKill(false)
                        .OnKill(() => collapseTween = null)
                        .Pause();
            }
            private void OnDisable()
            {
                  piece.Selected -= Collapse;
                  piece.Canceled -= Expand;
                  piece.Attached -= Hide;

                  piece.enabled = false;

                  collapseTween?.Kill();
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