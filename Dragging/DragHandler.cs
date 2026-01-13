using UnityEngine;
using UnityEngine.InputSystem;

namespace USP.Utility
{
      public sealed class DragHandler : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new Camera camera;

            [Header("• I N P U T ")]
            [SerializeField] private InputAction position = new("Pointer Position", InputActionType.Value, "<Pointer>/position");
            [SerializeField] private InputAction press = new("Pointer Press", InputActionType.Button, "<Pointer>/press");

            [Header("• C O N F I G U R A T I O N")]
            public ContactFilter2D Filter;
            public int MaxResults;
            private Collider2D[] hits;

            private DraggableObject current;

            private Vector2 WorldPosition => camera.ScreenToWorldPoint(position.ReadValue<Vector2>());


            private void Reset()
            {
                  camera = FindAnyObjectByType<Camera>();
            }
            private void Awake()
            {
                  hits = new Collider2D[MaxResults];
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
                              {
                                    int count = Physics2D.OverlapPoint(WorldPosition, Filter, hits);
                                    for (int i = 0; i < count; i++)
                                    {
                                          var result = hits[i];
                                          if (result.TryGetComponent(out DraggableObject obj) && obj.enabled)
                                          {
                                                (current = obj).Pick();
                                                break;
                                          }
                                    }
                                    break;
                              }
                        case InputActionPhase.Canceled when current != null:
                              {
                                    current.Release();
                                    current = null;
                                    break;
                              }
                        default: return;
                  }
            }
      }
}