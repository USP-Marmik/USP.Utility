using UnityEngine;

namespace USP.Utility
{
	[RequireComponent(typeof(Camera))]
	public class CameraAutoFit : MonoBehaviour
	{
		public enum FitMode { Horizontal, Vertical }

		public new Camera camera;


		[Header("C O N F I G U R A T I O N")]
		public SpriteRenderer background;
		public Vector2 backgroundPadding;
		public FitMode mode;

		[Space(2F)]
		public float maxOrthographicSize = 5.4F;
		[SerializeField] private bool autoApplyOnStart = true;


		private void Reset()
		{
			camera = GetComponent<Camera>();
		}
		public void Start()
		{
			if (autoApplyOnStart) Apply();
		}

		public void Apply() => Apply(mode, background, backgroundPadding);
		public void Apply(FitMode mode, SpriteRenderer background, Vector2 padding = default)
		{
			if (background == null) throw new System.ArgumentNullException(nameof(background));

			Bounds bounds = background.bounds;
			bounds.Expand(new Vector3(padding.x * 2F, padding.y * 2F));

			float aspect = (float) Screen.width / Screen.height;

			float orthographicSize = mode switch { FitMode.Horizontal => bounds.extents.x / aspect, FitMode.Vertical => bounds.extents.y, _ => camera.orthographicSize };
			float target = Mathf.Min(orthographicSize, maxOrthographicSize);

			camera.orthographicSize = target;

			Vector3 position = camera.transform.position;
			position.x = bounds.center.x;
			position.y = bounds.center.y;
			camera.transform.position = position;
		}
	}
}