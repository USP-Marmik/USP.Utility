using UnityEngine;

using DG.Tweening;

namespace USP.Utility
{
      public class Slot : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new Collider2D collider;
            [SerializeField] private SpriteRenderer mask, hint;

            [Header("• T W E E N   S E T T I N G S")]
            public float fadeDuration = 0.3F;

            private Tweener maskTween;

            public string Key => hint.sprite.name;
            public int Order => mask != null ? mask.sortingOrder : 0;


            private void Reset()
            {
                  collider = GetComponent<Collider2D>();
                  var renderers = GetComponentsInChildren<SpriteRenderer>();
                  mask = renderers[0]; hint = renderers[1];
            }
            private void OnEnable() => collider.enabled = hint.enabled = true;
            private void OnDisable()
            {
                  collider.enabled = hint.enabled = false;

                  Fade(0F);
            }

            public void Fade(float alpha)
            {
                  maskTween ??= mask.DOColor(default, fadeDuration).SetAutoKill(false);
                  Color c = mask.color;
                  c.a = Mathf.Clamp01(alpha);
                  maskTween.ChangeEndValue(c, true).Restart();
            }
      }
}