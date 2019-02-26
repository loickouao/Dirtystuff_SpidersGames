using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	public void PlayGame() {
		SceneManager.LoadScene("Choice Scene");
	}

	public void QuitGame() {
		Application.Quit();
	}

}
