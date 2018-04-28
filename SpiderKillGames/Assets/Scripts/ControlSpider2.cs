using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSpider2 : MonoBehaviour {

	private GameObject playerG;
	private float playerDistance;
	public float rotationDamping;
	public float moveSpeed;
	private Animation anim;
	int nb = 0;
	public static bool isPlayerAlive = true;

	// Use this for initialization
	void Start () {
		anim = transform.GetComponent<Animation> ();
		playerG = GameObject.Find("FirstPersonCharacter");
	}

	// Update is called once per frame
	void Update () {
		if (isPlayerAlive) {
			playerDistance = Vector3.Distance(playerG.transform.position, transform.position);
			if (playerDistance < 20f) {

				lookAtPlayer();
			}
			if (playerDistance < 15f) {
				if (playerDistance > 2.5f) {
					chase ();
				} else {
					attack();
				}
			}
		}	
	}
	void lookAtPlayer(){
		Quaternion rotation = Quaternion.LookRotation (playerG.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotationDamping);
	}

	void chase(){
		anim.clip = anim.GetClip ("run");
		anim.Play ();
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}

	void attack(){
		anim.clip = anim.GetClip ("attack2");
		anim.Play ();
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit)) {
			if (hit.collider.gameObject.tag == "Player") {
				nb = nb + 1;
				if (nb == 15) {
					hit.collider.gameObject.GetComponent<health_script> ().health -= 2f;
					print(hit.collider.gameObject.GetComponent<health_script> ().health);
					nb=0;
				}
			}
		}
	}
}
