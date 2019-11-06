using System;
using System.Collections;
using UnityEngine;

namespace Timeline
{
	[Serializable]
	public class TimeCard : Card
	{
		public TimelineHitZone hitZone;

		[SerializeField]
		private Color highlightColor;

		public void Move() => StartCoroutine(MoveTransform(transform.position, hitZone.transform.position, .5f));

		private IEnumerator FadeColor(Color start, Color end, float time)
		{
			for (float passedTime = 0; passedTime < time; passedTime += Time.deltaTime)
			{
				image.color = Color.Lerp(start, end, passedTime);
				yield return null;
			}

			image.color = end;
		}

		private IEnumerator MoveTransform(Vector3 startPos, Vector3 endPos, float time)
		{
			Vector3 startScale  = transform.localScale;
			Vector3 targetScale = new Vector3(.2f, .2f, 1);
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

		public void Activate()
		{
			Glow(true);
			hitZone.gameObject.SetActive(true);
		}
	}
}