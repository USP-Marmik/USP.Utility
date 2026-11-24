using DG.Tweening;
using Emp37.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TubeSlide : MonoBehaviour
{
	[Separator(Shade.Icterine)]
	[StylizedTitle("References", Shade.Icterine)]
	[SerializeField] private LayoutGroup layoutGroup;
	[SerializeField] private Image tubeTemplate;
	private Image[] tubes;
	[Space(4F)]

	[Separator(Shade.Icterine)]
	[StylizedTitle("Animation Settings", Shade.Icterine)]
	[SerializeField, Range(1, 10)] private byte tubeCount = 5;
	[SerializeField] private float expandDuration = 1F, collapseDuration = 1F, intervalDuration = 1F;
	[SerializeField] private Ease expandEase, collapseEase;
	[Space(4F)]

	[Separator(Shade.Icterine)]
	[StylizedTitle("Visual Settings", Shade.Icterine)]
	[SerializeField] private bool useGradient;
	[SerializeField, ShowIf(nameof(useGradient), Invert = true)] private Color primary, secondary;
	[SerializeField, ShowIf(nameof(useGradient))] private Gradient gradient = new();
	[Space(4F)]

	[Separator(Shade.Icterine)]
	[StylizedTitle("Events", Shade.Icterine)]
	public UnityEvent OnTransition;


	private void Awake()
	{
		tubes = new Image[tubeCount];
		for (int i = 0; i < tubeCount; i++)
		{
			Image image = Instantiate(tubeTemplate, layoutGroup.transform);
			image.color = useGradient ? gradient.Evaluate(tubeCount > 1 ? i / (float) (tubeCount - 1) : 0F) : (i % 2 == 0 ? primary : secondary);
			tubes[i] = image;
		}
	}

	[Button(Size.Large, Tint = Shade.Icterine, Name = "Preview")]
	public void Play()
	{
		SetTubesActive(true);
		var seq = DOTween.Sequence().SetLink(gameObject);
		for (int i = 0; i < tubes.Length; i++) seq.Append(tubes[i].transform.DOScaleY(1F, expandDuration).From(0F).SetEase(expandEase));
		seq.AppendCallback(OnTransition.Invoke).AppendInterval(intervalDuration);
		for (int i = tubes.Length - 1; i >= 0; i--) seq.Append(tubes[i].transform.DOScaleY(0F, collapseDuration).SetEase(collapseEase));
		seq.OnComplete(() => SetTubesActive(false));
	}
	private void SetTubesActive(bool active)
	{
		for (int i = 0; i < tubes.Length; i++) tubes[i].gameObject.SetActive(active);
	}
}