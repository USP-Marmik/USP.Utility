using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer))]
      public class Icon : MonoBehaviour
      {
            [SerializeField] private Piece piece;

            [Header("• C O N F I G U R A T I O N   -   I C O N")]
            public float collapseDuration = 0.2F;
            public Ease collapseEase = Ease.InSine;
            public float restoreDuration = 0.2F;
            public Ease restoreEase = Ease.OutBack;

            [Header("• C O N F I G U R A T I O N   -   P I E C E")]
            public float pieceScaleDuration = 0.37F;
            public Ease pieceScaleEase = Ease.OutBack;
            public float pickedScale = 1, placedScale = 1;

            private Sequence pickSequence, returnSequence;

            private Transform pieceTransform;
            private Vector2 originalIconScale, originalPieceScale;


            private void Reset()
            {
                  piece = GetComponentInChildren<Piece>();
            }
            private void Awake()
            {
                  originalIconScale = transform.localScale;

                  pieceTransform = piece.transform;
                  originalPieceScale = pieceTransform.localScale;
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
                        .Append(transform.DOScale(Vector2.zero, collapseDuration).SetEase(collapseEase))
                        .Join(pieceTransform.DOScale(Vector2.one * pickedScale, pieceScaleDuration).SetEase(pieceScaleEase))
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
                              .Append(transform.DOScale(originalIconScale, restoreDuration).SetEase(restoreEase))
                              .AppendCallback(() => pieceTransform.SetParent(transform, true))
                              .Append(pieceTransform.DOLocalMove(Vector2.zero, piece.attachDuration).SetEase(piece.attachEase))
                              .Join(pieceTransform.DOScale(originalPieceScale, pieceScaleDuration).SetEase(pieceScaleEase))
                              .OnKill(() => returnSequence = null)
                              .Play();
                  }
                  else
                  {
                        gameObject.SetActive(false);
                        pieceTransform.DOScale(Vector2.one * placedScale, pieceScaleDuration).SetEase(pieceScaleEase).SetLink(gameObject);
                  }
            }
      }
}