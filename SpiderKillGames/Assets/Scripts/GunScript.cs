using System.Collections;
using UnityEngine;

public class GunScript : MonoBehaviour {

	// Use this for initialization
	
	public GameObject controllerRight;
	private SteamVR_TrackedObject obj;
	private SteamVR_Controller.Device device;
	private SteamVR_TrackedController controller;
	public Transform muzzleTransform;
	public float damage = 25f;
	public float range = 100f;
	public float fireRate = 5f;
	public ParticleSystem muzzleflash;
	public GameObject impactEffect;
	public float impactForce = 60f;
	private float nextTimeToFire = 0f;

	void Start() {
		controller = controllerRight.GetComponent<SteamVR_TrackedController>();	
		obj = controllerRight.GetComponent<SteamVR_TrackedObject>();
	}

	private void Update() {
		if (controller.triggerPressed && Time.time >= nextTimeToFire) {
			nextTimeToFire = Time.time + 1f/fireRate + 0.05f;
			Shoot();
		}
	}

	void Shoot() {
		muzzleflash.Play();
		RaycastHit hit;
		device = SteamVR_Controller.Input((int) obj.index);
		device.TriggerHapticPulse(3000);

		if (Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, range)) {
			ControlSpider control = hit.transform.GetComponent<ControlSpider>();
			if (control != null) {
				control.TakeDamage(damage);
			}

			if (hit.rigidbody != null) {
				hit.rigidbody.AddForce(-hit.normal * impactForce);
			}
			if (Vector3.Distance(hit.point, gameObject.transform.position) > 1f) {
				GameObject obj = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
				Destroy(obj, 1f);
			}
		}
	}
}
