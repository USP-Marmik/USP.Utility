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
            public float Scale = 1F;

            public Ease EaseIn = Ease.Linear, EaseOut = Ease.Linear;


            protected override Tween IntroTween
            {
                  get
                  {
                        Vector2 backgroundSize = background.sprite.rect.size;
                        Vector2 shapeSize = shape.sprite.rect.size;
                        Vector2 scale = new(backgroundSize.x / shapeSize.x, backgroundSize.y / shapeSize.y);
                        return shape.rectTransform.DOSizeDelta(Scale * Mathf.Max(scale.x, scale.y) * shapeSize, Duration).SetEase(EaseIn);
                  }
            }
            protected override Tween ExitTween => shape.rectTransform.DOSizeDelta(Vector2.zero, Duration).SetEase(EaseOut)
                  .OnKill(() => shape.rectTransform.sizeDelta = Vector2.zero).OnComplete(() => gameObject.SetActive(false));

            protected override void Initialize()
            {
                  gameObject.SetActive(true);

                  MaskSet selected = sets[Random.Range(0, sets.Length)];
                  shape.sprite = selected.Shape;
                  background.sprite = selected.Background;

                  background.rectTransform.sizeDelta = new(Screen.width, Screen.height);
            }
      }
}
