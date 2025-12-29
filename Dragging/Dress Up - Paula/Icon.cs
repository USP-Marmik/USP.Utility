using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer))]
      public class Icon : MonoBehaviour
      {
            [SerializeField] private Piece piece;

            private Vector2 iconScale, pieceScale;
            [SerializeField] private float overrideScale = 1F;


            private void Reset()
            {
                  piece = GetComponentInChildren<Piece>();
            }
            private void Awake()
            {
                  iconScale = transform.localScale;
                  pieceScale = piece.transform.localScale;
            }
            private void OnEnable()
            {
                  piece.enabled = true;

                  piece.OnPick.AddListener(OnPick);
                  piece.OnRelease.AddListener(OnRelease);
            }
            private void OnDisable()
            {
                  piece.OnPick.RemoveListener(OnPick);
                  piece.OnRelease.RemoveListener(OnRelease);

                  piece.enabled = false;
            }

            private void OnPick()
            {
                  piece.transform.SetParent(null);

                  transform.DOScale(Vector2.zero, 0.2F).SetEase(Ease.InSine);
                  piece.transform.DOScale(overrideScale * Vector2.one, 0.25F).SetEase(Ease.InSine);
            }
            private void OnRelease() => StartCoroutine(HandleRelease());
            private IEnumerator HandleRelease()
            {
                  yield return new WaitForEndOfFrame();

                  bool locked = piece.IsLocked;
                  if (locked)
                  {
                        gameObject.SetActive(false);
                        piece.transform.DOScale(Vector2.one, 0.3F).SetEase(Ease.OutSine);
                  }
                  else
                        transform
                              .DOScale(iconScale, 0.2F)
                              .SetEase(Ease.OutBack)
                              .OnComplete(() => DOTween.Sequence()
                              .AppendCallback(() => piece.transform.SetParent(transform, true))
                              .Append(piece.transform.DOLocalMove(Vector2.zero, 0.2F).SetEase(Ease.InOutSine))
                              .Join(piece.transform.DOScale(pieceScale, 0.3F).SetEase(Ease.OutSine))
                              .Play());
            }
      }
}