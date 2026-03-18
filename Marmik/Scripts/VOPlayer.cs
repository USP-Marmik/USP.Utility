using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace USP.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class VOPlayer : MonoBehaviour
	{
		public sealed class Playback : CustomYieldInstruction
		{
			public AudioClip Clip;
			public bool IsComplete;

			public override bool keepWaiting => !IsComplete;
		}

		[SerializeField] private AudioSource source;
		[SerializeField] private AudioClip[] audioClips;

		private readonly Queue<Playback> queue = new();
		private Coroutine runner;

		private IEnumerator RunQueue
		{
			get
			{
				yield return new WaitWhile(() => source.isPlaying);
				while (queue.Count > 0)
				{
					Playback playback = queue.Dequeue();
					source.PlayOneShot(playback.Clip);

					yield return new WaitWhile(() => source.isPlaying);
					playback.IsComplete = true;
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
			CancelQueued();
			if (clip == null) return;

			source.Stop();
			source.PlayOneShot(clip);
		}
		public void Play(int index) => Play(audioClips[index]);
		public Playback Queue(AudioClip clip)
		{
			Playback playback = new();
			if (clip == null)
			{
				playback.IsComplete = true;
				return playback;
			}
			playback.Clip = clip;
			queue.Enqueue(playback);

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
			CancelQueued();
		}
		private void CancelQueued()
		{
			foreach (Playback playback in queue) playback.IsComplete = true;
			queue.Clear();
		}
	}
}