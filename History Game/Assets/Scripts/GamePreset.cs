using Memory;
using Timeline;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Preset", menuName = "Game Preset", order = 0)]
public class GamePreset : ScriptableObject
{
	public TimelineGamePreset TimelinePreset;
	public MemoryGamePreset MemoryPreset;
}