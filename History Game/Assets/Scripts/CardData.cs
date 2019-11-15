using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct CardData
{
	public TextOrImage textOrImage;
	[CanBeNull]
	public string name;
	[FormerlySerializedAs("year")]
	public int value;
	public Sprite sprite;
}