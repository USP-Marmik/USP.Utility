using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace USP.Utility
{
      [DisallowMultipleComponent, RequireComponent(typeof(Slider))]
      public class ProgressBar : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Canvas canvas;
            [SerializeField] private Slider slider;

            public Vector2 VisiblePosition, HiddenPosition;

            [Header("• T W E E N   S E T T I N G S")]
            public float showDuration = 0.5F;
            public Ease showEase = Ease.OutBack;

            public float hideDuration = 0.4F;
            public Ease hideEase = Ease.InBack;

            private RectTransform rect;
            private Tween showTween, hideTween;

            public bool IsVisible { get => canvas.enabled; set => canvas.enabled = value; }
            public float Progress { get => slider.normalizedValue; set => slider.normalizedValue = Mathf.Clamp01(value); }


            private void Reset()
            {
                  canvas = GetComponentInParent<Canvas>();
                  slider = GetComponent<Slider>();
            }
            private void Awake()
            {
                  rect = transform as RectTransform;
            }

            public void Show(bool reset = false)
            {
                  IsVisible = true;
                  if (reset) slider.value = 0F;

                  showTween ??= rect.DOAnchorPos(VisiblePosition, showDuration).SetEase(showEase).SetUpdate(true).SetAutoKill(false).SetRecyclable(false).Pause();
                  hideTween?.Pause(); showTween.Restart();
            }
            public void Hide()
            {
                  hideTween ??= rect.DOAnchorPos(HiddenPosition, hideDuration).SetEase(hideEase).OnComplete(() => IsVisible = false).SetUpdate(true).SetAutoKill(false).SetRecyclable(false).Pause();
                  showTween?.Pause(); hideTween.Restart();
            }
      }
}