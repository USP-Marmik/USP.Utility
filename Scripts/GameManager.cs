using UnityEngine;

namespace USP.Utility
{
      [DefaultExecutionOrder(1)]
      public class GameManager : MonoBehaviour
      {
            public ScreenOrientation Orientation;
            public bool IsMultiTouch;


            private void Awake()
            {
                  Screen.orientation = Orientation;
                  Input.multiTouchEnabled = IsMultiTouch;
            }
      }
}