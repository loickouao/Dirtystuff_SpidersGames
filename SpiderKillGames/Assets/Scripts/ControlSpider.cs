using UnityEngine;

public class ControlSpider : MonoBehaviour {

	private GameObject playerG;
	private GameObject manager;
	private float rotationDamping = 3;
	private float moveSpeed = 2f;
	private Animation anim;
	private float health = 20f;

    // Use this for initialization
    void Start () {
		anim = transform.GetComponent<Animation> ();
		playerG = GameObject.FindGameObjectWithTag ("MainCamera");
		manager = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
		if (playerG == null)
			return;
		float playerDistance = Vector3.Distance(playerG.transform.position, transform.position);
		if (playerDistance < 100f) {
			lookAtPlayer();
		}
		if (playerDistance < 99f) {
			if (playerDistance > 1.5f) {
				chase ();
			} else {
				attack();
			}
		}
	}

	void lookAtPlayer(){
		Quaternion rotation = Quaternion.LookRotation (playerG.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * rotationDamping);
	}
	void chase(){
		anim.clip = anim.GetClip ("Run");
		anim.Play ();
		transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}
	void attack() {
		health -= 20f;
		PlayerController control = manager.GetComponent<PlayerController>();
		if (control != null) {
			control.TakeDamage(10f);
		}
		if (health <= 0f) {
			Die();
		}
	}
	public void TakeDamage(float amount) {
		health -= amount;
		PlayerController control = manager.GetComponent<PlayerController>();
		if (control != null) {
			control.AddScore (5);
		}
		if (health <= 0f) {
			Die();
		}
	}
	void Die() {
		Destroy(gameObject);
	}

	public void UpdateSpeed(float newValue) {
		moveSpeed = newValue;
	}

	public float getSpeed() {
		return moveSpeed;
	}

}
