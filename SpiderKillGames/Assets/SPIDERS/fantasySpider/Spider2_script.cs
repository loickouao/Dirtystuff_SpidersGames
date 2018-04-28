using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider2_script : MonoBehaviour {
	public Transform player;
	private float playerDistance;
	public float rotationDamping;
	public float moveSpeed;
	private Animation anim;
	public static bool isPlayerAlive = true;
	int nb = 0;
	// Use this for initialization
	void Start () {
		anim = transform.GetComponent<Animation> ();
	}

	// Update is called once per frame
	void Update () {
		if (isPlayerAlive) {
			playerDistance = Vector3.Distance (player.position, transform.position);
			if (playerDistance < 15f) {
				lookAtPlayer ();
			}
			if (playerDistance < 12f) {
				if (playerDistance > 2f) {
					chase ();
				} else if (playerDistance < 2f) {
					attack ();
				}
			}
		}
	}
	void lookAtPlayer(){
		Quaternion rotation = Quaternion.LookRotation (player.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotationDamping);
	}
	void chase(){
		anim.clip = anim.GetClip ("run");
		anim.Play ();
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}
	void attack(){
		anim.clip = anim.GetClip ("attack1");
		anim.Play ();
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			if (hit.collider.gameObject.tag == "Player") {
				nb = nb + 1;
				print (nb);
				if (nb == 100) {
					hit.collider.gameObject.GetComponent<health_script> ().health -= 5f;
					nb = 0;
				}
			}
		} 	
	}
}