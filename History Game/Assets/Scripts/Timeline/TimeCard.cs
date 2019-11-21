using System;
using System.Collections;
using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.Events;

namespace Timeline
{
	[Serializable]
	public class TimeCard : Card
	{
		public Transform target;
		public bool ready = true;

		[SerializeField]
		private Color highlightColor;

		public void Move() =>
			StartCoroutine(MoveTransform(transform.position, target.transform.position, .5f,
										 new Vector3(0.2f, 0.2f, 1)));

		public void Move(Vector3 position) => StartCoroutine(MoveTransform(transform.position, position, .5f,
																		   new Vector3(0.2f, 0.2f, 1)));

		private IEnumerator FadeColor(Color start, Color end, float time)
		{
			for (float passedTime = 0; passedTime < time; passedTime += Time.deltaTime)
			{
				image.color = Color.Lerp(start, end, passedTime);
				yield return null;
			}

			image.color = end;
		}

		private IEnumerator MoveTransform(Vector3 startPos, Vector3 endPos, float time, Vector3 targetScale)
		{
			Vector3 startScale = transform.localScale;
			transform.SetParent(GetComponentInParent<Canvas>().transform);

			for (float passedTime = 0; passedTime < time; passedTime += Time.deltaTime)
			{
				float progress = passedTime / time;
				transform.position   = Vector3.Lerp(startPos, endPos, progress);
				transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
				yield return null;
			}

			transform.position   = endPos;
			transform.localScale = targetScale;
		}

		public void Deactivate()
		{
			Glow(false);
		}

		public void Activate()
		{
			Glow(true);
		}
	}
}