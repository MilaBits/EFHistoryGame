using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	[SerializeField]
	private bool autoSwitch = default;

	[SerializeField]
	private string scene = default;

	public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

	public void SwitchScene(string sceneName) => SceneManager.LoadScene(sceneName);

	public void SwitchScene()
	{
		SceneManager.LoadScene(scene);
	}

	private void OnEnable()
	{
		if (autoSwitch) SwitchScene();
	}
}