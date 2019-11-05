using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timeline
{
	[CreateAssetMenu(fileName = "New Timeline Preset", menuName = "History Game/Timeline Game Preset", order = 0)]
	[Serializable]
	public class TimelineGamePreset : ScriptableObject
	{
		public List<CardData> cards = new List<CardData>();

		public List<TimeRange> timeRanges = new List<TimeRange>();
	}
}