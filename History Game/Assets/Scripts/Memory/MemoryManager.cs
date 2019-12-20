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

		[Header("Scene References")]
		[SerializeField]
		private TextMeshProUGUI descriptionText = default;

		[SerializeField]
		private RectTransform imageCardContainer = default;
		[SerializeField]
		private RectTransform ValueCardContainer = default;

		[Header("Prefab References")]
		[SerializeField]
		private MemoryCard cardPrefab = default;

		[Space]
		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();


		private void Start()
		{
			_gamePreset = FindObjectOfType<PresetHolder>().gamePreset.memoryPreset;
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
						activeCard.Finish();
						_doneCount++;
					}
				}
				else
				{
					foreach (MemoryCard activeCard in _activeCards)
					{
						activeCard.Hide();
					}
				}

				_activeCards.Clear();
			}

			if (_doneCount >= _matches.Count * 2) gameDone.Invoke();
		}

		private void ShuffleCards(int iterations)
		{
			for (int i = 0; i < iterations; i++)
			{
				int childCount = ValueCardContainer.childCount;
				ValueCardContainer.GetChild(Random.Range(0, childCount))
								  .SetSiblingIndex(Random.Range(0, childCount));
				imageCardContainer.GetChild(Random.Range(0, childCount))
								  .SetSiblingIndex(Random.Range(0, childCount));
			}

			List<MemoryCard> cards = new List<MemoryCard>();
			for (int i = 0; i < _matches.Count; i++)
			{
				cards.Add(ValueCardContainer.GetChild(i).GetComponent<MemoryCard>());
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
				MemoryCard cardB = Instantiate(cardPrefab, ValueCardContainer);
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