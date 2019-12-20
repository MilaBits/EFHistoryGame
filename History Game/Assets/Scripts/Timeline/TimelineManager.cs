using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Timeline
{
	public class TimelineManager : MonoBehaviour
	{
		private TimelineGamePreset _gamePreset = default;
		private List<TimeCard> _cards = new List<TimeCard>();
		private TimeCard _activeCard;

		[SerializeField]
		private float allowedInaccuracy = default;

		[SerializeField]
		private AnimationCurve slideAnimation = default;

		[Header("Scene References")]
		[SerializeField]
		private TextMeshProUGUI descriptionText = default;
		[SerializeField]
		private TimeLine timeLine = default;
		[SerializeField]
		private List<RectTransform> cardSlots = default;

		[Header("Prefab References")]
		[SerializeField]
		private TimeCard cardPrefab = default;

		[Space]
		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		private bool _interactable = true;

		public void ActivateCard()
		{
			if (_activeCard) _activeCard.Deactivate();

			List<TimeCard> availableCards = _cards.Where(x => !x.done).ToList();
			if (availableCards.Count < 1)
			{
				gameDone.Invoke();
				return;
			}

			if (_cards.Any(x => x.ready))
				StartCoroutine(SlideCards());
			else
				StartCoroutine(CheckCards(.75f));
		}

		private IEnumerator SlideCards()
		{
			_interactable = false;
			for (int i = 1; i < cardSlots.Count; i++)
			{
				if (cardSlots[i].childCount > 0)
				{
					RectTransform firstEmpty = GetFirstEmptySlot();
					if (firstEmpty)
						yield return StartCoroutine(SlideCard((RectTransform) cardSlots[i].GetChild(0).transform,
															  firstEmpty, .15f));
				}
			}

			Transform firstFilledSlot = cardSlots.FirstOrDefault(x => x.childCount != 0);
			if (firstFilledSlot)
			{
				_activeCard = firstFilledSlot.GetChild(0).GetComponent<TimeCard>();
				_activeCard.Activate();
			}

			_interactable = true;
		}

		private RectTransform GetFirstEmptySlot() => cardSlots.FirstOrDefault(x => x.childCount == 0);

		private IEnumerator SlideCard(RectTransform card, RectTransform target, float duration)
		{
			Vector3 startPos = card.transform.position;

			for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
			{
//				card.transform.position = Vector3.Lerp(startPos, target.transform.position, elapsed / duration);
				card.transform.position = Vector3.LerpUnclamped(startPos, target.transform.position, slideAnimation.Evaluate(elapsed / duration));
				yield return null;
			}

			card.transform.position = target.transform.position;
			card.SetParent(target);
		}

		public void TemporaryHit(Vector3 position)
		{
			if (_activeCard.ready && _interactable)
			{
				_activeCard.ready = false;
				_activeCard.Move(position);
				ActivateCard();
			}
		}

		private void Start()
		{
			_gamePreset          = FindObjectOfType<PresetHolder>().gamePreset.timelinePreset;
			descriptionText.text = _gamePreset.description;

			LoadCardsFromPreset();

			timeLine.GenerateTimeline(_gamePreset);
			timeLine.AddCards(_cards);

			_activeCard = cardSlots[0].GetChild(0).GetComponent<TimeCard>();
			_activeCard.Activate();
		}

		private void LoadCardsFromPreset()
		{
			var list = _gamePreset.GetCards();
			for (int i = 0; i < list.Count; i++)
			{
				CardData cardData = list[i];
				TimeCard card     = Instantiate(cardPrefab, cardSlots[i]);
				card.Init(cardData);
				_cards.Add(card);
			}
		}

		private IEnumerator CheckCards(float delay)
		{
			yield return new WaitForSeconds(delay);
			DistanceBasedCheck();
			ActivateCard();
		}

		private void DistanceBasedCheck()
		{
			List<TimeCard> cards = _cards.Where(x => !x.done).ToList();
			for (int i = 0; i < cards.Count; i++)
			{
				float distance = Vector3.Distance(
					Camera.main.WorldToScreenPoint(cards[i].transform.position),
					Camera.main.WorldToScreenPoint(cards[i].target.position));

				if (distance > allowedInaccuracy) cards[i].MoveBack(cardSlots[i]);
				else
				{
					cards[i].done = true;
					cards[i].Move();
				}
			}
		}
	}
}