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

            [Header("• B O U N D S")]
            public Collider2D Confiner;

            [Header("• E V E N T S")]
            public UnityEvent OnPick;
            public UnityEvent OnRelease;

            private Collider2D area;
            private Vector2 velocity;

            public bool IsDragging { get; private set; }


            private void Awake()
            {
                  area = GetComponent<Collider2D>();

                  OnPick.AddListener(() => IsDragging = true);
                  OnRelease.AddListener(OnDisable);
            }
            private void OnDisable() => IsDragging = false;

            public void DragTo(Vector2 position)
            {
                  Vector2 target = Confiner == null ? position : ClampTarget(position);
                  Vector3 p = transform.position;

                  if (!FreezeX && p.x != target.x) p.x = Mathf.SmoothDamp(p.x, target.x, ref velocity.x, SmoothTime);
                  if (!FreezeY && p.y != target.y) p.y = Mathf.SmoothDamp(p.y, target.y, ref velocity.y, SmoothTime);

                  transform.position = p;
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