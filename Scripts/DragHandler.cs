using UnityEngine;

namespace USP.Utility
{
      public class DragHandler : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new Camera camera;

            [Header("• C O N F I G U R A T I O N")]
            public ContactFilter2D Filter;

            private readonly Collider2D[] hits = new Collider2D[3];
            private Draggable current;

            private Vector2 WorldPosition => camera.ScreenToWorldPoint(InputController.Position);


            private void Reset()
            {
                  camera = FindAnyObjectByType<Camera>();
            }
            private void Update()
            {
                  if (InputController.WasPressed)
                  {
                        int count = Physics2D.OverlapPoint(WorldPosition, Filter, hits);
                        if (count == 0) return;

                        foreach (Collider2D collider in hits)
                        {
                              if (collider.TryGetComponent(out Draggable d))
                              {
                                    current = d;
                                    current.Begin();
                                    break;
                              }
                        }
                  }

                  if (current == null) return;

                  if (InputController.IsHeld) current.Position = WorldPosition;
                  if (InputController.WasReleased)
                  {
                        current.End();
                        current = null;
                  }
            }
      }
}