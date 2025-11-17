using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace USP
{
      [RequireComponent(typeof(Image))]
      public class Scale : Transition
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Image image;
            [SerializeField] private Sprite[] shapes;

            [Header("• C O N F I G U R A T I O N")]
            public Color OverlayShade = Color.white;
            public float Duration = 1F;
            public float Target = 8F;
            public Ease EaseIn = Ease.Linear, EaseOut = Ease.Linear;

            protected override Tween IntroTween => transform.DOScale(Target, Duration).SetEase(EaseIn);
            protected override Tween ExitTween => transform.DOScale(0F, Duration).SetEase(EaseOut);

            protected override void Initialize()
            {
                  image.color = OverlayShade;
                  image.sprite = shapes[Random.Range(0, shapes.Length)];
            }
      }
}