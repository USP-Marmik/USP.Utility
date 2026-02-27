using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace USP.Utility
{
      public class PunchEffect : MonoBehaviour, IPointerDownHandler
      {
            public float Offset = 0.1F, Duration = 0.25F, Elasticity = 0.8F;
            public int Vibration = 1;

            private Tween pressTween;


            public void OnPointerDown(PointerEventData _)
            {
                  pressTween ??= transform
                        .DOPunchScale(Vector3.one * Offset, Duration, Vibration, Elasticity)
                        .SetAutoKill(false)
                        .Pause();

                  pressTween.Restart();
            }
      }
}