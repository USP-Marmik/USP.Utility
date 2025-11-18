using UnityEngine;

namespace USP.Utility
{
      [RequireComponent(typeof(Camera))]
      public class CameraAutoFit : MonoBehaviour
      {
            public enum FitMode { Horizontal, Vertical }

            public SpriteRenderer Background;
            public Camera Camera;

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
                  float aspectRatio = (float) Screen.width / Screen.height;

                  Vector2 backgroundSize = Background.sprite.bounds.size;
                  float orthographicSize = mode switch
                  {
                        FitMode.Horizontal => backgroundSize.x / (2F * aspectRatio),
                        FitMode.Vertical => Mathf.Max(backgroundSize.y / 2F, backgroundSize.x / (2F * aspectRatio)),
                        _ => MaxOrthographicSize
                  };

                  Camera.orthographicSize = Mathf.Min(orthographicSize, MaxOrthographicSize);
            }
      }
}