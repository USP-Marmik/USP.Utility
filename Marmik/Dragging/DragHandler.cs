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
            public ContactFilter2D Filter = new() { useTriggers = true };
            public int MaxResults = 3;

            private DraggableObject currentObject;
            private Collider2D[] results;

            private Vector2 PointerPosition => camera.ScreenToWorldPoint(position.ReadValue<Vector2>());


            private void Reset()
            {
                  camera = FindAnyObjectByType<Camera>();
            }
            private void Awake()
            {
                  results = new Collider2D[MaxResults];
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
                  if (currentObject == null) return;
                  if (currentObject.enabled == false)
                  {
                        currentObject = null;
                        return;
                  }
                  currentObject.DragTo(PointerPosition);
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
                  if (context.phase is InputActionPhase.Started)
                  {
                        int count = Physics2D.OverlapPoint(PointerPosition, Filter, results);
                        for (int i = 0; i < count; i++)
                        {
                              if (results[i].TryGetComponent(out DraggableObject obj) && obj.enabled)
                              {
                                    (currentObject = obj).Pick();
                                    break;
                              }
                        }
                  }
                  if (context.phase is InputActionPhase.Canceled && currentObject != null)
                  {
                        currentObject.Release();
                        currentObject = null;
                  }
            }
      }
}