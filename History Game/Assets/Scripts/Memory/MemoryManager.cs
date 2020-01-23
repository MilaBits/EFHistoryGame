using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Memory
{
	public class MemoryManager : MonoBehaviour
	{
		private MemoryGamePreset _gamePreset = default;
		private List<MemoryCard> _activeCards = new List<MemoryCard>();
		private List<Match> _matches = new List<Match>();
		private int _doneCount;
		private int _score = 0;

		[Header("Scene References")]
		[SerializeField]
		private TextMeshProUGUI descriptionText = default;

		[SerializeField]
		private RectTransform imageCardContainer = default;
		[SerializeField]
		private RectTransform valueCardContainer = default;
		[SerializeField]
		private TextMeshProUGUI scoreText = default;

		[Header("Prefab References")]
		[SerializeField]
		private MemoryCard cardPrefab = default;

		[Space]
		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();


		private void Start()
		{
			_gamePreset          = FindObjectOfType<PresetHolder>().gamePreset.memoryPreset;
			descriptionText.text = _gamePreset.description;
			LoadMatches();
			ShuffleCards(12);
		}

		public void CheckCards(MemoryCard card)
		{
			_activeCards.Add(card);
			card.Glow(true);
			if (_activeCards.Count > 1)
			{
				if (_activeCards[0].match.Equals(_activeCards[1].match))
				{
					foreach (MemoryCard activeCard in _activeCards)
					{
						activeCard.Finish(true);
						UpdateScore(_gamePreset.wrongCount - activeCard.wrongCount);
						_doneCount++;
					}
				}
				else
				{
					foreach (MemoryCard activeCard in _activeCards)
					{
						activeCard.wrongCount++;
						activeCard.Select(false);

						if (activeCard.wrongCount >= _gamePreset.wrongCount)
						{
							activeCard.match.cardA.Finish(false);
							activeCard.match.cardB.Finish(false);
							_doneCount += 2;
						}
					}
				}

				_activeCards.Clear();
			}

			if (_doneCount >= _matches.Count * 2) gameDone.Invoke();
		}

		private void UpdateScore(int value)
		{
			_score          += value;
			scoreText.text =  _score.ToString();
		}

		private void ShuffleCards(int iterations)
		{
			for (int i = 0; i < iterations; i++)
			{
				int childCount = valueCardContainer.childCount;
				valueCardContainer.GetChild(Random.Range(0, childCount))
								  .SetSiblingIndex(Random.Range(0, childCount));
				imageCardContainer.GetChild(Random.Range(0, childCount))
								  .SetSiblingIndex(Random.Range(0, childCount));
			}

			List<MemoryCard> cards = new List<MemoryCard>();
			for (int i = 0; i < _matches.Count; i++)
			{
				cards.Add(valueCardContainer.GetChild(i).GetComponent<MemoryCard>());
				cards.Add(imageCardContainer.GetChild(i).GetComponent<MemoryCard>());
			}

			cards = cards.OrderByDescending(x => (int) x.textOrImage).ToList();
			for (int i = 0; i < cards.Count; i++)
			{
				cards[i].transform.SetSiblingIndex(i);
			}
		}

		private void LoadMatches()
		{
			foreach (MatchData matchData in _gamePreset.GetMatches())
			{
				MemoryCard cardA = Instantiate(cardPrefab, imageCardContainer);
				MemoryCard cardB = Instantiate(cardPrefab, valueCardContainer);
				cardA.Init(matchData.cardDataA);
				cardB.Init(matchData.cardDataB);

				Match match = new Match {cardA = cardA, cardB = cardB};

				cardA.manager = cardB.manager = this;
				cardA.match   = cardB.match   = match;

				_matches.Add(match);
			}
		}
	}
}