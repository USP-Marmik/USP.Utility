using UnityEngine;

using DG.Tweening;

namespace USP.Utility
{
      public class TransitionController : MonoBehaviour
      {
            [SerializeField] private Transition[] transitions;


            private void Reset()
            {
                  transitions = FindObjectsByType<Transition>(FindObjectsSortMode.None);
            }

            public void Play(int index, TweenCallback midpoint, float hold, TweenCallback complete = null)
            {
                  Transition selected = transitions[Mathf.Clamp(index, 0, transitions.Length - 1)];
                  selected.Play(midpoint, hold, complete);
            }
            public void PlayRandom(TweenCallback midpoint, float hold, TweenCallback complete = null)
            {
                  int index = Random.Range(0, transitions.Length);
                  Play(index, midpoint, hold, complete);
            }
      }
}