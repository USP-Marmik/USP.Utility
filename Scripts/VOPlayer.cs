using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(AudioSource))]
      public class VOPlayer : MonoBehaviour
      {
            [Serializable]
            public sealed class Playback : CustomYieldInstruction
            {
                  internal bool complete;

                  public bool IsComplete => complete;
                  public override bool keepWaiting => !complete;
            }

            [SerializeField] private AudioSource source;
            [SerializeField] private AudioClip[] audioClips;

            private readonly Queue<Playback> queue = new();
            private readonly Dictionary<Playback, AudioClip> map = new();
            private Coroutine runner;


            private IEnumerator RunQueue
            {
                  get
                  {
                        while (queue.Count > 0)
                        {
                              Playback playback = queue.Dequeue();
                              if (playback.complete)
                              {
                                    map.Remove(playback);
                                    continue;
                              }
                              yield return new WaitWhile(() => source.isPlaying);

                              source.PlayOneShot(map[playback]);
                              map.Remove(playback);

                              yield return new WaitWhile(() => source.isPlaying);
                              playback.complete = true;
                        }
                        runner = null;
                  }
            }


            private void Reset()
            {
                  source = GetComponent<AudioSource>();
            }

            public void Play(AudioClip clip)
            {
                  CancelPlayback();
                  if (clip == null) return;

                  source.clip = clip;
                  source.Play();
            }
            public void Play(int index) => Play(audioClips[index]);
            public Playback Queue(AudioClip clip)
            {
                  Playback playback = new();
                  if (clip == null)
                  {
                        playback.complete = true;
                        return playback;
                  }
                  queue.Enqueue(playback);
                  map[playback] = clip;
                  runner ??= StartCoroutine(RunQueue);
                  return playback;
            }
            public Playback Queue(int index) => Queue(audioClips[index]);
            public void Stop()
            {
                  source.Stop();
                  if (runner != null)
                  {
                        StopCoroutine(runner);
                        runner = null;
                  }
                  CancelPlayback();
            }

            private void CancelPlayback()
            {
                  foreach (var playback in queue) playback.complete = true;
                  queue.Clear(); map.Clear();
            }
      }
}