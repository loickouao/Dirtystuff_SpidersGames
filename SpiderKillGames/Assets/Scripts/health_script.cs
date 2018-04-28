using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health_script : MonoBehaviour {
	public float health;
	private GameObject playerG;
	// Use this for initialization
	void Start () {
		playerG = GameObject.Find("FPSController");
	}
	
	// Update is called once per frame
	void Update () {
		if (health < 0f) {
			//Destroy (gameObject);
			print("Game Over");
			ControlSpider.isPlayerAlive = false;
			ControlSpider2.isPlayerAlive = false;
		}
	}
}
