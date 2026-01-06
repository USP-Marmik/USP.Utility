using UnityEngine;
using UnityEngine.InputSystem;

namespace USP.Utility
{
      public sealed class DragHandler : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private Camera cam;

            [Header("• C O N F I G U R A T I O N")]
            [SerializeField] private InputAction position = new("Pointer Position", InputActionType.Value, "<Pointer>/position");
            [SerializeField] private InputAction press = new("Pointer Press", InputActionType.Button, "<Pointer>/press");
            public ContactFilter2D Filter;

            private readonly Collider2D[] hits = new Collider2D[3];
            private DraggableObject current;

            private Vector2 WorldPosition => cam.ScreenToWorldPoint(position.ReadValue<Vector2>());


            private void Reset()
            {
                  cam = FindAnyObjectByType<Camera>();
            }
            private void OnEnable()
            {
                  position.Enable();
                  press.Enable();

                  press.started += HandleAction;
                  press.canceled += HandleAction;
            }
            private void LateUpdate()
            {
                  if (current != null)
                  {
                        if (!current.enabled)
                        {
                              current = null;
                              return;
                        }
                        current.DragTo(WorldPosition);
                  }
            }
            private void OnDisable()
            {
                  press.started -= HandleAction;
                  press.canceled -= HandleAction;

                  position.Disable();
                  press.Disable();
            }

            private void HandleAction(InputAction.CallbackContext context)
            {
                  switch (context.phase)
                  {
                        case InputActionPhase.Started:
                              int count = Physics2D.OverlapPoint(WorldPosition, Filter, hits);
                              for (int i = 0; i < count; i++)
                              {
                                    Collider2D collider = hits[i];
                                    if (collider.TryGetComponent(out DraggableObject d) && d.enabled)
                                    {
                                          current = d;
                                          current.OnPick.Invoke();
                                          break;
                                    }
                              }
                              break;

                        case InputActionPhase.Canceled when current != null:
                              current.OnRelease.Invoke();
                              current = null;
                              break;

                        default: return;
                  }
            }
      }
}