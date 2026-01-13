using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(AudioSource))]
      public class AudioPlayer : MonoBehaviour
      {
            private AudioSource source;

            [SerializeField] private AudioClip[] audioClips;


            private void Awake()
            {
                  source = GetComponent<AudioSource>();
            }

            public void PlayImmediate(int index) => source.PlayOneShot(audioClips[index]);
            public void PlayImmediate(int index, float volume) => source.PlayOneShot(audioClips[index], volume);
            public void Play(int index)
            {
                  if (source.isPlaying) source.Stop();
                  PlayImmediate(index);
            }
            public void Play(int index, float volume)
            {
                  if (source.isPlaying) source.Stop();
                  PlayImmediate(index, volume);
            }
      }
}