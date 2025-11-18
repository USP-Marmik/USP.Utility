using UnityEngine;

using DG.Tweening;
using System;

namespace USP
{
      public class TransitionController : MonoBehaviour
      {
            [SerializeField] private Transition[] transitions;


            public void Play(int index, TweenCallback midpoint, float interval, TweenCallback finish = null)
            {
                  var selected = transitions[Mathf.Clamp(index, 0, transitions.Length - 1)];
                  selected.Play(midpoint, interval, finish);
            }
            public void PlayRandom(TweenCallback midpoint, float interval, TweenCallback finish = null) => Play(UnityEngine.Random.Range(0, transitions.Length), midpoint, interval, finish);
      }
}