using System;
using UnityEngine;
using DG.Tweening;

namespace USP.Utility
{
      [RequireComponent(typeof(Collider2D))]
      [RequireComponent(typeof(SpriteRenderer), typeof(DraggableObject), typeof(Rigidbody2D))]
      public class Piece : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new SpriteRenderer renderer;
            [SerializeField] private new Collider2D collider;
            [SerializeField] private DraggableObject draggable;
            [SerializeField] private Sprite keySprite;
            private new Transform transform;

            private Slot matchingSlot;
            private string key;
            private int originalSortingOrder;

            [Header("• T W E E N   S E T T I N G S")]
            public float embiggenTarget = 1;
            [Min(0F)] public float embiggenTweenDuration = 0.3F;
            public Ease embiggenTweenEase = Ease.InOutBack;
            private Tween embiggenTween;
            [Space(2F)]
            public float attachTweenScaleTarget = 1F;
            [Min(0F)] public float attachTweenMoveDuration = 0.25F, attachTweenScaleDuration = 0.5F;
            public Ease attachTweenMoveEase = Ease.InOutQuint, attachTweenScaleEase = Ease.OutBounce;

            public bool IsDraggable { get => draggable.enabled; set => draggable.enabled = value; }
            public bool IsAttached { get; private set; }

            public event Action OnSelect = delegate { }, OnCancel = delegate { }, OnAttach = delegate { };


            private void Reset()
            {
                  renderer = GetComponent<SpriteRenderer>();
                  collider = GetComponent<Collider2D>();
                  draggable = GetComponent<DraggableObject>();
            }
            private void Awake()
            {
                  transform = base.transform;

                  originalSortingOrder = renderer.sortingOrder;
                  key = keySprite != null ? keySprite.name : renderer.sprite.name;
            }
            private void OnEnable()
            {
                  IsAttached = false;
                  draggable.enabled = collider.enabled = IsDraggable = true;

                  draggable.OnPick.AddListener(HandleSelected);
                  draggable.OnRelease.AddListener(HandleReleased);
                  draggable.OnReturn.AddListener(HandleReturn);

                  embiggenTween = transform.DOScale(Vector2.one * embiggenTarget, embiggenTweenDuration).SetEase(embiggenTweenEase).SetAutoKill(false).OnKill(() => embiggenTween = null).Pause();
            }
            private void OnDisable()
            {
                  draggable.OnPick.RemoveListener(HandleSelected);
                  draggable.OnRelease.RemoveListener(HandleReleased);
                  draggable.OnReturn.RemoveListener(HandleReturn);

                  draggable.enabled = collider.enabled = IsDraggable = false;

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

                  if (!IsAttached) slot.Fade(1F);
                  matchingSlot = null;
            }

            private void HandleSelected()
            {
                  renderer.sortingOrder = 200;
                  embiggenTween.Restart();

                  OnSelect();
            }
            private void HandleReleased()
            {
                  if (matchingSlot != null)
                  {
                        AttachToSlot(matchingSlot);
                        return;
                  }
                  embiggenTween.PlayBackwards();

                  OnCancel();
            }
            private void HandleReturn()
            {
                  renderer.sortingOrder = originalSortingOrder;
            }

            private void AttachToSlot(Slot slot)
            {
                  IsAttached = true;
                  transform.SetParent(slot.transform, true);
                  OnAttach();

                  DOTween.Sequence()
                        .Append(transform.DOLocalMove(Vector2.zero, attachTweenMoveDuration).SetEase(attachTweenMoveEase).OnComplete(() => slot.Fade(0F)))
                        .AppendCallback(() =>
                        {
                              if (keySprite != null) renderer.sprite = keySprite;
                              renderer.sortingOrder = slot.Order + 1;
                        })
                        .Append(transform.DOScale(Vector2.one * attachTweenScaleTarget, attachTweenScaleDuration).SetEase(attachTweenScaleEase))
                        .SetLink(gameObject)
                        .Play();

                  enabled = false;
            }
      }
}