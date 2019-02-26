using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NumberSelect : MonoBehaviour {

	// Use this for initialization
	public void select1() {
		HoldData.numberSelected = 4;
		SceneManager.LoadScene("MainScene");
	}

	public void select2() {
		HoldData.numberSelected = 6;
		SceneManager.LoadScene("MainScene");
	}

	public void select3() {
		HoldData.numberSelected = 7;
		SceneManager.LoadScene("MainScene");
	}

	public void selectRandom() {
		HoldData.numberSelected = 99;
		SceneManager.LoadScene("MainScene");
	}
}
