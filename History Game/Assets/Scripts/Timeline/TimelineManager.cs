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
		private int _score;
		private bool _interactable = true;
		private int _turns;
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
		[SerializeField]
		private TextMeshProUGUI scoreText = default;
		[Header("Prefab References")]
		[SerializeField]
		private TimeCard cardPrefab = default;
		[Space]
		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		public void NextCard(bool tryCheck)
		{
			Debug.Log("Going to next card");
			if (_activeCard) _activeCard.Deactivate();

			List<TimeCard> availableCards = _cards.Where(x => !x.done).ToList();
			if (availableCards.Count < 1 || _turns >= 3)
			{
				gameDone.Invoke();
				return;
			}

			bool anyReady = _cards.Any(x => x.ready);
			if (anyReady && cardSlots[1].childCount > 0) StartCoroutine(SlideCards());
			else if (tryCheck) StartCoroutine(CheckCards(.75f));
		}

		private IEnumerator SlideCards()
		{
			_interactable = false;
			for (int i = 1; i < cardSlots.Count; i++)
				if (cardSlots[i].childCount > 0 && cardSlots[0].childCount == 0)
					StartCoroutine(
						SlideCard((RectTransform) cardSlots[i].GetChild(0).transform, cardSlots[i - 1], .15f));
			yield return new WaitForSeconds(.15f);
			ActivateFirstCard();
		}

		private bool ActivateFirstCard()
		{
			Debug.Log("activating card");
			Transform firstFilledSlot = cardSlots.FirstOrDefault(x => x.childCount != 0);
			if (firstFilledSlot)
			{
				_activeCard = firstFilledSlot.GetChild(0).GetComponent<TimeCard>();
				_activeCard.Activate();
				_interactable = true;
				return true;
			}

			return false;
		}

		private IEnumerator SlideCard(RectTransform card, RectTransform target, float duration)
		{
			Vector3 startPos = card.transform.position;

			for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
			{
				card.transform.position =
					Vector3.LerpUnclamped(startPos, target.transform.position,
										  slideAnimation.Evaluate(elapsed / duration));
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
				NextCard(true);
			}
		}

		private void Start()
		{
			_gamePreset = FindObjectOfType<PresetHolder>().gamePreset.timelinePreset;
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
				TimeCard card = Instantiate(cardPrefab, cardSlots[i]);
				card.Init(cardData);
				_cards.Add(card);
			}
		}

		private IEnumerator CheckCards(float delay)
		{
			yield return new WaitForSeconds(delay);
			DistanceBasedCheck();
		}

		private void DistanceBasedCheck()
		{
			Debug.Log("Checking");
			List<TimeCard> returnBuffer = new List<TimeCard>();
			List<TimeCard> cards = _cards.Where(x => !x.done).ToList();
			for (int i = 0; i < cards.Count; i++)
			{
				float distance = Vector3.Distance(
					Camera.main.WorldToScreenPoint(cards[i].transform.position),
					Camera.main.WorldToScreenPoint(cards[i].target.position));

				if (distance > allowedInaccuracy)
				{
					cards[i].wrongCount++;
					returnBuffer.Add(cards[i]);
				}
				else
				{
					UpdateScore(_gamePreset.wrongCount - cards[i].wrongCount);
					cards[i].done = true;
					cards[i].Move();
				}
			}

			StartCoroutine(ReturnCards(returnBuffer));
		}

		private IEnumerator ReturnCards(List<TimeCard> cards)
		{
			_turns++;
			for (var i = 0; i < cards.Count; i++) yield return StartCoroutine(ReturnCard(cards[i], .1f, cardSlots[i]));
			if (!ActivateFirstCard()) NextCard(false);
		}

		private IEnumerator ReturnCard(TimeCard card, float waitTime, RectTransform target)
		{
			yield return new WaitForSeconds(waitTime);
			float value = (float) card.wrongCount / _gamePreset.wrongCount;
			card.SetFill(value, Color.Lerp(Color.yellow, Color.red, value));
			card.MoveBack(target);
		}

		private void UpdateScore(int value)
		{
			_score += value;
			scoreText.text = _score.ToString();
		}
	}
}