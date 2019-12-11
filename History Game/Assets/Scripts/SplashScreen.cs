using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
	[SerializeField]
	private string nextScene;
	[Space]
	[SerializeField]
	private float splashDuration = 5f;
	[SerializeField]
	private float fadeDuration = 1f;
	[Space]
	[SerializeField]
	private Image targetImage;

	private void Start()
	{
		StartCoroutine(PlaySplash());
	}

	private IEnumerator PlaySplash()
	{
		float stayTime = splashDuration - fadeDuration * 2;

		yield return StartCoroutine(Fade(0, 1, fadeDuration));
		yield return new WaitForSeconds(stayTime);
		yield return StartCoroutine(Fade(1, 0, fadeDuration));
		SceneManager.LoadScene(nextScene);
	}

	private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
	{
		Color start = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, startAlpha);
		Color end   = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, endAlpha);

		for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
		{
			targetImage.color = Color.Lerp(start, end, elapsed / duration);
			yield return null;
		}

		targetImage.color = end;
	}
}