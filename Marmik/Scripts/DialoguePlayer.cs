using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace USP.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class DialoguePlayer : MonoBehaviour
	{
		public sealed class Playback : CustomYieldInstruction
		{
			public AudioClip Clip;
			public bool IsComplete;

			public override bool keepWaiting => !IsComplete;
		}

		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioClip[] audioClips_female, audioClips_male;

		private readonly Queue<Playback> playbackQueue = new();
		private Coroutine runner;

		[field: SerializeField] public bool IsFemale { get; set; } = true;
		public AudioClip[] ActiveClips => IsFemale ? audioClips_female : audioClips_male;


		private void Reset()
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}

		public void Play(AudioClip clip)
		{
			CancelQueue();
			if (clip == null) return;

			audioSource.Stop();
			audioSource.clip = clip;
			audioSource.Play();
		}
		public void Play(int index)
		{
			if (IsValidIndex(index)) Play(ActiveClips[index]);
		}

		private IEnumerator RunQueueRoutine()
		{
			while (audioSource.isPlaying) yield return null;
			while (playbackQueue.Count > 0)
			{
				Playback playback = playbackQueue.Dequeue();
				audioSource.PlayOneShot(playback.Clip);
				while (audioSource.isPlaying) yield return null;
				playback.IsComplete = true;
			}
			runner = null;
		}

		public Playback Enqueue(AudioClip clip)
		{
			Playback playback = new();
			if (clip == null)
			{
				playback.IsComplete = true;
				return playback;
			}
			playback.Clip = clip;
			playbackQueue.Enqueue(playback);

			runner ??= StartCoroutine(RunQueueRoutine());
			return playback;
		}
		public Playback Enqueue(int index) => IsValidIndex(index) ? Enqueue(ActiveClips[index]) : Enqueue(null);

		public void Stop()
		{
			audioSource.Stop();
			if (runner != null)
			{
				StopCoroutine(runner);
				runner = null;
			}
			CancelQueue();
		}
		private bool IsValidIndex(int index)
		{
			if (index < 0 || index >= (ActiveClips.Length))
			{
				Debug.LogWarning($"[{typeof(DialoguePlayer).Name}] Invalid audio clip index: {index}");
				return false;
			}
			return true;
		}
		private void CancelQueue()
		{
			foreach (Playback playback in playbackQueue)
			{
				playback.IsComplete = true;
			}
			playbackQueue.Clear();
		}
	}
}