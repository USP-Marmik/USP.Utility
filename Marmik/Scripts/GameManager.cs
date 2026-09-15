using UnityEngine;
using USP.Data;

namespace USP.Utility
{
	public class GameManager : MonoBehaviour
	{
		public bool IsMultiTouch;
		public bool UnlockFrameRate;

		[SerializeField] private AudioSource music, voice;


		private void Awake()
		{
			Input.multiTouchEnabled = IsMultiTouch;

			if (UnlockFrameRate) Application.targetFrameRate = 400;

			if (music != null) music.mute = !OfflineChildData.Instance.GetSettingToggleStates(SettingToggle.Music);
			if (voice != null) voice.mute = !OfflineChildData.Instance.GetSettingToggleStates(SettingToggle.Sound);
		}
	}
}