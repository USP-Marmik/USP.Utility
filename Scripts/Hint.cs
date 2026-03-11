using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace USP.Utility
{
	public class Hint : MonoBehaviour
	{
		[Header("• T W E E N   S E T T I N G S")]
		public float visibilityDuration = 0.2F;
		public Ease visibilityEase = Ease.OutBack;

		private Tween visibilityTween, delayedCall;

		public UnityEvent OnShow, OnHide;


		private void OnEnable()
		{
			visibilityTween = transform.DOScale(Vector2.one, visibilityDuration)
				.From(Vector2.zero)
				.SetEase(visibilityEase)
				.SetAutoKill(false)
				.OnKill(() => visibilityTween = null)
				.Pause();
		}
		private void OnDisable()
		{
			visibilityTween?.Kill();
		}

		public void Show()
		{
			delayedCall?.Kill();

			visibilityTween.PlayForward();
			OnShow.Invoke();
		}
		public void Show(float delay)
		{
			visibilityTween.Restart(true, delay);

			delayedCall?.Kill();
			delayedCall = DOVirtual.DelayedCall(delay, OnShow.Invoke).OnKill(() => delayedCall = null);
		}
		public void Hide()
		{
			delayedCall?.Kill();

			visibilityTween.SmoothRewind();
			OnHide.Invoke();
		}
	}
}