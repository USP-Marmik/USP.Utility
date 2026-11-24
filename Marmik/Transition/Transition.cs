using Emp37.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
	public abstract class Transition : MonoBehaviour
	{
		[Separator(Shade.Icterine)]
		[StylizedTitle("Events", Shade.Icterine)]
		public UnityEvent OnTransition;

		public abstract void Play();
	}
}