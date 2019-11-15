using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Card : MonoBehaviour
{
	public TextOrImage textOrImage;
	public new string name;
	public Sprite sprite;
	public int year;

	[SerializeField]
	protected Animator animator = default;
	private static readonly int Pulsate = Animator.StringToHash("Pulsate");

	[SerializeField, Space]
	protected Image image;
	[SerializeField]
	protected TextMeshProUGUI text;
	[Space]
	public bool done;

	public void Glow(bool glow) => animator.SetBool(Pulsate, glow);

	public void Init(CardData data)
	{
		textOrImage = data.textOrImage;
		sprite      = data.sprite;
		name        = data.name;
		year        = data.value;

		text.text    = name;
		image.sprite = sprite;
	}
}