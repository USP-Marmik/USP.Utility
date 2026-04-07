using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

namespace USP.Utility
{
	[RequireComponent(typeof(Collider2D))]
	public sealed class DraggableObject : MonoBehaviour
	{
		private new Transform transform;
		private new Collider2D collider;

		private Tween returnTween;
		private Coroutine releaseCoroutine;

		private Vector2 dragVelocity;

		[Header("D R A G")]
		public Collider2D Confiner;
		public Vector2 Offset;
		[Min(0F)] public float SmoothTime = 0.1F;
		[Min(0F)] public float MaximumSpeed = 1000F;
		public bool FreezeX;
		public bool FreezeY;

		[Header("T W E E N   S E T T I N G S")]
		public bool autoReturnOnRelease;
		public Ease returnEase = Ease.OutQuad;
		public float returnDuration = 0.5F;

		[Header("E V E N T S")]
		public UnityEvent OnPick;
		public UnityEvent OnRelease;
		public UnityEvent OnReturn;

		public Vector2 Origin { get; set; }
		public bool IsDragging { get; private set; }

		public Vector2 Velocity => dragVelocity;
		public bool IsReturning => returnTween != null && returnTween.IsActive() && returnTween.IsPlaying();


		private void Awake()
		{
			transform = base.transform;
			collider = GetComponent<Collider2D>();

			Origin = transform.localPosition;
		}
		private void OnDisable()
		{
			IsDragging = false;
			dragVelocity = Vector2.zero;

			StopReleaseCoroutine();
		}

		internal void Pick()
		{
			StopReleaseCoroutine();
			CancelReturn();

			IsDragging = true;
			OnPick.Invoke();
		}
		internal void DragTo(Vector2 position)
		{
			Vector2 target = position + Offset;
			Vector3 p = transform.position;

			if (Confiner != null) target = ClampTarget(target);

			if (!FreezeX && p.x != target.x)
				p.x = Mathf.SmoothDamp(p.x, target.x, ref dragVelocity.x, SmoothTime, MaximumSpeed);
			if (!FreezeY && p.y != target.y)
				p.y = Mathf.SmoothDamp(p.y, target.y, ref dragVelocity.y, SmoothTime, MaximumSpeed);

			transform.position = p;
		}
		internal void Release()
		{
			IsDragging = false;
			dragVelocity = Vector2.zero;
			if (autoReturnOnRelease)
			{
				StopReleaseCoroutine();
				releaseCoroutine = StartCoroutine(ReturnOnNextPhysicsUpdate());
			}
			OnRelease.Invoke();
		}

		public void Return()
		{
			returnTween?.Kill(false);
			returnTween = transform.DOLocalMove(Origin, returnDuration)
				.SetEase(returnEase)
				.OnComplete(OnReturn.Invoke)
				.OnKill(() => returnTween = null);
		}
		public void CancelReturn() => returnTween?.Kill(false);

		private void StopReleaseCoroutine()
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
		private Vector2 ClampTarget(Vector2 target)
		{
			if (Confiner == null) return target;

			Bounds confinerBounds = Confiner.bounds, draggableBounds = collider.bounds;

			Vector2 min = confinerBounds.min + draggableBounds.extents, max = confinerBounds.max - draggableBounds.extents;

			if (min.x > max.x) min.x = max.x = confinerBounds.center.x;
			if (min.y > max.y) min.y = max.y = confinerBounds.center.y;

			Vector2 offset = draggableBounds.center - transform.position;
			Vector2 center = target + offset;
			Vector2 output = new(Mathf.Clamp(center.x, min.x, max.x), Mathf.Clamp(center.y, min.y, max.y));

			return output - offset;
		}
	}
}