using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Timeline
{
	public class TimelineManager : MonoBehaviour
	{
		private TimelineGamePreset _gamePreset = default;

		private List<TimeCard> _cards = new List<TimeCard>();
		private TimeCard _activeCard;

		[SerializeField]
		private TimeCard cardPrefab = default;
		[SerializeField]
		private RectTransform stepPrefab = default;
		[SerializeField]
		private StepItem yearPrefab = default;
		[SerializeField]
		private RectTransform timeLine = default;
		[SerializeField]
		private RectTransform cardContainer;
		[SerializeField]
		private TimelineHitZone hitZonePrefab = default;
		[SerializeField]
		private HorizontalLayoutGroup timeLineLayout = default;

		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		public void ActivateCard()
		{
			List<TimeCard> availableCards = _cards.Where(x => !x.done).ToList();
			if (availableCards.Count > 0)
			{
				_activeCard = availableCards[Random.Range(0, availableCards.Count)];
				_activeCard.Activate();
			}
			else
			{
				gameDone.Invoke();
			}
		}

		private void Start()
		{
			_gamePreset = FindObjectOfType<PresetHolder>().gamePreset.timelinePreset;

			LoadCardsFromPreset();

			GenerateTimeline();

			ActivateCard();
		}

		private void GenerateTimeline()
		{
			float               leftoverWidth = timeLine.GetComponent<RectTransform>().rect.width;
			List<RectTransform> stepsToScale  = new List<RectTransform>();

			StepItem item = Instantiate(yearPrefab, timeLine);
			item.Init(_gamePreset.timeRanges[0].start);
			leftoverWidth -= timeLineLayout.spacing * 2;

			foreach (TimeRange timeRange in _gamePreset.timeRanges)
			{
				for (int year = timeRange.start; year < timeRange.end; year += timeRange.stepSize)
				{
					stepsToScale.Add(Instantiate(stepPrefab, timeLine));
					leftoverWidth -= timeLineLayout.spacing;

					if (_cards.Any(x => x.year.Between(year, year + timeRange.stepSize, true)))
					{
						TimeCard relevantCard =
							_cards.First(x => x.year.Between(year, year + timeRange.stepSize, true));

						TimelineHitZone hitZone =
							Instantiate(hitZonePrefab, stepsToScale[stepsToScale.Count - 1].transform);
						relevantCard.hitZone = hitZone;
						hitZone.onHit.AddListener(CheckCard);
					}
				}

				item = Instantiate(yearPrefab, timeLine);
				item.Init(timeRange.end);
				leftoverWidth -= timeLineLayout.spacing * 2;
			}

			leftoverWidth /= stepsToScale.Count;
			for (int i = 0;
				i < stepsToScale.Count;
				i++)
			{
				stepsToScale[i].sizeDelta = new Vector2(leftoverWidth, timeLine.rect.size.y);
			}
		}

		private void LoadCardsFromPreset()
		{
			foreach (CardData cardData in _gamePreset.cards)
			{
				TimeCard card = Instantiate(cardPrefab, cardContainer);
				card.Init(cardData);
				_cards.Add(card);
			}
		}

		private void CheckCard()
		{
			_activeCard.Move();
			_activeCard.done = true;
			_activeCard.hitZone.gameObject.SetActive(false);
			ActivateCard();
		}
	}
}