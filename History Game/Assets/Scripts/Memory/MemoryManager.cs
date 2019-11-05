using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Memory
{
	public class MemoryManager : MonoBehaviour
	{
		private MemoryGamePreset gamePreset = default;

		[SerializeField]
		private List<Match> matches = new List<Match>();

		[SerializeField]
		private MemoryCard cardPrefab = default;

		[SerializeField]
		private RectTransform cardContainer = default;

		private List<MemoryCard> _activeCards = new List<MemoryCard>();

		[SerializeField]
		private UnityEvent gameDone = new UnityEvent();

		private int doneCount;

		private void Start()
		{
			gamePreset = FindObjectOfType<PresetHolder>().gamePreset.MemoryPreset;
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
						doneCount++;
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

			if (doneCount >= matches.Count * 2) gameDone.Invoke();
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
			foreach (MatchData matchData in gamePreset.matches)
			{
				MemoryCard cardA = Instantiate(cardPrefab, cardContainer);
				MemoryCard cardB = Instantiate(cardPrefab, cardContainer);
				cardA.Init(matchData.cardDataA);
				cardB.Init(matchData.cardDataB);

				Match match = new Match {CardA = cardA, CardB = cardB};

				cardA.manager = cardB.manager = this;
				cardA.match   = cardB.match   = match;

				matches.Add(match);
			}
		}
	}
}