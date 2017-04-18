using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwarmNest : MonoBehaviour {

	private Blackboard blackboard;

	public bool active = false;
	public bool swarm = false;
	private bool swarming = false;

	public int numToSwarm = 10;
	private int numberCurrentlySwarming = 0;

	public GameObject enemyPrefab;

	public int totalNumEnemies = 50;

	private List<GameObject> enemies = new List<GameObject>();
	private List<FlyingEnemy> enemyScripts = new List<FlyingEnemy>();



	// Use this for initialization
	void Start ()
	{

		blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();

		for (int i = 0; i < totalNumEnemies; i++) {
			GameObject enemy = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
			enemies.Add(enemy);
			FlyingEnemy enemyScript = enemy.GetComponent<FlyingEnemy>();
			enemyScripts.Add(enemyScript);
			enemy.SetActive(false);
		}

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (active) {

			if (swarm) {
				if (!swarming) {
					swarming = true;
					StartCoroutine(ActivateSwarmer());
				}
			}

		}

	}

	IEnumerator ActivateSwarmer ()
	{
		yield return new WaitForSeconds (0.3f);
		if (numberCurrentlySwarming < numToSwarm) {
			enemies [numberCurrentlySwarming].SetActive (true);
			enemyScripts [numberCurrentlySwarming].target = blackboard.skySeekPoint;
			numberCurrentlySwarming++;
			StartCoroutine (ActivateSwarmer ());
		} else {
			swarm = false;
			swarming = false;
		}
	}

}
