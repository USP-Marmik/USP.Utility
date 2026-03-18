using System;
using UnityEngine;
using DG.Tweening;

namespace USP.Utility
{
	[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
	[RequireComponent(typeof(DraggableObject))]
	public class Piece : MonoBehaviour
	{
		[Header("• R E F E R E N C E S")]
		[SerializeField] private new Transform transform;
		[SerializeField] private new SpriteRenderer renderer;
		[SerializeField] private new Rigidbody2D rigidbody;
		[SerializeField] private new Collider2D collider;
		[SerializeField] private DraggableObject draggable;
		[SerializeField] private Sprite keySprite;
		[SerializeField] private AudioClip infoClip;

		[Header("• C O N F I G U R A T I O N")]
		[SerializeField] private int overrideSortingOrder = 200;

		private Slot matchingSlot;
		private int originalSortingOrder;

		[Header("• T W E E N   S E T T I N G S")]
		public float embiggenTarget = 1;
		[Min(0F)] public float embiggenTweenDuration = 0.3F;
		public Ease embiggenTweenEase = Ease.InOutBack;
		private Tween embiggenTween;
		[Space(2F)]
		public float attachTweenScaleTarget = 1F;
		[Min(0F)] public float attachTweenMoveDuration = 0.25F, attachTweenScaleDuration = 0.5F;
		public Ease attachTweenMoveEase = Ease.InOutQuint, attachTweenScaleEase = Ease.OutBounce;

		public bool IsAttached { get; private set; }
		public AudioClip InfoClip => infoClip;
		public bool IsLocked { get => !draggable.enabled; set => draggable.enabled = !value; }

		public event Action Picked = delegate { }, Released = delegate { }, Attached = delegate { };


		private void Reset()
		{
			transform = base.transform;

			renderer = GetComponent<SpriteRenderer>();

			rigidbody = GetComponent<Rigidbody2D>();
			rigidbody.bodyType = RigidbodyType2D.Kinematic;
			rigidbody.sleepMode = RigidbodySleepMode2D.StartAsleep;

			collider = GetComponent<Collider2D>();
			collider.isTrigger = true;

			draggable = GetComponent<DraggableObject>();
			draggable.autoReturnOnRelease = true;
		}
		private void Awake()
		{
			originalSortingOrder = renderer.sortingOrder;
		}
		private void OnEnable()
		{
			IsAttached = false;

			draggable.enabled = collider.enabled = true;

			draggable.OnPick.AddListener(Pick);
			draggable.OnRelease.AddListener(Release);
			draggable.OnReturn.AddListener(Return);

			embiggenTween = transform.DOScale(Vector2.one * embiggenTarget, embiggenTweenDuration)
				.SetEase(embiggenTweenEase)
				.SetAutoKill(false)
				.OnKill(() => embiggenTween = null)
				.Pause();
		}
		private void OnDisable()
		{
			draggable.OnPick.RemoveListener(Pick);
			draggable.OnRelease.RemoveListener(Release);
			draggable.OnReturn.RemoveListener(Return);

			draggable.enabled = collider.enabled = false;

			embiggenTween?.Kill();
		}
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.TryGetComponent(out Slot slot) || !CompareKey(slot.Key)) return;

			slot.Fade(0.5F);
			matchingSlot = slot;
		}
		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.TryGetComponent(out Slot slot) || !CompareKey(slot.Key)) return;

			if (!IsAttached) slot.Fade(1F);
			matchingSlot = null;
		}

		private void Pick()
		{
			renderer.sortingOrder = overrideSortingOrder;
			embiggenTween.Restart();

			Picked();
		}
		private void Release()
		{
			if (matchingSlot != null)
			{
				Attach(matchingSlot);
				return;
			}
			embiggenTween.PlayBackwards();

			Released();
		}
		private void Return() => renderer.sortingOrder = originalSortingOrder;

		private void Attach(Slot slot)
		{
			transform.SetParent(slot.transform, true);

			IsAttached = true;
			Attached();

			DOTween.Sequence()
				.Append(transform.DOLocalMove(Vector2.zero, attachTweenMoveDuration).SetEase(attachTweenMoveEase).OnComplete(() => slot.Fade(0F)))
				.AppendCallback(() =>
				{
					if (keySprite != null) renderer.sprite = keySprite;
					renderer.sortingOrder = slot.SortingOrder + 1;
				})
				.Append(transform.DOScale(Vector2.one * attachTweenScaleTarget, attachTweenScaleDuration).SetEase(attachTweenScaleEase))
				.SetLink(gameObject)
				.Play();

			enabled = false;
		}
		private bool CompareKey(string otherKey) => (keySprite != null ? keySprite.name : renderer.sprite.name) == otherKey;
	}
}