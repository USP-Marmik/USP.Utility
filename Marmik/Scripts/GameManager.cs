using UnityEngine;

namespace USP.Utility
{
	[DefaultExecutionOrder(1)]
	public class GameManager : MonoBehaviour
	{
		public ScreenOrientation Orientation = ScreenOrientation.LandscapeLeft;
		public bool IsMultiTouch;

		public bool UnlockFrameRate;


		private void Awake()
		{
			Screen.orientation = Orientation;
			Input.multiTouchEnabled = IsMultiTouch;

			if (UnlockFrameRate) Application.targetFrameRate = 400;
		}
	}
}