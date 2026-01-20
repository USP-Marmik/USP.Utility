using System;
using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(AudioSource))]
      public class AudioPlayer : MonoBehaviour
      {
            [Serializable]
            private struct AudioCue
            {
                  [TextArea(1, 2)] public string Description;
                  public AudioClip Clip;
                  [Range(0F, 1F)] public float VolumeScale;
                  public bool Interrupts;
            }

            [SerializeField] private AudioSource source;
            [SerializeField] private AudioCue[] cues;


            private void Reset()
            {
                  source = GetComponent<AudioSource>();
            }

            public void Play(int index)
            {
                  var cue = cues[index];
                  if (cue.Interrupts && source.isPlaying) source.Stop();
                  if (cue.Clip != null) source.PlayOneShot(cue.Clip, cue.VolumeScale);
            }
      }
}