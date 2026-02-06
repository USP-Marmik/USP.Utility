using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer))]
      public class Icon : MonoBehaviour
      {
            [SerializeField] private Piece piece;
            [SerializeField] private float pickScale = 1, placementScale = 1;

            private Vector2 originalIconScale, originalPieceScale;
            private Transform pieceTransform;
            private Sequence pickSequence, returnSequence;


            private void Reset()
            {
                  piece = GetComponentInChildren<Piece>();
            }
            private void Awake()
            {
                  originalIconScale = transform.localScale;
                  originalPieceScale = piece.transform.localScale;
            }
            private void OnEnable()
            {
                  piece.enabled = true;
                  piece.OnPick.AddListener(HandlePick);
                  piece.OnRelease.AddListener(HandleRelease);
            }
            private void OnDisable()
            {
                  piece.OnPick.RemoveListener(HandlePick);
                  piece.OnRelease.RemoveListener(HandleRelease);
                  piece.enabled = false;

                  pickSequence?.Kill(); returnSequence?.Kill();
            }

            private void HandlePick()
            {
                  piece.transform.SetParent(null);
                  pickSequence ??= DOTween.Sequence()
                        .Append(transform.DOScale(Vector2.zero, 0.2F).SetEase(Ease.InSine))
                        .Join(pieceTransform.DOScale(Vector2.one * pickScale, 0.37F).SetEase(Ease.OutBack))
                        .OnKill(() => pickSequence = null)
                        .SetAutoKill(false)
                        .Pause();
                  pickSequence.Restart();
            }
            private void HandleRelease() => StartCoroutine(ReleaseRoutine());
            private IEnumerator ReleaseRoutine()
            {
                  yield return new WaitForEndOfFrame();

                  if (piece.enabled)
                  {
                        if (pickSequence != null && pickSequence.IsPlaying()) pickSequence.Pause();
                        returnSequence?.Kill();

                        returnSequence = DOTween.Sequence()
                              .Append(transform.DOScale(originalIconScale, 0.2F).SetEase(Ease.OutBack))
                              .AppendCallback(() => pieceTransform.SetParent(transform, true))
                              .Append(pieceTransform.DOLocalMove(Vector2.zero, piece.attachDuration).SetEase(piece.attachEase))
                              .Join(pieceTransform.DOScale(originalPieceScale, 0.2F).SetEase(Ease.InBack))
                              .OnKill(() => returnSequence = null)
                              .Play();
                  }
                  else
                  {
                        gameObject.SetActive(false);

                        DOTween.Sequence()
                              .Append(pieceTransform.DOLocalMove(Vector2.zero, piece.attachDuration))
                              .Append(pieceTransform.DOScale(Vector2.one * placementScale, piece.attachDuration))
                              .SetEase(piece.attachEase).SetLink(piece.gameObject)
                              .Play();
                  }
            }
      }
}