using UnityEngine;

namespace USP
{
      [DefaultExecutionOrder(1)]
      public class GameManager : MonoBehaviour
      {
            public ScreenOrientation Orientation;
            public bool IsMultiTouch;


            private void Start()
            {
                  Screen.orientation = Orientation;
            }
            private void OnEnable()
            {
                  Input.multiTouchEnabled = IsMultiTouch;
            }
            private void OnDisable()
            {
                  if (IsMultiTouch)
                  {
                        Input.multiTouchEnabled = false;
                  }
            }
      }
}