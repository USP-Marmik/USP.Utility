using UnityEngine;

namespace USP.Utility
{
	[RequireComponent(typeof(Camera))]
	public class CameraAutoFit : MonoBehaviour
	{
		public enum FitMode { Horizontal, Vertical }

		public Camera Camera;
		public SpriteRenderer Background;
		public Vector2 BackgroundPadding;

		public FitMode Mode;
		public float MaxOrthographicSize = 5.4F;

		[SerializeField] private bool autoApplyOnStart = true;


		private void Reset()
		{
			Camera = GetComponent<Camera>();
		}
		public void Start()
		{
			if (autoApplyOnStart) Apply(Mode);
		}

		public void Apply(FitMode mode)
		{
			if (Background == null)
			{
				Debug.LogError(typeof(CameraAutoFit).Name + ": Background is not assigned.", this);
				return;
			}

			Bounds bounds = Background.bounds;
			bounds.Expand(new Vector3(BackgroundPadding.x * 2F, BackgroundPadding.y * 2F));

			float aspect = (float) Screen.width / Screen.height;
			float target = Mathf.Min(mode switch { FitMode.Horizontal => bounds.extents.x / aspect, FitMode.Vertical => bounds.extents.y, _ => Camera.orthographicSize }, MaxOrthographicSize);

			Camera.orthographicSize = target;

			Vector3 position = Camera.transform.position;
			position.x = bounds.center.x;
			position.y = bounds.center.y;
			Camera.transform.position = position;
		}
	}
}