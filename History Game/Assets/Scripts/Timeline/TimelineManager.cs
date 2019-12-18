using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
		private float allowedInaccuracy = default;

		[Header("Scene References")]
		[SerializeField]
		private TextMeshProUGUI descriptionText = default;
		[SerializeField]
		private TimeLine timeLine = default;
		[SerializeField]
		private RectTransform cardContainer = default;

		[Header("Prefab References")]
		[SerializeField]
		private TimeCard cardPrefab = default;

		[Space]
		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		public void ActivateCard()
		{
			if (_activeCard) _activeCard.Deactivate();

			List<TimeCard> availableCards = _cards.Where(x => !x.done).ToList();
			if (availableCards.Count < 1)
			{
				gameDone.Invoke();
				return;
			}

			if (cardContainer.childCount > 0)
			{
				TimeCard nextCard = cardContainer.GetChild(0).GetComponent<TimeCard>();
				_activeCard = nextCard;
				_activeCard.Activate();
			}
			else
			{
				StartCoroutine(CheckCards(.75f));
			}
		}

		public void TemporaryHit(Vector3 position)
		{
			_activeCard.ready = false;
			_activeCard.Move(position);
			ActivateCard();
		}

		private void Start()
		{
			_gamePreset          = FindObjectOfType<PresetHolder>().gamePreset.timelinePreset;
			descriptionText.text = _gamePreset.description;

			LoadCardsFromPreset();

			timeLine.GenerateTimeline(_gamePreset);
			timeLine.AddCards(_cards);

			ActivateCard();
		}

		private void LoadCardsFromPreset()
		{
			foreach (CardData cardData in _gamePreset.GetCards())
			{
				TimeCard card = Instantiate(cardPrefab, cardContainer);
				card.Init(cardData);
				_cards.Add(card);
			}
		}

		private IEnumerator CheckCards(float delay)
		{
			yield return new WaitForSeconds(delay);

			foreach (TimeCard card in _cards.Where(x => !x.done))
			{
				float distance = Vector3.Distance(
					Camera.main.WorldToScreenPoint(card.transform.position),
					Camera.main.WorldToScreenPoint(card.target.position));

				if (distance > allowedInaccuracy)
				{
					card.MoveBack();
					DelayedResetLayout(1f);
				}
				else
				{
					card.done = true;
					card.Move();
				}
			}

			ActivateCard();
		}

		private IEnumerator DelayedResetLayout(float delay)
		{
			yield return new WaitForSeconds(delay);
			LayoutRebuilder.ForceRebuildLayoutImmediate(cardContainer);
		}
	}
}