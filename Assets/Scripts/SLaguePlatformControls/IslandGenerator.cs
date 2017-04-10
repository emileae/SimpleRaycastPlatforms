using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandGenerator : MonoBehaviour {

	public GameObject[] platformPrefabs;
	public GameObject ladderPrefab;

	// character prefabs
	public GameObject player;
	public GameObject npc;
	private Bounds npcBounds;
	public GameObject enemy;
	private Bounds enemyBounds;

	public float[] islandHeights;
	public int maxPlatformsPerIsland = 5;
	public int numberOfIslands;
	public float distanceBetweenIslands;

	// generate types of npcs
	public int numAverageJoes = 10;
	public int numBuilders = 3;
	public int numFighters = 5;

	// keep track of all the platforms
	private List<GameObject> platforms = new List<GameObject>();
	public List<Bounds> platformBoundsList = new List<Bounds>();

	// Use this for initialization
	void Start ()
	{

		float currentXPos = numberOfIslands * distanceBetweenIslands * 0.5f * -1;
		int currentPlatformIndex = 0;

		// multiple platforms per island
		for (int i = 0; i < numberOfIslands; i++) {
			int numPlatforms = Random.Range (1, maxPlatformsPerIsland + 1);// integer Random.Range is exclusive of max value... so add 1
			for (int j = 0; j < numPlatforms; j++) {
				// choose a random height within a range
				int islandHeightIndex = Random.Range (0, islandHeights.Length);

				// instantiate platform
				GameObject platform = (GameObject)Instantiate (platformPrefabs [0], new Vector3 (currentXPos, islandHeights [islandHeightIndex], 0.1f * j), Quaternion.identity);
				platforms.Add (platform);
				Platform platformScript = platform.transform.GetChild(0).GetComponent<Platform>();// platform script is in the trigger component
				platformScript.maxCoins = Random.Range(1, 12);
				platformScript.ListCoins();
				Bounds platformBounds = platform.GetComponent<EdgeCollider2D> ().bounds;
				// save some getComponent calls
				platformBoundsList.Add (platformBounds);

				// add a ladder to each platform
				GameObject ladder = (GameObject)Instantiate (ladderPrefab, new Vector3 (platformBounds.min.x, islandHeights [islandHeightIndex], 0.1f * j), Quaternion.identity);
				// TODO: can make this more efficient by only calling the GetComponent for the ladder once.... only true if the ladder stays the same for all platforms, i.e. no rope with different bounds, or alternative ladders
				Bounds ladderBounds = ladder.GetComponent<BoxCollider2D> ().bounds;


				// modify ladder position
				// TODO: scaling ladder deforms the sprite, might need to use a quad and set a repeating texture to keep sprite looking right
				if (j > 0) {
					if (platformBoundsList [currentPlatformIndex - 1].max.y > platformBounds.max.y) {
//						Debug.Log ("ladder needs to go up");
						float heightDifference = platformBoundsList [currentPlatformIndex - 1].max.y - platformBounds.max.y;
//						ladder.transform.position = new Vector3 (ladder.transform.position.x + ladderBounds.extents.x, platformBounds.max.y + ladderBounds.extents.y, ladder.transform.position.z);
						ladder.transform.position = new Vector3 (ladder.transform.position.x + ladderBounds.extents.x, platformBounds.max.y + (heightDifference * 0.5f), ladder.transform.position.z);
						float ladderScaleFactor = (heightDifference) / ladderBounds.size.y;
						ladder.transform.localScale += new Vector3 (1, ladderScaleFactor, 1);
					} else if (platformBoundsList [currentPlatformIndex - 1].max.y < platformBounds.max.y) {
//						Debug.Log ("ladder needs to go down");
						float heightDifference = platformBounds.max.y - platformBoundsList [currentPlatformIndex - 1].max.y;
//						ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - ladderBounds.extents.y, ladder.transform.position.z);
						ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - (heightDifference * 0.5f), ladder.transform.position.z);
						float ladderScaleFactor = (heightDifference) / ladderBounds.size.y;
						ladder.transform.localScale += new Vector3 (1, ladderScaleFactor, 1);
					} else if (platformBoundsList [currentPlatformIndex - 1].max.y == platformBounds.max.y) {
//						Debug.Log ("No ladder needed");
					}
				} else {
					// make sure ladder reaches into the sea, so stranded player can get back to platform...
					ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - ladderBounds.extents.y, ladder.transform.position.z);
				}

				currentXPos += platformBounds.size.x;

				// randomly assign an enemy to a platform
				if (Random.Range (0f, 1f) > 0.5f) {
					GameObject enemyObj = Instantiate (enemy, new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
					if (enemyBounds == null) {
						enemyBounds = enemyObj.GetComponent<BoxCollider2D> ().bounds;
					}
					enemyObj.transform.position = new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + enemyBounds.extents.y, platform.transform.position.z);
					// TODO: maybe add enemy to a public list somewhere
				}

				// Assign NPCs to a platform -- offset slightly or make sure NPCs don't spawn overlapping an enemy?

				GameObject npcObj = Instantiate (npc, new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
				if (npcBounds == null) {
					npcBounds = npcObj.GetComponent<BoxCollider2D> ().bounds;
				}
				npcObj.transform.position = new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + npcBounds.extents.y, platform.transform.position.z);
				NPC npcScript = npcObj.GetComponent<NPC> ();
				if (numBuilders > 0) {
					npcScript.npcType = 2;
//					platformScript.builders.Add(npcObj);
					numBuilders -= 1;
				}else if (numFighters > 0){
					npcScript.npcType = 3;
//					platformScript.fighters.Add(npcObj);
					numFighters -= 1;
				}else if (numAverageJoes > 0){
					npcScript.npcType = 1;
//					platformScript.averageJoes.Add(npcObj);
					numAverageJoes -= 1;
				}
				npcScript.SetType();
				// TODO: maybe add npc to a public list somewhere



				currentPlatformIndex++;// update to keep track of where we are in the list
			}

			currentXPos += distanceBetweenIslands;
		}

		// Place player on a random platform, near the middle of the islands
		int randomPlatformIndex = Random.Range(Mathf.FloorToInt(0.25f * currentPlatformIndex), Mathf.FloorToInt(0.75f * currentPlatformIndex));
		Vector3 platformPosition = platforms[randomPlatformIndex].transform.position;
		player.transform.position = new Vector3(platformBoundsList[randomPlatformIndex].center.x, platformBoundsList[randomPlatformIndex].max.y + 20.0f, player.transform.position.z);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
