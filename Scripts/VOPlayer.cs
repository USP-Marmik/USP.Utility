using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(AudioSource))]
      public class VOPlayer : MonoBehaviour
      {
            [SerializeField] private AudioSource source;
            [SerializeField] private AudioClip[] audioClips;

            private readonly Queue<AudioClip> queue = new();
            private Coroutine runner;

            private IEnumerator RunQueue
            {
                  get
                  {
                        while (queue.Count > 0)
                        {
                              yield return new WaitWhile(() => source.isPlaying);

                              source.clip = queue.Dequeue();
                              source.Play();
                        }
                        source.clip = null;
                        runner = null;
                  }
            }


            private void Reset()
            {
                  source = GetComponent<AudioSource>();
            }

            public void Play(AudioClip clip)
            {
                  source.clip = clip;
                  source.Play();
            }
            public void Play(int index)
            {
                  AudioClip clip = audioClips[index];
                  if (clip == null) return;

                  queue.Clear();
                  Play(clip);
            }
            public void Queue(AudioClip clip)
            {
                  queue.Enqueue(clip);

                  runner ??= StartCoroutine(RunQueue);
            }
            public void Queue(int index)
            {
                  AudioClip clip = audioClips[index];
                  if (clip == null) return;

                  Queue(clip);
            }
            public void Stop()
            {
                  source.Stop();
                  if (runner != null)
                  {
                        StopCoroutine(runner);
                        runner = null;
                  }
                  queue.Clear();
            }
      }
}