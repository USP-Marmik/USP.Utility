using UnityEngine;
using UnityEngine.UI;
using Emp37.Utility;
using DG.Tweening;

namespace USP.Utility
{
	[RequireComponent(typeof(Button))]
	public class AnimatedButton : MonoBehaviour
	{
		[SerializeField] private Button button;

		[Separator(Shade.Grey)]

		[StylizedTitle("Tween Settings", Shade.Grey)]
		[SerializeField, Range(-1F, 1F)] private float punchStrength = -0.1F;
		[SerializeField] private float punchElasticity = 1F;
		public float punchDuration = 0.5F;
		public int punchVibrato = 1;
		[SerializeField] private bool isIndependentUpdate;

		[SerializeField] private bool doPulse;
		[ShowIf(nameof(doPulse))] public float pulseDuration = 0.3F, pulseStrength = 0.1F, pulseInterval = 4F;
		[ShowIf(nameof(doPulse))] public Ease pulseEase = Ease.InOutSine;

		private Tween pulseTween;


		[Separator(Shade.Grey)]

		[StylizedTitle("Events", Shade.Grey)]
		public Button.ButtonClickedEvent OnClick;
		public Button.ButtonClickedEvent OnClickImmediate => button.onClick;

		private Tween previewTween;

		public bool Interactable
		{
			get => button.interactable;
			set => button.interactable = value;
		}


		private void Reset()
		{
			button = GetComponent<Button>();
			button.transition = Selectable.Transition.None;
		}
		private void OnEnable()
		{
			button.onClick.AddListener(PlayAnimation);

			if (doPulse)
			{
				pulseTween = DOTween.Sequence()
					.Join(transform.DOScale(Vector2.one * pulseStrength, pulseDuration).SetLoops(2, LoopType.Yoyo).SetEase(pulseEase).SetRelative())
					.AppendInterval(pulseInterval).SetLoops(-1, LoopType.Restart);
			}
		}
		private void OnDisable()
		{
			button.onClick.RemoveListener(PlayAnimation);

			if (pulseTween.IsActive())
			{
				pulseTween.Rewind();
				pulseTween.Kill();
			}
		}

		private void PlayAnimation()
		{
			transform.DOPunchScale(Vector3.one * punchStrength, punchDuration, punchVibrato, punchElasticity).OnComplete(OnAnimationComplete).SetUpdate(isIndependentUpdate);
			button.interactable = false;
		}
		private void OnAnimationComplete()
		{
			OnClick.Invoke();
			button.interactable = true;
		}

		[Button(Size.Large)]
		public void PreviewAnimation() // exposed in the inspector for testing purposes
		{
			if (previewTween?.IsActive() == true)
			{
				previewTween.Rewind();
				previewTween.Kill();
			}
			previewTween = transform.DOPunchScale(Vector3.one * punchStrength, punchDuration, punchVibrato, punchElasticity).SetUpdate(isIndependentUpdate).OnKill(() => previewTween = null);

			//#if UNITY_EDITOR
			//			if (UnityEditor.EditorApplication.isPlaying) return;

			//			DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(previewTween);
			//			DG.DOTweenEditor.DOTweenEditorPreview.Start();
			//#endif
		}
	}
}