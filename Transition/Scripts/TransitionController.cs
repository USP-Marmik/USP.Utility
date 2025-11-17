using UnityEngine;

using DG.Tweening;

namespace USP
{
      public class TransitionController : MonoBehaviour
      {
            [SerializeField] private Transition[] transitions;


            public void Play(int index, TweenCallback callback, float interval = 1F) => transitions[Mathf.Clamp(index, 0, transitions.Length - 1)].Play(callback, interval);
            public void PlayRandom(TweenCallback callback, float interval = 1F) => transitions[Random.Range(0, transitions.Length)].Play(callback, interval);
      }
}