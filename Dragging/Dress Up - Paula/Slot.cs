using UnityEngine;
using DG.Tweening;

namespace USP.Utility
{
	[RequireComponent(typeof(Collider2D))]
	public class Slot : MonoBehaviour
	{
		[Header("? R E F E R E N C E S")]
		[SerializeField] private new Collider2D collider;
		[SerializeField] private SpriteRenderer silhoutte, key;

		[Header("? T W E E N   S E T T I N G S")]
		public float fadeTweenDuration = 0.37F;
		public Ease fadeTweenEase = Ease.OutCubic;

		private Tweener fadeTween;

		public string Key => key.sprite.name;
		public int SortingOrder => silhoutte != null ? silhoutte.sortingOrder : 0;


		private void Reset()
		{
			collider = GetComponent<Collider2D>();
			collider.isTrigger = true;

			var renderers = GetComponentsInChildren<SpriteRenderer>();
			silhoutte = renderers[0]; key = renderers[1];

			key.sortingOrder = silhoutte.sortingOrder - 1;
		}
		private void OnEnable()
		{
			collider.enabled = key.enabled = true;

			fadeTween = silhoutte.DOColor(default, fadeTweenDuration)
				.SetEase(fadeTweenEase)
				.OnKill(() => fadeTween = null)
				.SetAutoKill(false)
				.Pause();
		}
		private void OnDisable()
		{
			collider.enabled = key.enabled = false;

			fadeTween?.Kill();
		}

		public void Fade(float alpha)
		{
			if (alpha == 0) fadeTween.OnComplete(() => enabled = false);
			Color color = silhoutte.color;
			color.a = Mathf.Clamp01(alpha);
			fadeTween.ChangeEndValue(color, true).Restart();
		}
	}
}