using System;
using System.Collections;
using SmartwallPackage.SmartwallInput;
using UnityEngine;

namespace Memory
{
	[Serializable]
	public class MemoryCard : Card, I_SmartwallInteractable
	{
		public Match match;
		public bool interactable = true;

		public MemoryManager manager;
		private static readonly int FinishTrigger = Animator.StringToHash("Finish");

		private void Awake()
		{
			Hide(false, 0f);
			interactable = true;
		}

		public void Reveal(bool animate)
		{
			if (animate) StartCoroutine(Flip(true, .5f));
			else ToggleView(true);
		}

		public void Hide(bool animate, float delay)
		{
			if (animate) StartCoroutine(DelayedFlip(false, .5f, .75f));
			else ToggleView(false);
		}

		private IEnumerator DelayedFlip(bool show, float duration, float delay)
		{
			interactable = false;
			yield return new WaitForSeconds(delay);
			StartCoroutine(Flip(show, duration));
		}

		private IEnumerator Flip(bool show, float duration)
		{
			bool flipped = false;
			interactable = false;

			Vector3 start = transform.localScale;
			Vector3 end   = new Vector3(-start.x, 1, 1);

			for (float passedTime = 0; passedTime < duration; passedTime += Time.deltaTime)
			{
				float progress = passedTime / duration;

				if (!flipped && progress > 0.5)
				{
					flipped = true;
					Vector3 temp = end;
					end   = start;
					start = temp;
					ToggleView(show);
				}

				transform.localScale = Vector3.Lerp(start, end, progress);

				yield return null;
			}

			ToggleView(show);
			if (!show) interactable = true;
			transform.localScale = end;
		}

		private void ToggleView(bool show)
		{
			switch (textOrImage)
			{
				case TextOrImage.Text:
					text.gameObject.SetActive(show);
					break;
				case TextOrImage.Image:
					image.gameObject.SetActive(show);
					break;
				case TextOrImage.Both:
					text.gameObject.SetActive(show);
					image.gameObject.SetActive(show);
					break;
			}
		}

		public void Hit(Vector3 hitPosition)
		{
			if (!interactable) return;

			Reveal(true);
			manager.CheckCards(this);
		}

		public void Finish()
		{
			interactable = false;
			done         = true;

			Glow(false);
			animator.SetTrigger(FinishTrigger);
		}
	}
}