using System;

using UnityEngine;

using DG.Tweening;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer), typeof(DraggableObject), typeof(Rigidbody2D))]
      public class Piece : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new SpriteRenderer renderer;
            [SerializeField] private new Collider2D collider;
            [SerializeField] private DraggableObject draggable;
            private new Transform transform;
            [SerializeField] private Sprite keySprite;

            [Header("• T W E E N   S E T T I N G S")]
            public float embiggenTarget = 1;
            [Min(0F)] public float embiggenTweenDuration = 0.3F;
            public Ease embiggenTweenEase = Ease.OutBack;

            private Tween embiggenTween;

            [Space(2F)]
            public float attachTweenScaleTarget = 1F;
            [Min(0F)] public float attachTweenMoveDuration = 0.25F, attachTweenScaleDuration = 0.5F;
            public Ease attachTweenMoveEase = Ease.InOutQuint, attachTweenScaleEase = Ease.OutBounce;

            private string key;
            private Slot matchingSlot;


            public event Action Selected = delegate { }, Canceled = delegate { }, Attached = delegate { };


            private void Reset()
            {
                  renderer = GetComponent<SpriteRenderer>();
                  collider = GetComponent<Collider2D>();
                  draggable = GetComponent<DraggableObject>();
            }
            private void Awake()
            {
                  transform = base.transform;

                  key = keySprite != null ? keySprite.name : renderer.sprite.name;
            }
            private void OnEnable()
            {
                  draggable.enabled = collider.enabled = true;

                  draggable.OnPick.AddListener(HandleSelected);
                  draggable.OnRelease.AddListener(HandleReleased);

                  embiggenTween = transform.DOScale(Vector2.one * embiggenTarget, embiggenTweenDuration)
                        .SetEase(embiggenTweenEase)
                        .SetAutoKill(false)
                        .OnKill(() => embiggenTween = null)
                        .Pause();
            }
            private void OnDisable()
            {
                  draggable.OnPick.RemoveListener(HandleSelected);
                  draggable.OnRelease.RemoveListener(HandleReleased);

                  draggable.enabled = collider.enabled = false;

                  embiggenTween?.Kill();
            }
            private void OnTriggerEnter2D(Collider2D other)
            {
                  if (!other.TryGetComponent(out Slot slot) || key != slot.Key) return;

                  slot.Fade(0.5F);
                  matchingSlot = slot;
            }
            private void OnTriggerExit2D(Collider2D other)
            {
                  if (!other.TryGetComponent(out Slot slot) || key != slot.Key) return;

                  if (enabled) slot.Fade(1F);
                  matchingSlot = null;
            }

            private void HandleSelected()
            {
                  embiggenTween.Restart();

                  Selected();
            }
            private void HandleReleased()
            {
                  if (matchingSlot != null)
                  {
                        AttachToSlot(matchingSlot);
                        return;
                  }
                  embiggenTween.SmoothRewind();

                  Canceled();
            }
            private void AttachToSlot(Slot slot)
            {
                  enabled = false;

                  transform.SetParent(slot.transform, true);

                  DOTween.Sequence()
                        .Append(transform.DOLocalMove(Vector2.zero, attachTweenMoveDuration).SetEase(attachTweenMoveEase).OnComplete(() => slot.Fade(0F)))
                        .AppendCallback(() => { if (keySprite != null) renderer.sprite = keySprite; })
                        .Append(transform.DOScale(Vector2.one * attachTweenScaleTarget, attachTweenScaleDuration).SetEase(attachTweenScaleEase))
                        .SetLink(gameObject)
                        .Play();

                  renderer.sortingOrder = slot.Order + 1;
                  Attached();
            }
      }
}