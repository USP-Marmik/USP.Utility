using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace USP.Utility
{
	public class AssemblyManager : MonoBehaviour
	{
		[SerializeField] private AudioSource soundSource, voiceSource;
		[SerializeField] private Piece[] pieces;
		[SerializeField] private Slot[] slots;

		[SerializeField] private AudioClip pickSFX, releaseSFX, attachSFX, introVoice, exitVoice;

		[SerializeField] private ParticleSystem attachedVFX;

		[Header("• H I N T")]
		[SerializeField] private Hint hint;
		[SerializeField] private float hintDelay = 4F;

		[Header("• E V E N T S")]
		public UnityEvent Assembled;

		private Action[] attachHandlers;
		private Coroutine playInfoRoutine;
		private Tween hintTween;
		private float idleTimer;
		private int attachedPieceCount;
		private bool isDraggingPiece, isHintPlaying;

		private IEnumerator WaitForPlayback
		{
			get
			{
				LockUnattachedPieces(true);
				yield return new WaitWhile(() => voiceSource.isPlaying);
				LockUnattachedPieces(false);

				TryCompleteAssembly();
				playInfoRoutine = null;
			}
		}

		/// <summary>
		/// AI CODE - IGNORE
		/// </summary>
		private (Piece, Slot) RandomUnattachedPieceSlotPair
		{
			get
			{
				int unattachedCount = 0;
				for (int i = 0; i < pieces.Length; i++)
				{
					if (!pieces[i].IsAttached) unattachedCount++;
				}
				if (unattachedCount == 0) return default;
				int targetUnattachedIndex = Random.Range(0, unattachedCount);
				for (int i = 0; i < pieces.Length; i++)
				{
					if (pieces[i].IsAttached) continue;
					if (targetUnattachedIndex == 0) return (pieces[i], slots[i]);
					targetUnattachedIndex--;
				}
				return default;
			}
		}
		public bool IsActive { set => gameObject.SetActive(value); }


		private IEnumerator Start()
		{
			voiceSource.PlayOneShot(introVoice);
			LockUnattachedPieces(true);
			yield return new WaitWhile(() => voiceSource.isPlaying);
			LockUnattachedPieces(false);
		}
		private void OnEnable()
		{
			attachedPieceCount = 0;
			attachHandlers = new Action[pieces.Length];

			for (int i = 0; i < pieces.Length; i++)
			{
				Piece piece = pieces[i];
				piece.Picked += HandlePicked;
				piece.Attached += attachHandlers[i] = handler;
				piece.Released += HandleReleased;

				void handler() => HandleAttached(piece);
			}
		}
		private void OnDisable()
		{
			isHintPlaying = isDraggingPiece = false;

			if (attachHandlers != null)
			{
				for (int i = 0; i < pieces.Length; i++)
				{
					Piece piece = pieces[i];
					piece.Picked -= HandlePicked;
					if (attachHandlers[i] != null) piece.Attached -= attachHandlers[i];
					piece.Released -= HandleReleased;
				}
			}
			if (playInfoRoutine != null)
			{
				StopCoroutine(playInfoRoutine);
				playInfoRoutine = null;
			}
		}
		private void Update()
		{
			if (isDraggingPiece || isHintPlaying) return;
			if (idleTimer < hintDelay) idleTimer += Time.deltaTime;
			else
			{
				idleTimer = 0F;
				ShowIdleHint();
			}
		}

		private void ShowIdleHint()
		{
			hint.Show();

			var pair = RandomUnattachedPieceSlotPair;
			Vector2 startPosition = pair.Item1.transform.position, endPosition = pair.Item2.transform.position;
			hintTween?.Kill();
			hintTween = hint.transform.DOMove(endPosition, 1F)
				.SetDelay(hint.visibilityDuration + 0.5F)
				.From(startPosition)
				.OnStart(() => isHintPlaying = true)
				.SetEase(Ease.OutSine)
				.OnComplete(() =>
				{
					hint.Hide();
					isHintPlaying = false;
				})
				.OnKill(() => hintTween = null);
		}
		private void HandlePicked()
		{
			isDraggingPiece = true;
			soundSource.PlayOneShot(pickSFX, 0.5F);

			idleTimer = 0F;
			hint.Hide();
		}
		private void HandleReleased()
		{
			isDraggingPiece = false;

			soundSource.PlayOneShot(releaseSFX);
		}
		private void HandleAttached(Piece piece)
		{
			isDraggingPiece = false;
			soundSource.PlayOneShot(attachSFX);

			attachedPieceCount++;
			if (playInfoRoutine != null)
			{
				StopCoroutine(playInfoRoutine);
				playInfoRoutine = null;
				voiceSource.Stop();
				LockUnattachedPieces(false);
			}
			if (piece.InfoClip == null)
			{
				TryCompleteAssembly();
				return;
			}
			voiceSource.PlayOneShot(piece.InfoClip);
			playInfoRoutine = StartCoroutine(WaitForPlayback);

			if (attachedVFX != null) attachedVFX.Play();
		}
		private void TryCompleteAssembly()
		{
			if (attachedPieceCount == pieces.Length)
			{
				enabled = false;
				Assembled.Invoke();

				voiceSource.PlayOneShot(exitVoice);
			}
		}
		private void LockUnattachedPieces(bool value)
		{
			for (int i = 0; i < pieces.Length; i++)
			{
				Piece piece = pieces[i];
				if (piece.IsAttached) continue;

				piece.IsLocked = value;
			}
		}
	}
}