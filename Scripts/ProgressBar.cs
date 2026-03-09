using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace USP.Utility
{
      [DisallowMultipleComponent, RequireComponent(typeof(Slider))]
      public class ProgressBar : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Slider slider;

            private RectTransform rect;

            [Header("• T W E E N   S E T T I N G S")]
            public float visibilityDuration = 0.5F;
            public Ease visibilityEase = Ease.InOutBack;

            private Tween visibilityTween;

            public float Progress 
            { get => slider.normalizedValue; set => slider.normalizedValue = Mathf.Clamp01(value); }


            private void Reset()
            {
                  slider = GetComponentInChildren<Slider>();
            }
            private void Awake()
            {
                  rect = slider.transform as RectTransform;
            }
            private void Start()
            {
                  visibilityTween = rect.DOAnchorPos(Vector2.up * rect.sizeDelta.y, visibilityDuration)
                        .SetEase(visibilityEase)
                        .SetUpdate(true).SetAutoKill(false)
                        .Pause();
            }

            public void Show(bool reset = false)
            {
                  if (reset) slider.value = 0F;
                  visibilityTween.SmoothRewind();
            }
            public void Hide()
            {
                  visibilityTween.Restart();
            }
      }
}