using UnityEngine;

using DG.Tweening;

namespace USP.Utility
{
      public class Slot : MonoBehaviour
      {
            [SerializeField] private SpriteRenderer mask, hint;
            private Piece current;
            private Tween maskTween;


            private void OnTriggerEnter2D(Collider2D collision)
            {
                  if (!collision.TryGetComponent(out Piece piece) || hint.sprite == null || hint.sprite.name != piece.Key) return;

                  current = piece;
                  current.OnRelease.AddListener(HandleRelease);

                  FadeMask(0.5F);
            }
            private void OnTriggerExit2D(Collider2D collision)
            {
                  if (current == null || !collision.TryGetComponent(out Piece piece) || piece != current) return;

                  FadeMask(current.IsLocked ? 0F : 1F);

                  current.OnRelease.RemoveListener(HandleRelease);
                  current = null;
            }

            private void HandleRelease() => current.Attach(transform, mask.sortingOrder + 1);
            private void FadeMask(float alpha)
            {
                  maskTween?.Kill();
                  Color c = mask.color;
                  c.a = alpha;
                  maskTween = mask.DOColor(c, 0.2F).OnComplete(() => maskTween = null);
            }
      }
}