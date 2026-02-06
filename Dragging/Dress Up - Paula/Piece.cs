using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;

namespace USP.Utility
{
      [RequireComponent(typeof(SpriteRenderer), typeof(DraggableObject), typeof(Rigidbody2D))]
      public class Piece : MonoBehaviour
      {
            [Header("• R E F E R E N C E S")]
            [SerializeField] private new SpriteRenderer renderer;
            [SerializeField] private new Collider2D collider;
            [SerializeField] private DraggableObject draggable;
            [SerializeField] private Sprite keySprite;

            [Header("• T W E E N   S E T T I N G S")]
            [Min(0f)] public float attachDuration = 0.25F;
            public Ease attachEase = Ease.InOutSine;

            private Slot matchingSlot;

            public UnityEvent OnPick => draggable.OnPick;
            public UnityEvent OnRelease => draggable.OnRelease;
            public string PieceKey => keySprite != null ? keySprite.name : renderer.sprite.name;


            private void Reset()
            {
                  renderer = GetComponent<SpriteRenderer>();
                  collider = GetComponent<Collider2D>();
                  draggable = GetComponent<DraggableObject>();
            }
            private void OnEnable()
            {
                  draggable.enabled = collider.enabled = true;
                  draggable.OnRelease.AddListener(HandleRelease);
            }
            private void OnDisable()
            {
                  draggable.enabled = collider.enabled = false;
                  draggable.OnRelease.RemoveListener(HandleRelease);
            }
            private void OnTriggerEnter2D(Collider2D other)
            {
                  if (other.TryGetComponent(out Slot slot) && PieceKey == slot.Key)
                  {
                        slot.Fade(0.5F);
                        matchingSlot = slot;
                  }
            }
            private void OnTriggerExit2D(Collider2D other)
            {
                  if (other.TryGetComponent(out Slot slot) && PieceKey == slot.Key)
                  {
                        slot.Fade(1F);
                        matchingSlot = null;
                  }
            }

            private void HandleRelease()
            {
                  if (matchingSlot != null) Attach(matchingSlot);
            }
            private void Attach(Slot slot)
            {
                  enabled = false;

                  transform.SetParent(slot.transform, true);
                  transform.DOLocalMove(Vector2.zero, attachDuration).SetEase(attachEase).SetLink(gameObject);

                  if (keySprite != null) renderer.sprite = keySprite;
                  renderer.sortingOrder = slot.Order + 1;

                  slot.enabled = false;
            }
      }
}