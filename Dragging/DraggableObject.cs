using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      [RequireComponent(typeof(Collider2D))]
      public sealed class DraggableObject : MonoBehaviour
      {
            [Header("• M O T I O N")]
            [Min(0f)] public float SmoothTime = 0.1F;
            public bool FreezeX, FreezeY;
            public Vector2 DragOffset;
            private Vector2 velocity;

            [Header("• B O U N D S")]
            public Collider2D Confiner;
            private Collider2D area;

            [Header("• E V E N T S")]
            public UnityEvent OnPick;
            public UnityEvent OnRelease;
            public UnityEvent OnReturn;

            [Header("• T W E E N   S E T T I N G S")]
            public bool ReturnOnRelease;
            public float ReturnDuration = 0.5F;
            public Ease ReturnEase = Ease.OutQuad;
            private Vector2 initialPosition;
            private Tween returnTween;

            public bool IsDragging { get; private set; }


            private void Awake()
            {
                  area = GetComponent<Collider2D>();

                  initialPosition = transform.position;
            }
            private void OnEnable()
            {
                  OnPick.AddListener(HandlePick);
                  OnRelease.AddListener(HandleRelease);
            }
            private void OnDisable()
            {
                  OnPick.RemoveListener(HandlePick);
                  OnRelease.RemoveListener(HandleRelease);

                  IsDragging = false;
            }

            public void Return()
            {
                  returnTween?.Kill(false);
                  returnTween = transform.DOMove(initialPosition, ReturnDuration).SetEase(ReturnEase).OnComplete(() => { returnTween = null; OnReturn.Invoke(); });
            }
            public void DragTo(Vector2 position)
            {
                  Vector2 target = (Confiner == null ? position : ClampTarget(position)) + DragOffset;
                  Vector3 p = transform.position;

                  if (!FreezeX && p.x != target.x) p.x = Mathf.SmoothDamp(p.x, target.x, ref velocity.x, SmoothTime);
                  if (!FreezeY && p.y != target.y) p.y = Mathf.SmoothDamp(p.y, target.y, ref velocity.y, SmoothTime);

                  transform.position = p;
            }

            private void HandlePick()
            {
                  returnTween?.Kill(false); returnTween = null;

                  IsDragging = true;
            }
            private void HandleRelease()
            {
                  IsDragging = false;
                  if (ReturnOnRelease) Return();
            }
            private Vector2 ClampTarget(Vector2 target)
            {
                  Bounds confinerBounds = Confiner.bounds, colliderBounds = area.bounds;

                  Vector2 offset = (Vector2) (colliderBounds.center - transform.position);
                  Vector2 desiredCenter = target + offset;
                  Vector2 min = confinerBounds.min + colliderBounds.extents, max = confinerBounds.max - colliderBounds.extents;

                  if (min.x > max.x) min.x = max.x = confinerBounds.center.x;
                  if (min.y > max.y) min.y = max.y = confinerBounds.center.y;

                  Vector2 output = new(Mathf.Clamp(desiredCenter.x, min.x, max.x), Mathf.Clamp(desiredCenter.y, min.y, max.y));

                  return output - offset;
            }
      }
}