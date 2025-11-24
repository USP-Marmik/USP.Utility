using UnityEngine;
using UnityEngine.InputSystem;

namespace USP.Utility
{
      public static class InputController
      {
            private static Touchscreen _touchscreen; 
            private static Mouse _mouse;

            public static Touchscreen Touchscreen => _touchscreen ??= Touchscreen.current;
            public static Mouse Mouse => _mouse ??= Mouse.current;

            public static Vector2 Position => Touchscreen?.primaryTouch.position.ReadValue() ?? Mouse?.position.ReadValue() ?? default;
            public static bool WasPressed => Touchscreen?.primaryTouch.press.wasPressedThisFrame ?? Mouse?.leftButton.wasPressedThisFrame ?? false;
            public static bool IsHeld => Touchscreen?.primaryTouch.press.isPressed ?? Mouse?.leftButton.isPressed ?? false;
            public static bool WasReleased => Touchscreen?.primaryTouch.press.wasReleasedThisFrame ?? Mouse?.leftButton.wasReleasedThisFrame ?? false;
            public static bool Any => WasPressed || IsHeld || WasReleased;
      }
}