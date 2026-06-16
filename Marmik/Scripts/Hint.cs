using DG.Tweening;
using Emp37.Utility;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace USP.Utility
{
	public class Hint : MonoBehaviour
	{
		private enum TransitionType { Scale, Fade }

		[Header("- A N I M A T I O N   S E T T I N G S")]
		[SerializeField] private TransitionType transitionType;
		[Space(2F)]
		public float visibilityDuration = 0.25F;
		public Ease visibilityEase = Ease.Linear;
		public float moveDuration = 1.5F;
		public Ease moveEase = Ease.Linear;

		private Tween visibilityTween, delayedCall;
		private Sequence moveSequence;
		private Vector3 originalScale;

		[field: SerializeField] public bool IsVisible { get; private set; }


		private void Awake()
		{
			originalScale = transform.localScale;
		}
		private void OnEnable()
		{
			switch (transitionType)
			{
				case TransitionType.Scale:
					visibilityTween = transform.DOScale(originalScale, visibilityDuration).From(Vector2.zero);
					break;

				case TransitionType.Fade:
					if (TryGetComponent(out SpriteRenderer renderer))
					{
						visibilityTween = renderer.DOFade(1F, visibilityDuration).From(0F);
					}
					else
					{
						Debug.LogWarning($"No {typeof(SpriteRenderer).Name} component found on {name}. Disabling the hint.");
						enabled = false;
						return;
					}
					break;
			}
			visibilityTween
				.SetEase(visibilityEase)
				.SetAutoKill(false)
				.OnKill(() =>
				{
					visibilityTween = null;
					IsVisible = false;
				})
				.Pause();
		}
		private void OnDisable()
		{
			visibilityTween?.Kill();
		}

		public void Show()
		{
			delayedCall?.Kill(false);

			IsVisible = true;
			visibilityTween.PlayForward();
		}
		public void Show(float delay)
		{
			delayedCall?.Kill(false);
			delayedCall = DOVirtual.DelayedCall(delay, Show).OnKill(() => delayedCall = null);
		}
		public void Hide()
		{
			moveSequence?.Kill(false);
			delayedCall?.Kill(false);

			visibilityTween.PlayBackwards();
			IsVisible = false;
		}

		public void MoveTo(Vector2 target, float holdDuration = 0.4F)
		{
			moveSequence = DOTween.Sequence();
			if (!IsVisible) moveSequence.AppendCallback(Show).AppendInterval(visibilityDuration);
			moveSequence.Append(transform.DOMove(target, moveDuration).SetEase(moveEase)).AppendInterval(holdDuration).AppendCallback(Hide);
			moveSequence.OnKill(() => moveSequence = null);
		}
		public void MoveTo(Vector2 target, Vector2 from, float holdDuration = 0.4F)
		{
			transform.position = from;
			MoveTo(target, holdDuration);
		}
	}
}
