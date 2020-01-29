using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Card : MonoBehaviour
{
	public TextOrImage textOrImage;
	public new string name;
	public Sprite sprite;
	public int year;
	public int wrongCount;

	[SerializeField]
	protected Animator animator = default;
	private static readonly int Pulsate = Animator.StringToHash("Pulsate");

	[SerializeField, Space]
	protected Image image;
	[SerializeField]
	protected TextMeshProUGUI text;
	[Space]
	[SerializeField]
	protected AudioSource audioSource;

	public bool done;

	public void Glow(bool glow) => animator.SetBool(Pulsate, glow);

	public void PlaySound(AudioClip clip) => audioSource.PlayOneShot(clip);

	public void Init(CardData data)
	{
		textOrImage = data.textOrImage;
		sprite      = data.sprite;
		name        = data.name;
		year        = data.value;

		switch (textOrImage)
		{
			case TextOrImage.Name:
				text.text = name;
				break;
			case TextOrImage.Value:
				text.text = year.ToString();
				break;
			case TextOrImage.Image:
				image.sprite = sprite;
				break;
			case TextOrImage.NameAndImage:
				text.text    = name;
				image.sprite = sprite;
				break;
			case TextOrImage.ValueAndImage:
				text.text    = year.ToString();
				image.sprite = sprite;
				break;
		}
	}
}