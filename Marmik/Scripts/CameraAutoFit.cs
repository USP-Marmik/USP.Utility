using UnityEngine;

namespace USP.Utility
{
	[RequireComponent(typeof(Camera))]
	public class CameraAutoFit : MonoBehaviour
	{
		public enum FitMode { Horizontal, Vertical }

		public Camera Camera;

		[Space(2F)]
		public float MaxOrthographicSize = 5.4F;

		[Header("C O N F I G U R A T I O N")]
		[SerializeField] private SpriteRenderer background;
		[SerializeField] private Vector2 backgroundPadding;
		[SerializeField] private FitMode mode;

		[SerializeField] private bool autoApplyOnStart = true;


		private void Reset()
		{
			Camera = GetComponent<Camera>();
		}
		public void Start()
		{
			if (autoApplyOnStart) Apply();
		}

		public void Apply() => Apply(mode, background, backgroundPadding);
		public void Apply(FitMode mode) => Apply(mode, background, backgroundPadding);
		public void Apply(FitMode mode, SpriteRenderer background, Vector2 padding = default)
		{
			if (background == null)
			{
				throw new System.ArgumentNullException(nameof(background));
			}

			Bounds bounds = background.bounds;
			bounds.Expand(new Vector3(padding.x * 2F, padding.y * 2F));

			float aspect = (float) Screen.width / Screen.height;

			float orthographicSize = mode switch { FitMode.Horizontal => bounds.extents.x / aspect, FitMode.Vertical => bounds.extents.y, _ => Camera.orthographicSize };
			float target = Mathf.Min(orthographicSize, MaxOrthographicSize);

			Camera.orthographicSize = target;

			Vector3 position = Camera.transform.position;
			position.x = bounds.center.x;
			position.y = bounds.center.y;
			Camera.transform.position = position;
		}
	}
}