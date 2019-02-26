using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	// Use this for initialization
    private GameObject player;
    public GameObject timeText;
    public GameObject healthText;
    public GameObject scoreText;
    public string restartText;
    public string gameOverText;
    private float health = 100f;
    private bool gameOver;
    private bool restart;
    private int score;
    public GameObject prefab;
	private int nspiders = HoldData.numberSelected;
    private ArrayList spiders = new ArrayList();

	private float nextTimeToCreate = 0.5f;
    private float startTime = 0f;
    private float endTime = 0f;
    private bool isRandom;

    void Start ()
    {
        isRandom = false;
        player = GameObject.FindGameObjectWithTag ("MainCamera");
        gameOver = false;
        restart = false;
        restartText = "";
        gameOverText = "";
        score = 0;
        startTime = Time.time;
        if (HoldData.numberSelected == 99)
            isRandom = true;
    }

    void Update ()
    {
        if (gameOver && Time.time - endTime > 50f)
            SceneManager.LoadScene("Menu");
        
        if (gameOver)
        return;

        float t = Time.time - startTime;
        if (health <= 0f || t >= 90f) {
            GameOver();
        }

        if (restart)
        {
            SceneManager.LoadScene("MainScene");
        }

        if (Time.time >= nextTimeToCreate && !gameOver) {
            if (isRandom) {
                create(new System.Random().Next(4, 7));
                nextTimeToCreate = Time.time + 1f/0.4f;
            }
            else 
            {
                create(nspiders);
                nextTimeToCreate = Time.time + 1f/0.4f;
            }
        }
        
        timeText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Time : " + ((int) t / 60).ToString() + "' " + ((int) t % 60).ToString() + "'' ");
        healthText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Health : " + health);
        scoreText.GetComponent<TMPro.TextMeshProUGUI>().SetText("Score : " + score);
        
    }

    void create(int number) {
		float x = transform.position.x; 
		float z = transform.position.z;
		float minx = x + 6;
		float maxx = x + 12;
		float minz = z - 20;
		float maxz = z + 3;
        for (int i = 0; i < number; i++) {
			spiders.Add(Instantiate(prefab, new Vector3(Random.Range(minx, maxx), 1f, Random.Range(minz, maxz)), Quaternion.identity));
		}
	}

	public void AddScore (int newScoreValue)
    {
        score += newScoreValue;
    }

    public void GameOver ()
    {
        endTime = Time.time;
        gameOverText = "Game Over!";
        gameOver = true;
        foreach(GameObject spider in spiders) {
            Destroy(spider);
        }
    }

    public void TakeDamage(float amount) {
        health -= amount;
    }

    public void setnspiders(int number) {
        nspiders = number;
        Debug.Log(number);
    }

}
