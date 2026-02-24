using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      [RequireComponent(typeof(AudioSource))]
      public class PiecesHandler : MonoBehaviour
      {
            [Header("A U D I O")]
            [SerializeField] private AudioSource voiceSource;
            [SerializeField] private AudioClip[] voiceOverClips;

            [Header("G A M E P L A Y")]
            [SerializeField] private Piece[] pieces;
            [SerializeField] private float hintInterval = 4F;

            [Header("E V E N T S")]
            public UnityEvent OnAssemblyComplete;

            private Action[] attachHandlers;
            private Coroutine voiceOverGateRoutine;
            private float idleTime;
            private int attachedCount;


            private void Reset()
            {
                  voiceSource = GetComponent<AudioSource>();
            }
            private void OnEnable()
            {
                  idleTime = 0F;

                  attachHandlers = new Action[pieces.Length];
                  for (int i = 0; i < pieces.Length; i++)
                  {
                        int index = i;
                        attachHandlers[index] = () => PlayVO(index);

                        Piece piece = pieces[i];
                        piece.OnAttach += attachHandlers[i];
                        piece.OnSelect += ResetIdleTime;
                  }
            }
            private void OnDisable()
            {
                  if (attachHandlers != null)
                  {
                        for (int i = 0; i < pieces.Length; i++)
                        {
                              Piece piece = pieces[i];
                              piece.OnAttach -= attachHandlers[i];
                              piece.OnSelect -= ResetIdleTime;
                        }
                  }
                  if (voiceOverGateRoutine != null)
                  {
                        StopCoroutine(voiceOverGateRoutine);
                        voiceOverGateRoutine = null;
                  }
            }
            private void Update()
            {
                  if (voiceSource.isPlaying) return;
                  if (idleTime < hintInterval) idleTime += Time.deltaTime;
                  else
                  {
                        idleTime = 0F;
                  }
            }

            public void PlayVO(int index)
            {
                  if (voiceOverGateRoutine != null)
                  {
                        StopCoroutine(voiceOverGateRoutine);
                        voiceSource.Stop();
                  }
                  var clip = voiceOverClips[index];
                  voiceSource.PlayOneShot(clip);
                  voiceOverGateRoutine = StartCoroutine(WaitForClipLength(clip));
            }
            private IEnumerator WaitForClipLength(AudioClip clip)
            {
                  attachedCount++;
                  SetInteractable(false);
                  yield return new WaitForSeconds(clip.length);
                  SetInteractable(true);

                  if (attachedCount == pieces.Length) OnAssemblyComplete.Invoke();
                  voiceOverGateRoutine = null;
            }
            private void ResetIdleTime()
            {
                  idleTime = 0;
            }
            private void SetInteractable(bool value)
            {
                  for (int i = 0; i < pieces.Length; i++)
                  {
                        Piece piece = pieces[i];
                        if (!piece.IsAttached) piece.IsDraggable = value;
                  }
            }
      }
}