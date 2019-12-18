using System;
using System.Collections.Generic;
using System.Linq;
using SmartwallPackage.GlobalGameSettings;
using UnityEngine;

public class PresetHolder : MonoBehaviour
{
	public GamePreset gamePreset;

	[SerializeField]
	private List<ThemeString> themeStrings = default;

	private void Awake()
	{
		GetChosenPreset();
	}

	private void GetChosenPreset()
	{
		string theme = GlobalGameSettings.GetSetting("Theme");

		GamePreset chosenPreset = themeStrings.First(x => x.name.Equals(theme)).preset;

		if (chosenPreset != null) gamePreset = chosenPreset;
	}
}

[Serializable]
public struct ThemeString
{
	public string name;
	public GamePreset preset;
}