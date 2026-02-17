using System;
using System.Collections;
using UnityEngine;
using USP.Utility;

namespace USP.Minigame.Paula
{
      [RequireComponent(typeof(AudioSource))]
      public class PiecesHandler : MonoBehaviour
      {
            [Header("A U D I O")]
            [SerializeField] private AudioSource voiceSource;
            [SerializeField] private AudioClip[] voiceOverClips;
            private Action[] playVoiceOverHandlers;

            [Header("G A M E P L A Y")]
            [SerializeField] private Piece[] pieces;
            [SerializeField] private Hint hint;
            [SerializeField] private float hintInterval = 4F;

            private float idleTime;
            private Coroutine voiceOverRoutine;

            private Vector3 GetRandomUnattachedPiecePosition
            {
                  get
                  {
                        var available = Array.FindAll(pieces, p => !p.IsAttached);
                        if (available.Length == 0) return Vector3.zero;
                        var selected = available[UnityEngine.Random.Range(0, available.Length)];
                        return selected.transform.position;
                  }
            }


            private void Reset()
            {
                  voiceSource = GetComponent<AudioSource>();
            }
            private void OnEnable()
            {
                  playVoiceOverHandlers = new Action[pieces.Length];
                  for (int i = 0; i < pieces.Length; i++)
                  {
                        int index = i;
                        playVoiceOverHandlers[index] = () => PlayVO(index);
                        Piece piece = pieces[index];
                        piece.Attached += playVoiceOverHandlers[index];
                        piece.Selected += ResetIdleTime;
                  }
            }
            private void OnDisable()
            {
                  if (playVoiceOverHandlers != null)
                  {
                        for (int i = 0; i < pieces.Length; i++)
                        {
                              Piece piece = pieces[i];
                              piece.Attached -= playVoiceOverHandlers[i];
                              piece.Selected -= ResetIdleTime;
                        }
                  }
                  if (voiceOverRoutine != null)
                  {
                        StopCoroutine(voiceOverRoutine);
                        voiceOverRoutine = null;
                  }
            }
            private void Update()
            {
                  if (voiceSource.isPlaying) return;
                  if (idleTime < hintInterval) idleTime += Time.deltaTime;
                  else
                  {
                        hint.Show(GetRandomUnattachedPiecePosition);
                        idleTime = 0F;
                  }
            }

            public void PlayVO(int index)
            {
                  if (voiceOverRoutine != null)
                  {
                        StopCoroutine(voiceOverRoutine);
                        voiceSource.Stop();
                  }
                  var clip = voiceOverClips[index];
                  voiceSource.PlayOneShot(clip);
                  voiceOverRoutine = StartCoroutine(WaitForClipLength(clip));
            }
            private IEnumerator WaitForClipLength(AudioClip clip)
            {
                  SetInteractable(false);
                  yield return new WaitForSeconds(clip.length);
                  SetInteractable(true);
                  voiceOverRoutine = null;
            }

            private void ResetIdleTime()
            {
                  idleTime = 0;
                  hint.Hide();
            }
            private void SetInteractable(bool value)
            {
                  for (int i = 0; i < pieces.Length; i++)
                  {
                        Piece piece = pieces[i];

                        if (!piece.IsAttached) piece.Interactable = value;
                  }
            }
      }
}