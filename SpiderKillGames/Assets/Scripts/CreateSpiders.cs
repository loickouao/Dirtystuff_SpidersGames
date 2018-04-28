using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateSpiders : MonoBehaviour {

	public GameObject prefab;
	public int nspiders;
	public GameObject prefab2;
	public int nspiders2;
	public GameObject prefab3;
	public int nspiders3;

	// Use this for initialization
	void Start () {
		float x = transform.position.x; 
		float z = transform.position.z;
		//List<float> pos1 = new List<float>();
		//pos1.Add(23f); pos1.Add(1.5f); pos1.Add(23f);
		//38 --22 
		//18-9
		float minx = x + 6;
		float maxx = x + 12;
		float minz = z - 20;
		float maxz = z + 3;
		for (int i = 0; i < nspiders; i++) {
			Instantiate(prefab, new Vector3(Random.Range(minx, maxx), 1f, Random.Range(minz, maxz)), Quaternion.identity);
			//Instantiate(prefab, new Vector3(pos1[0], pos1[1], pos1[2]), Quaternion.identity);
		}
		for (int i = 0; i < nspiders2; i++) {
			Instantiate(prefab2, new Vector3(Random.Range(minx, maxx), 1f, Random.Range(minz, maxz)), Quaternion.identity);
		}
		for (int i = 0; i < nspiders3; i++) {
			Instantiate(prefab3, new Vector3(Random.Range(minx, maxx), 1f, Random.Range(minz, maxz)), Quaternion.identity);
		}	}

	void Update() {
	}		
}
