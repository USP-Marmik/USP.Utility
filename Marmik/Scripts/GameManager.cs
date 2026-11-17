using UnityEngine;

namespace USP.Utility
{
	public class GameManager : MonoBehaviour
	{
		public bool IsMultiTouch;
		public bool UnlockFrameRate;
#if UNITY_EDITOR
		public float TimeScale = 1F;
#endif

		private void Awake()
		{
			Input.multiTouchEnabled = IsMultiTouch;

			if (UnlockFrameRate) Application.targetFrameRate = 400;

#if UNITY_EDITOR
			Time.timeScale = TimeScale;
#endif
		}
	}
}