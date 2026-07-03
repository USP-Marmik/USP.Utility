using UnityEngine;

namespace USP.Utility
{
	public static class GameUtility
	{
		public static bool IsPhoneLikeAspectRatio
		{
			get
			{
				float aspect = (float) Screen.width / Screen.height;
				return aspect <= 1.29F || aspect >= 1.36F;
			}
		}
	}
}