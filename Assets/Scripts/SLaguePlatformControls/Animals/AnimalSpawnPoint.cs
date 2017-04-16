using UnityEngine;
using System.Collections;

public class AnimalSpawnPoint : MonoBehaviour {

	public bool active = true;

	public Platform platformScript;

	public int carryingCapacity = 3;
	private int totalAnimals = 0;

	public bool smallBirdSpawner;
	public bool smallAnimalSpawner;
	public bool largeAnimalSpawner;

	public GameObject smallBirdPrefab;
	public GameObject smallAnimalPrefab;
	public GameObject largeAnimalPrefab;

	public float spawnTime = 2.0f;

	private SpriteRenderer sprite;
	private Bounds spriteBounds;


	void Start ()
	{
		sprite = GetComponent<SpriteRenderer>();
		spriteBounds = sprite.bounds;
		if (active) {
			StartSpawnAnimal ();
		}

	}

	void StartSpawnAnimal(){
		StartCoroutine(SpawnAnimal ());
	}

	IEnumerator SpawnAnimal ()
	{
		yield return new WaitForSeconds (spawnTime);
		if (smallAnimalSpawner) {
			GameObject smallAnimal = Instantiate (smallAnimalPrefab, transform.position, Quaternion.identity) as GameObject;
			AnimalMovement animalScript = smallAnimal.GetComponent<AnimalMovement>();
			animalScript.platformScript = platformScript;
		}
		totalAnimals += 1;
		if (totalAnimals < carryingCapacity) {
			StartSpawnAnimal ();
		}
	}
	

}
