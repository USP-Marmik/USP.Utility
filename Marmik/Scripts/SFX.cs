using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace USP.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class SFX : MonoBehaviour
	{
		[Serializable]
		public class Sound
		{
			public AudioClip clip;
			[Range(0F, 1F)] public float volume = 1F;
		}

		[Header("- R E F E R E N C E S")]
		[SerializeField] private AudioSource source;
		[SerializeField] private Sound[] sounds;


		private void Reset()
		{
			source = GetComponent<AudioSource>();
			source.playOnAwake = false;
		}

		public void PlayExclusive(int index)
		{
			if (source.isPlaying) return;
			Play(index);
		}
		public void Play(int index)
		{
			Sound sound = sounds[index];
			source.PlayOneShot(sound.clip, sound.volume);
		}
		public void Play(string name)
		{
			foreach (var sound in sounds)
			{
				if (!sound.clip.name.Contains(name, StringComparison.OrdinalIgnoreCase)) continue;
				source.PlayOneShot(sound.clip, sound.volume);
				break;
			}
		}
		public void PlayRandom(params int[] indices)
		{
			int index = indices[Random.Range(0, indices.Length)];
			Play(index);
		}
	}
}