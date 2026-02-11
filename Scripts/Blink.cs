using System.Linq;

using UnityEngine;

using DG.Tweening;

namespace USP.Utility
{
      public class Blink : MonoBehaviour
      {
            [SerializeField] private Transform[] blinkableTransforms;

            [Header("• C O N F I G U R A T I O N")]
            public float blinkTargetScale;
            [SerializeField] private Vector2 blinkIntervalSeconds = new(2F, 5F);
            private float elapsedSinceBlink, nextBlinkDelay;

            [Header("• T W E E N   S E T T I N G S")]
            public float blinkDurationSeconds = 0.25F;
            public Ease blinkEaseType = Ease.Linear;

            private Tween[] blinkTweens;


            private void Reset()
            {
                  blinkableTransforms = GetComponentsInChildren<Transform>().Skip(1).ToArray();
            }
            private void OnEnable()
            {
                  blinkTweens = new Tween[blinkableTransforms.Length];
                  for (int i = 0; i < blinkableTransforms.Length; i++)
                  {
                        var eye = blinkableTransforms[i];
                        blinkTweens[i] = eye.DOScaleY(blinkTargetScale, blinkDurationSeconds).SetEase(blinkEaseType).SetLoops(2, LoopType.Yoyo).SetAutoKill(false).Pause();
                  }

                  ScheduleNextBlink(blinkIntervalSeconds);
            }
            private void OnDisable()
            {
                  foreach (var tween in blinkTweens)
                  {
                        tween.Kill();
                  }
            }
            private void LateUpdate()
            {
                  if (elapsedSinceBlink < nextBlinkDelay)
                  {
                        elapsedSinceBlink += Time.deltaTime;
                        return;
                  }
                  elapsedSinceBlink = 0F;
                  ScheduleNextBlink(blinkIntervalSeconds);
                  PlayBlink();
            }

            private void PlayBlink()
            {
                  foreach (Tween tween in blinkTweens)
                  {
                        tween.Restart();
                  }
            }
            private void ScheduleNextBlink(Vector2 range) => nextBlinkDelay = Random.Range(range.x, range.y);
      }
}