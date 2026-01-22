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
            public float MaximumSpeed = 1000F;
            public bool FreezeX, FreezeY;
            public Vector2 PivotOffset;
            public Collider2D Confiner;
            public UnityEvent OnPick, OnRelease, OnReturn;

            [Header("• T W E E N   S E T T I N G S")]
            public bool autoReturnOnRelease;
            public float returnDuration = 0.5F;
            public Ease returnEase = Ease.OutQuad;

            private new Transform transform;
            private new Collider2D collider;

            private Tween returnTween;
            private Coroutine releaseCoroutine;

            private Vector2 dragVelocity;

            public Vector2 Origin { get; set; }
            public Vector2 Velocity => dragVelocity;
            public bool IsDragging { get; private set; }
            public bool IsReturning => returnTween != null && returnTween.IsActive();


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

            public void Pick()
            {
                  CancelReleaseCoroutine();
                  CancelReturn();

                  IsDragging = true;
                  OnPick.Invoke();
            }
            public void DragTo(Vector2 position)
            {
                  Vector2 target = (Confiner == null ? position + PivotOffset : ClampTarget(position + PivotOffset));
                  Vector3 p = transform.position;

                  if (!FreezeX && p.x != target.x)
                        p.x = Mathf.SmoothDamp(p.x, target.x, ref dragVelocity.x, SmoothTime, MaximumSpeed);

                  if (!FreezeY && p.y != target.y)
                        p.y = Mathf.SmoothDamp(p.y, target.y, ref dragVelocity.y, SmoothTime, MaximumSpeed);

                  transform.position = p;
            }
            public void Release()
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

                  Vector2 offset = (Vector2) (colliderBounds.center - base.transform.position);
                  Vector2 desiredCenter = target + offset;
                  Vector2 min = confinerBounds.min + colliderBounds.extents, max = confinerBounds.max - colliderBounds.extents;

                  if (min.x > max.x) min.x = max.x = confinerBounds.center.x;
                  if (min.y > max.y) min.y = max.y = confinerBounds.center.y;

                  Vector2 output = new(Mathf.Clamp(desiredCenter.x, min.x, max.x), Mathf.Clamp(desiredCenter.y, min.y, max.y));

                  return output - offset;
            }
      }
}