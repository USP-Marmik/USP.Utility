using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace USP.Utility
{
      [RequireComponent(typeof(CanvasGroup), typeof(Image))]
      public class Fade : Transition
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Image image;
            [SerializeField] private CanvasGroup group;

            [Header("• C O N F I G U R A T I O N")]
            public Color OverlayShade = Color.white;
            public float Duration = 1F;
            [Range(0F, 1F)] public float Target = 1F;

            public Ease EaseIn = Ease.Linear, EaseOut = Ease.Linear;

            protected override Tween Intro => group.DOFade(Target, Duration).SetEase(EaseIn);
            protected override Tween Outro => group.DOFade(0F, Duration).SetEase(EaseOut).OnKill(() => group.alpha = 0F);

            protected override void Initialize()
            {
                  image.color = OverlayShade;
                  group.alpha = 0F;
            }
      }
}
