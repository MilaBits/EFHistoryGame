using System.Collections.Generic;
using UnityEngine;

namespace Memory
{
	[CreateAssetMenu(fileName = "New Memory Preset", menuName = "History Game/Memory Game Preset", order = 0)]
	public class MemoryGamePreset : ScriptableObject
	{
		[SerializeField]
		public List<MatchData> matches;
	}
}