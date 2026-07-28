using UnityEngine;
using DG.Tweening;

namespace USP.Utility
{
	public class PunchEffect : MonoBehaviour
	{
		[SerializeField] private DraggableObject draggable;

		public float Offset = 0.1F;
		public float Duration = 0.25F;
		public int Vibration = 1;
		public float Elasticity = 0.8F;

		private Tween tween;


		private void Reset()
		{
			draggable = GetComponent<DraggableObject>();
		}
		private void OnEnable()
		{
			tween = transform
			   .DOPunchScale(Vector3.one * Offset, Duration, Vibration, Elasticity)
			   .SetAutoKill(false)
			   .OnKill(() => tween = null)
			   .Pause();

			draggable.OnPick.AddListener(Punch);
		}
		private void OnDisable()
		{
			tween?.Kill();

			draggable.OnPick.RemoveListener(Punch);
		}

		public void Punch() => tween.Restart(false);
	}
}