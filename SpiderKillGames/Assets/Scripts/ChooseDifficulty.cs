using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseDifficulty : MonoBehaviour {

	// Use this for initialization
	// Use this for initialization
    private GameObject player;
    public GameObject prefab;
	ArrayList spiders = new ArrayList();
	private float nextTimeToCreate = 0f;
    private float startTime;
	private bool over = false;
	private int[] nspiders = new int[3];
	private int index;
	public GameObject seqtext;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag ("MainCamera");
        startTime = Time.time;
		index = 0;
		nspiders[0] = 4; nspiders[1] = 6; nspiders[2] = 7;
		startTime = Time.time;
    }

    void Update ()
    {
		if (Time.time - startTime >= 25f)
			over = true;
		if (over)
			SceneManager.LoadScene("Menu");

        if (Time.time >= nextTimeToCreate && !over && index < 3) {
			create(nspiders[index]);
			index++;
			seqtext.GetComponent<TMPro.TextMeshProUGUI>().SetText("ACTIVE SEQUENCE : " + index);
			nextTimeToCreate = 6f + Time.time;
        }
    }

    void create(int number) {
		float x = transform.position.x; 
		float z = transform.position.z;
		float minx = x + 6;
		float maxx = x + 12;
		float minz = z - 20;
		float maxz = z + 3;
		prefab.GetComponent<ControlSpider>().UpdateSpeed(3f);
		for (int i = 0; i < number; i++) {
			spiders.Add(Instantiate(prefab, new Vector3(Random.Range(minx, maxx), 1f, Random.Range(minz, maxz)), Quaternion.identity));
		}
	}

}
