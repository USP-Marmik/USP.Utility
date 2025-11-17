using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace USP
{
      public class Mask : Transition
      {
            [System.Serializable]
            private struct MaskSet
            {
                  public Sprite Shape, Background;
            }

            [Header("• R E F E R E N C E S")]
            [SerializeField] private Image shape;
            [SerializeField] private Image background;
            [SerializeField] private MaskSet[] sets;

            [Header("• C O N F I G U R A T I O N")]
            public float Duration = 1F;

            public Ease EaseIn = Ease.Linear, EaseOut = Ease.Linear;

            protected override Tween IntroTween
            {
                  get
                  {
                        Vector2 size = background.rectTransform.sizeDelta;
                        return shape.rectTransform.DOSizeDelta(2F * Mathf.Max(size.x, size.y) * Vector2.one, Duration).SetEase(EaseIn);
                  }
            }
            protected override Tween ExitTween => shape.rectTransform.DOSizeDelta(Vector2.zero, Duration).SetEase(EaseOut);

            protected override void Initialize()
            {
                  gameObject.SetActive(true);

                  MaskSet selected = sets[Random.Range(0, sets.Length)];

                  shape.sprite = selected.Shape;
                  background.sprite = selected.Background;

                  background.rectTransform.sizeDelta = new(Screen.width, Screen.height);
            }
            protected override void OnFinish()
            {
                  gameObject.SetActive(false);
            }
      }
}
