using System.Collections.Generic;
using System.Linq;
using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
	public class TimeLine : MonoBehaviour, I_SmartwallInteractable
	{
		[Header("Prefab References")]
		[SerializeField]
		private StepItem minorStepPrefab = default;
		[SerializeField]
		private StepItem majorStepPrefab = default;

		[Space]
		public PositionUnityEvent hitEvent = new PositionUnityEvent();

		private List<StepItem> _steps = new List<StepItem>();
		private HorizontalLayoutGroup _layout;

		public void Hit(Vector3 hitPosition) => hitEvent.Invoke(hitPosition);
		private void Awake() => _layout = GetComponent<HorizontalLayoutGroup>();

		public void GenerateTimeline(TimelineGamePreset preset)
		{
			List<StepItem> generatedSteps = new List<StepItem>();

			float               leftoverWidth = ((RectTransform) transform).rect.width;
			List<RectTransform> stepsToScale  = new List<RectTransform>();

			StepItem item = Instantiate(majorStepPrefab, transform);
			item.InitMajor(preset.timeRanges[0].start);
			leftoverWidth -= _layout.spacing * 2;

			foreach (TimeRange timeRange in preset.timeRanges)
			{
				for (int currentYear = timeRange.start; currentYear < timeRange.end; currentYear += timeRange.stepSize)
				{
					StepItem minorStepItem = Instantiate(minorStepPrefab, transform);
					minorStepItem.InitMinor(currentYear, currentYear + timeRange.stepSize);

					stepsToScale.Add((RectTransform) minorStepItem.transform);
					generatedSteps.Add(minorStepItem);

					leftoverWidth -= _layout.spacing;
				}

				item = Instantiate(majorStepPrefab, transform);
				item.InitMajor(timeRange.end);
				leftoverWidth -= _layout.spacing * 2;
			}

			leftoverWidth /= stepsToScale.Count;
			for (int i = 0;
				i < stepsToScale.Count;
				i++)
			{
				stepsToScale[i].sizeDelta = new Vector2(leftoverWidth, ((RectTransform) transform).rect.size.y);
			}

			_steps = generatedSteps;
		}

		public void AddCards(List<TimeCard> cards)
		{
			foreach (TimeCard card in cards)
				card.target = _steps.First(x => card.year.Between(x.start, x.end, true)).transform;
		}
	}
}