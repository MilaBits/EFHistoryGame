using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	[SerializeField]
	private bool autoSwitch = default;

	[SerializeField]
	private string scene = default;

	[SerializeField]
	private float delay = 3f;

	public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	public void SwitchScene(string sceneName) => SceneManager.LoadScene(sceneName);
	public void SwitchScene() => SceneManager.LoadScene(scene);
	public void SwitchSceneWithDelay(string sceneName) => StartCoroutine(SwitchSceneDelayed(sceneName, delay));


	private IEnumerator SwitchSceneDelayed(string sceneName, float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneName);
	}

	private void OnEnable()
	{
		if (autoSwitch) SwitchScene();
	}
}