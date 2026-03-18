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

		[Header("• E V E N T S")]
		public UnityEvent OnShow;
		public UnityEvent OnHide;

		private Tween visibilityTween, delayedCall;

		public bool IsVisible { get; private set; }


		private void OnEnable()
		{
			visibilityTween = transform.DOScale(Vector2.one, visibilityDuration)
				.From(Vector2.zero)
				.SetEase(visibilityEase)
				.SetAutoKill(false)
				.OnKill(() =>
				{
					IsVisible = false;
					visibilityTween = null;
				})
				.Pause();
		}
		private void OnDisable()
		{
			visibilityTween?.Kill();
		}

		public void Show()
		{
			delayedCall?.Kill();

			IsVisible = true;
			visibilityTween.PlayForward();
			OnShow.Invoke();
		}
		public void Show(float delay)
		{
			visibilityTween.Restart(true, delay);

			delayedCall?.Kill();
			delayedCall = DOVirtual.DelayedCall(delay, OnShow.Invoke).OnComplete(() => IsVisible = true).OnKill(() => delayedCall = null);
		}
		public void Hide()
		{
			delayedCall?.Kill();

			IsVisible = false;
			visibilityTween.SmoothRewind();
			OnHide.Invoke();
		}
	}
}