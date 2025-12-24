using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      [RequireComponent(typeof(Collider2D))]
      public class Draggable : MonoBehaviour
      {
            [Header("• M O T I O N")]
            [Min(0f)] public float SmoothTime = 0.1F;
            public bool FreezeX, FreezeY;

            [Header("• B O U N D S")]
            public Collider2D Confiner;

            [Header("• E V E N T S")]
            public UnityEvent OnPick;
            public UnityEvent OnRelease;


            private new Collider2D collider;
            private Vector2 velocity;
            private bool isDragging; public bool IsDragging
            {
                  get => isDragging;
                  internal set
                  {
                        if (value == isDragging) return;

                        isDragging = value;
                        (value ? OnPick : OnRelease).Invoke();
                  }
            }
            internal Vector2 Position
            {
                  get => transform.position;
                  set
                  {
                        if (!enabled) return;

                        Vector2 target = Confiner == null ? value : ClampTarget(value);
                        Vector3 p = transform.position;

                        if (!FreezeX) p.x = Mathf.SmoothDamp(p.x, target.x, ref velocity.x, SmoothTime);
                        if (!FreezeY) p.y = Mathf.SmoothDamp(p.y, target.y, ref velocity.y, SmoothTime);

                        transform.position = p;
                  }
            }

            private void Awake()
            {
                  collider = GetComponent<Collider2D>();
            }
            private void OnDisable() => isDragging = false;

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