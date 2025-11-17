using UnityEngine;

namespace USP.Utility
{
      public class CameraFitter : MonoBehaviour
      {
            public Sprite Background;
            public Camera Camera;

            public float MaxOrthographicSize = 5.4F;


            public void Start()
            {
                  Execute();
            }

            public void Execute()
            {
                  float aspectRatio = (float) Screen.width / Screen.height;
                  float requiredSize = Background.bounds.size.x / (2F * aspectRatio);
                  Camera.orthographicSize = Mathf.Min(requiredSize, MaxOrthographicSize);
            }
      }
}