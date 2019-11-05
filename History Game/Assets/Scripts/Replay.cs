using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Replay : MonoBehaviour, I_SmartwallInteractable
{
	[SerializeField]
	private string replayScene = default;

	public void Hit(Vector3 hitPosition)
	{
		SceneManager.LoadScene(replayScene);
	}
}