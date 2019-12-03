using System;
using System.Collections;
using SmartwallPackage.SmartwallInput;
using UnityEngine;
using Random = System.Random;

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
			StartCoroutine(DelayedFlip(true, .5f, UnityEngine.Random.Range(.1f, 1f)));
			interactable = true;
		}

		public void Reveal(bool animate)
		{
//			if (animate) StartCoroutine(Flip(true, .5f));
//			else ToggleView(true);

			Select(true);
		}

		public void Hide(bool animate, float delay)
		{
//			if (animate) StartCoroutine(DelayedFlip(false, .5f, .75f));
//			else ToggleView(false);

			Select(false);
		}

		private IEnumerator DelayedFlip(bool show, float duration, float delay)
		{
			interactable = false;
			yield return new WaitForSeconds(delay);
			StartCoroutine(Flip(show, duration));
		}

		private void Select(bool select)
		{
			Glow(select);
			interactable = !select;
		}

		private IEnumerator Flip(bool show, float duration)
		{
			bool halfwayDone = false;
			interactable = false;

			Vector3 start = transform.localScale;
			Vector3 end   = new Vector3(-start.x, 1, 1);

			for (float passedTime = 0; passedTime < duration; passedTime += Time.deltaTime)
			{
				float progress = passedTime / duration;

				if (!halfwayDone && progress > 0.5)
				{
					halfwayDone = true;
					Vector3 temp = end;
					end   = start;
					start = temp;
					ToggleView(show);
				}

				transform.localScale = Vector3.Lerp(start, end, progress);

				yield return null;
			}

			ToggleView(show);
			interactable         = true;
			transform.localScale = end;
		}

		private void ToggleView(bool show)
		{
			switch (textOrImage)
			{
				case TextOrImage.Image:
					image.gameObject.SetActive(show);
					break;
				case TextOrImage.Name:
				case TextOrImage.Value:
					image.gameObject.SetActive(show);
					text.gameObject.SetActive(show);
					text.transform.parent.gameObject.SetActive(show);
					break;
				case TextOrImage.NameAndImage:
				case TextOrImage.ValueAndImage:
					text.gameObject.SetActive(show);
					text.transform.parent.gameObject.SetActive(show);
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