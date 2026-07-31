using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace USP.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class SFX : Lightweight<SFX>
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

		public static void Play(int index)
		{
			Sound sound = Instance.sounds[index];
			Instance.source.PlayOneShot(sound.clip, sound.volume);
		}
		public static void Play(string name)
		{
			foreach (var sound in Instance.sounds)
			{
				if (!sound.clip.name.Contains(name, StringComparison.OrdinalIgnoreCase)) continue;
				Instance.source.PlayOneShot(sound.clip, sound.volume);
				break;
			}
		}
		public static void PlayRandom(params int[] indices)
		{
			int index = indices[Random.Range(0, indices.Length)];
			Play(index);
		}
	}
}