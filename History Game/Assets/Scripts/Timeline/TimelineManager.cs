using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Timeline
{
	public class TimelineManager : MonoBehaviour
	{
		private TimelineGamePreset _gamePreset = default;
		private List<TimeCard> _cards = new List<TimeCard>();
		private TimeCard _activeCard;

		[SerializeField]
		private float allowedInaccuracy;

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
			if (availableCards.Count > 0)
			{
				availableCards = availableCards.Where(x => x.ready).ToList();
				if (availableCards.Count > 0)
				{
					_activeCard = availableCards[Random.Range(0, availableCards.Count)];
					_activeCard.Activate();
				}
				else
				{
					StartCoroutine(CheckCards(.75f));
				}
			}
			else
			{
				gameDone.Invoke();
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
					card.transform.localScale = Vector3.one;
					card.transform.SetParent(cardContainer);
					card.ready = true;
				}
				else
				{
					card.done = true;
					card.Move();
				}
			}

			ActivateCard();
		}
	}
}