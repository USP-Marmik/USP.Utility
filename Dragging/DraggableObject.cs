using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

namespace USP.Utility
{
      [RequireComponent(typeof(Collider2D))]
      public sealed class DraggableObject : MonoBehaviour
      {
            [Header("• M O T I O N")]
            [Min(0F)] public float SmoothTime = 0.1F;
            public float MaximumSpeed = 1000F;
            public bool FreezeX, FreezeY;
            public Vector2 PivotOffset;

            [Header("• B O U N D S")]
            public Collider2D Confiner;

            [Header("• E V E N T S")]
            public UnityEvent OnPick;
            public UnityEvent OnRelease;
            public UnityEvent OnReturn;

            [Header("• T W E E N   S E T T I N G S")]
            public bool AutoReturnOnRelease;
            public float ReturnDuration = 0.5F;
            public Ease ReturnEase = Ease.OutQuad;
            private Tween returnTween;

            private Vector2 _velocity;
            private new Collider2D collider;

            public bool IsDragging { get; private set; }
            public Vector2 Origin { get; set; }
            public Vector2 Velocity => _velocity;
            public bool IsReturning => returnTween != null;


            private void Awake()
            {
                  collider = GetComponent<Collider2D>();

                  Origin = transform.position;
            }
            private void OnDisable()
            {
                  IsDragging = false;
            }

            public void Pick()
            {
                  CancelReturn();
                  IsDragging = true;
                  OnPick.Invoke();
            }
            public void DragTo(Vector2 position)
            {
                  Vector2 target = (Confiner == null ? position : ClampTarget(position)) + PivotOffset;
                  Vector3 p = transform.position;

                  if (!FreezeX && p.x != target.x)
                        p.x = Mathf.SmoothDamp(p.x, target.x, ref _velocity.x, SmoothTime, MaximumSpeed);

                  if (!FreezeY && p.y != target.y)
                        p.y = Mathf.SmoothDamp(p.y, target.y, ref _velocity.y, SmoothTime, MaximumSpeed);

                  transform.position = p;
            }
            public void Release()
            {
                  IsDragging = false;
                  if (AutoReturnOnRelease) Return();
                  OnRelease.Invoke();
            }
            public void Return()
            {
                  CancelReturn();
                  returnTween = transform.DOMove(Origin, ReturnDuration)
                        .SetEase(ReturnEase)
                        .OnComplete(() =>
                        {
                              returnTween = null;
                              OnReturn.Invoke();
                        });
            }
            public void CancelReturn()
            {
                  returnTween?.Kill(false);
                  returnTween = null;
            }

            private Vector2 ClampTarget(Vector2 target)
            {
                  Bounds confinerBounds = Confiner.bounds, colliderBounds = collider.bounds;

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