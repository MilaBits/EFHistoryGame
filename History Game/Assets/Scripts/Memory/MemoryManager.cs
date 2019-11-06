using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Memory
{
	public class MemoryManager : MonoBehaviour
	{
		private MemoryGamePreset _gamePreset = default;

		[SerializeField]
		private List<Match> matches = new List<Match>();

		[SerializeField]
		private MemoryCard cardPrefab = default;

		[SerializeField]
		private RectTransform cardContainer = default;

		private List<MemoryCard> _activeCards = new List<MemoryCard>();

		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		private int _doneCount;

		private void Start()
		{
			_gamePreset = FindObjectOfType<PresetHolder>().gamePreset.memoryPreset;
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
						activeCard.Hide(true, .5f);
					}
				}

				_activeCards.Clear();
			}

			if (_doneCount >= matches.Count * 2) gameDone.Invoke();
		}

		private void ShuffleCards(int iterations)
		{
			for (int i = 0; i < iterations; i++)
			{
				int childCount = cardContainer.childCount;
				cardContainer.GetChild(Random.Range(0, childCount))
							 .SetSiblingIndex(Random.Range(0, childCount));
			}
		}

		private void LoadMatches()
		{
			foreach (MatchData matchData in _gamePreset.matches)
			{
				MemoryCard cardA = Instantiate(cardPrefab, cardContainer);
				MemoryCard cardB = Instantiate(cardPrefab, cardContainer);
				cardA.Init(matchData.cardDataA);
				cardB.Init(matchData.cardDataB);

				Match match = new Match {cardA = cardA, cardB = cardB};

				cardA.manager = cardB.manager = this;
				cardA.match   = cardB.match   = match;

				matches.Add(match);
			}
		}
	}
}