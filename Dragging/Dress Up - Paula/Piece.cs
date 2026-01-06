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
            [SerializeField] private Sprite key;
            [SerializeField] private DraggableObject drag;
            [SerializeField] private new Rigidbody2D rigidbody;

            [Header("• T W E E N")]
            public float AttachDuration = 0.2F;
            public Ease AttachEase = Ease.InOutSine;

            public bool IsLocked { get; private set; }
            public string Key => key != null ? key.name : renderer.sprite.name;

            public UnityEvent OnPick => drag.OnPick;
            public UnityEvent OnRelease => drag.OnRelease;


            private void Reset()
            {
                  renderer = GetComponent<SpriteRenderer>();
                  drag = GetComponent<DraggableObject>();
                  rigidbody = GetComponent<Rigidbody2D>();
            }
            private void OnEnable()
            {
                  drag.enabled = rigidbody.simulated = true;
            }
            private void OnDisable()
            {
                  drag.enabled = rigidbody.simulated = false;
            }


            public void Attach(Transform parent, int order)
            {
                  IsLocked = true;

                  transform.SetParent(parent.transform, true);
                  transform.DOLocalMove(Vector2.zero, AttachDuration).SetEase(AttachEase);

                  if (key != null) renderer.sprite = key;
                  renderer.sortingOrder = order;
            }
      }
}