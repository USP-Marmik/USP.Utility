using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

namespace USP.Utility
{
      [RequireComponent(typeof(Collider2D))]
      public sealed class DraggableObject : MonoBehaviour
      {
            [Header("• C O N F I G U R A T I O N")]
            [Min(0F)] public float SmoothTime = 0.1F;
            [Min(0F)] public float MaximumSpeed = 1000F;
            public bool FreezeX, FreezeY;
            public Vector2 Offset;
            public Collider2D Confiner;

            [Header("• T W E E N   S E T T I N G S")]
            public bool autoReturnOnRelease;
            public float returnDuration = 0.5F;
            public Ease returnEase = Ease.OutQuad;

            [Header("• E V E N T S")]
            public UnityEvent OnPick;
            public UnityEvent OnRelease;
            public UnityEvent OnReturn;

            private new Transform transform;
            private new Collider2D collider;

            private Tween returnTween;
            private Coroutine releaseCoroutine;

            private Vector2 velocity;

            public Vector2 Origin { get; set; }
            public Vector2 Velocity => velocity;
            public bool IsDragging { get; private set; }


            private void Awake()
            {
                  collider = GetComponent<Collider2D>();

                  transform = base.transform;
                  Origin = transform.localPosition;
            }
            private void OnDisable()
            {
                  IsDragging = false;
                  CancelReleaseCoroutine();
            }

            internal void Pick()
            {
                  CancelReleaseCoroutine();
                  CancelReturn();

                  IsDragging = true;
                  OnPick.Invoke();
            }
            internal void DragTo(Vector2 position)
            {
                  Vector2 target = (Confiner == null ? position + Offset : ClampTarget(position + Offset));
                  Vector3 pos = transform.position;

                  if (!FreezeX && pos.x != target.x)
                        pos.x = Mathf.SmoothDamp(pos.x, target.x, ref velocity.x, SmoothTime, MaximumSpeed);

                  if (!FreezeY && pos.y != target.y)
                        pos.y = Mathf.SmoothDamp(pos.y, target.y, ref velocity.y, SmoothTime, MaximumSpeed);

                  transform.position = pos;
            }
            internal void Release()
            {
                  IsDragging = false;

                  if (autoReturnOnRelease)
                  {
                        CancelReleaseCoroutine();
                        releaseCoroutine = StartCoroutine(ReturnOnNextPhysicsUpdate());
                  }
                  OnRelease.Invoke();
            }

            public void Return()
            {
                  returnTween?.Kill(false);
                  returnTween = transform.DOLocalMove(Origin, returnDuration)
                        .SetEase(returnEase)
                        .OnComplete(HandleReturnComplete);
            }
            public void CancelReturn()
            {
                  returnTween?.Kill(false);
                  returnTween = null;
            }

            private void CancelReleaseCoroutine()
            {
                  if (releaseCoroutine == null) return;

                  StopCoroutine(releaseCoroutine);
                  releaseCoroutine = null;
            }
            private IEnumerator ReturnOnNextPhysicsUpdate()
            {
                  yield return new WaitForFixedUpdate();
                  releaseCoroutine = null;
                  Return();
            }
            private void HandleReturnComplete()
            {
                  returnTween = null;
                  OnReturn.Invoke();
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