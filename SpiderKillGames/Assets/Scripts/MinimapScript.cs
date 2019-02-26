using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour {

	// Use this for initialization
	public Transform player;
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPos = player.position;
		newPos.y = transform.position.y;
		transform.position = newPos;
	}
}
