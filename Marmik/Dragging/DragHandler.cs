using UnityEngine;
using UnityEngine.InputSystem;

namespace USP.Utility
{
	public sealed class DragHandler : MonoBehaviour
	{
		[Header("- R E F E R E N C E S")]
		[SerializeField] private new Camera camera;

		[Header("- I N P U T ")]
		[SerializeField] private InputAction position = new("Pointer Position", InputActionType.Value, "<Pointer>/position");
		[SerializeField] private InputAction press = new("Pointer Press", InputActionType.Button, "<Pointer>/press");

		private DraggableObject currentObject;
		private Collider2D[] results;

		[Header("- C O N F I G U R A T I O N")]
		public ContactFilter2D Filter = new() { useTriggers = true };
		public int MaxResults = 3;


		private Vector2 PointerWorldPosition
		{
			get
			{
				Vector2 screenPosition = position.ReadValue<Vector2>();
				return camera.ScreenToWorldPoint(screenPosition);
			}
		}

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

			press.started += OnStarted;
			press.canceled += OnCanceled;
		}
		private void LateUpdate()
		{
			if (currentObject == null) return;
			if (!currentObject.isActiveAndEnabled) { currentObject = null; return; }

			currentObject.DragTo(PointerWorldPosition);
		}
		private void OnDisable()
		{
			press.started -= OnStarted;
			press.canceled -= OnCanceled;

			position.Disable();
			press.Disable();
		}

		private void OnStarted(InputAction.CallbackContext _)
		{
			int count = Physics2D.OverlapPoint(PointerWorldPosition, Filter, results);
			for (int i = 0; i < count; i++)
			{
				if (results[i].TryGetComponent(out DraggableObject draggable) && draggable.isActiveAndEnabled)
				{
					(currentObject = draggable).Pick();
					break;
				}
			}
		}
		private void OnCanceled(InputAction.CallbackContext _)
		{
			if (currentObject == null) return;

			currentObject.Release();
			currentObject = null;
		}
	}
}