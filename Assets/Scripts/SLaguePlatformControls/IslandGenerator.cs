using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandGenerator : MonoBehaviour {

	public Blackboard blackboard;

	// Sea parameters
	public float seaLevel = 0.0f;

	// structure prefabs
	public int numMedicalStructures = 2;
	public GameObject medicalStructure;
	private Bounds medicalStructureBounds;

	public int numDefenseStructures = 4;
	public GameObject defenseStructure;
	private Bounds defenseStructureBounds;

	// Platform prefabs
	public GameObject[] platformPrefabs;
	public GameObject ladderPrefab;
	public GameObject ghostTower;

	// character prefabs
	public GameObject player;
	public GameObject npc;
	private Bounds npcBounds;
	public GameObject enemy;
	private Bounds enemyBounds;

	// make sure to put lowest value first...
	public float[] islandHeights;
	private int minHeightIndex;
	public int maxPlatformsPerIsland = 5;
	public int numberOfIslands;
	public float distanceBetweenIslands;

	// Enemies
	public int numEnemies = 15;

	// NPCs
	public int numAverageJoes = 3;
	public int numBuilders = 1;
	public int numFighters = 1;

	// keep track of all the platforms
	public List<GameObject> platforms = new List<GameObject>();
	private List<Platform> platformScripts = new List<Platform>();
	public List<Bounds> platformBoundsList = new List<Bounds>();

	public List<int> temporaryPlatformIndices = new List<int>();
	public List<int> enemyPlatforms = new List<int>();

	// Use this for initialization
	void Start ()
	{
		if (blackboard == null) {
			blackboard = GameObject.Find ("Blackboard").GetComponent<Blackboard> ();
		}
		// Set sea LEvel
		blackboard.seaLevel = seaLevel;

		// BUILD PLATFORMS
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
				temporaryPlatformIndices.Add(currentPlatformIndex);// update the temporary platform indices... use this to track which platforms have items/enemies on them
				platforms.Add (platform);
				Platform platformScript = platform.transform.GetChild(0).GetComponent<Platform>();// platform script is in the trigger component
				platformScripts.Add(platformScript);
				platformScript.maxCoins = Random.Range(1, 12);
				platformScript.ListCoins();
				Bounds platformBounds = platform.GetComponent<EdgeCollider2D> ().bounds;
				// save some getComponent calls
				platformBoundsList.Add (platformBounds);

				// If its not the bottom platform then disable the edges... so no boats
				if (islandHeightIndex != 0) {
					platformScript.edgeLeft.GetComponent<PayController>().enabled = false;
					platformScript.edgeRight.GetComponent<PayController>().enabled = false;
				}

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
					// LAdder always goes downwards to the sea
					// make sure ladder reaches into the sea, so stranded player can get back to platform...
					float heightDifference = platformBounds.max.y - seaLevel;
					ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - (heightDifference * 0.5f), ladder.transform.position.z);
					float ladderScaleFactor = (heightDifference) / ladderBounds.size.y;
					ladder.transform.localScale += new Vector3 (1, ladderScaleFactor, 1);
				}

				currentXPos += platformBounds.size.x;

//				// randomly assign an enemy to a platform
//				if (Random.Range (0f, 1f) > 0.5f) {
//					GameObject enemyObj = Instantiate (enemy, new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
//					if (enemyBounds == null) {
//						enemyBounds = enemyObj.GetComponent<BoxCollider2D> ().bounds;
//					}
//					enemyObj.transform.position = new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + enemyBounds.extents.y, platform.transform.position.z);
//					// TODO: maybe add enemy to a public list somewhere
//				}

				// Assign NPCs to a platform -- offset slightly or make sure NPCs don't spawn overlapping an enemy?
//				if (numBuilders > 0 || numFighters > 0 || numAverageJoes > 0){
//					GameObject npcObj = Instantiate (npc, new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
//					if (npcBounds == null) {
//						npcBounds = npcObj.GetComponent<BoxCollider2D> ().bounds;
//					}
//					npcObj.transform.position = new Vector3 (platformBoundsList [currentPlatformIndex].center.x, platformBoundsList [currentPlatformIndex].max.y + npcBounds.extents.y, platform.transform.position.z);
//					NPC npcScript = npcObj.GetComponent<NPC> ();
//					npcScript.platformScript = platformScript;
//					if (numBuilders > 0) {
//						npcScript.npcType = 2;
//						platformScript.builders.Add(npcObj);
//						numBuilders -= 1;
//					}else if (numFighters > 0){
//						npcScript.npcType = 3;
//						platformScript.fighters.Add(npcObj);
//						numFighters -= 1;
//					}else if (numAverageJoes > 0){
//						npcScript.npcType = 1;
//						platformScript.averageJoes.Add(npcObj);
//						numAverageJoes -= 1;
//					}
//					npcScript.SetType();
//					// TODO: maybe add npc to a public list somewhere
//				}



				currentPlatformIndex++;// update to keep track of where we are in the list
			}

			currentXPos += distanceBetweenIslands;
		}

		// Place player on a random platform, near the middle of the islands
//		int randomPlatformIndex = Random.Range(Mathf.FloorToInt(0.25f * currentPlatformIndex), Mathf.FloorToInt(0.75f * currentPlatformIndex));
//		Vector3 platformPosition = platforms[randomPlatformIndex].transform.position;
//		player.transform.position = new Vector3(platformBoundsList[randomPlatformIndex].center.x, platformBoundsList[randomPlatformIndex].max.y + 20.0f, player.transform.position.z);

		// Place enemies first... they were there first, hehe
		PlaceEnemies ();
		PlacePlayer();
		PlaceGhostTower();
		PlaceStructures();
		PlaceNPCs ();
	
	}

	void PlaceEnemies ()
	{

		// choose random indices to place enemies
		for (int i = 0; i < numEnemies; i++) {
			int randomIndex = Random.Range (0, temporaryPlatformIndices.Count);
			enemyPlatforms.Add (temporaryPlatformIndices [randomIndex]);
			temporaryPlatformIndices.RemoveAt (randomIndex);
		}


		if (numEnemies > 0) {
			for (int i = 0; i < enemyPlatforms.Count; i++) {
				GameObject platform = platforms [enemyPlatforms[i]];
				GameObject enemyObj = Instantiate (enemy, new Vector3 (platformBoundsList [i].center.x, platformBoundsList [enemyPlatforms[i]].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
				if (enemyBounds == null) {
					enemyBounds = enemyObj.GetComponent<BoxCollider2D> ().bounds;
				}
				enemyObj.transform.position = new Vector3 (platformBoundsList [enemyPlatforms[i]].center.x, platformBoundsList [enemyPlatforms[i]].max.y + enemyBounds.extents.y, platform.transform.position.z);
				numEnemies--;
			}
		}
	}

	void PlacePlayer(){
		// place player on a platform not occupied by enemies at first, enemy platforms have been removed from temporaryPlatformIndices
		int platformIndex = Random.Range (0, temporaryPlatformIndices.Count);
		Vector3 platformPosition = platforms[temporaryPlatformIndices[platformIndex]].transform.position;
		player.transform.position = new Vector3(platformBoundsList[temporaryPlatformIndices[platformIndex]].center.x, platformBoundsList[temporaryPlatformIndices[platformIndex]].max.y + 20.0f, player.transform.position.z);
	}

	void PlaceGhostTower(){
		GameObject ghostTowerObject = Instantiate (ghostTower, new Vector3 (platforms [0].transform.position.x - 50.0f, 0, platforms [0].transform.position.z), Quaternion.identity) as GameObject;
		blackboard.ghostTower = ghostTowerObject;
		Bounds ghostTowerBounds = ghostTowerObject.GetComponent<EdgeCollider2D> ().bounds;
		blackboard.ghostTowerBounds = ghostTowerBounds;
	}

	void PlaceStructures ()
	{
		List<int> tempPlatformIndices = new List<int> ();
		for (int i = 0; i < platforms.Count; i++) {
			tempPlatformIndices.Add(i);
		}

		List<int> medicalPlatforms = new List<int>();

		// choose random indices to place enemies
		for (int i = 0; i < numMedicalStructures; i++) {
			int randomIndex = Random.Range (0, tempPlatformIndices.Count);
			medicalPlatforms.Add (tempPlatformIndices [randomIndex]);
			tempPlatformIndices.RemoveAt (randomIndex);
		}


		if (numMedicalStructures > 0) {
			for (int i = 0; i < medicalPlatforms.Count; i++) {
				GameObject platform = platforms [medicalPlatforms[i]];
				GameObject structureObj = Instantiate (medicalStructure, new Vector3 (platformBoundsList [i].center.x, platformBoundsList [medicalPlatforms[i]].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
				Altar structureScript = structureObj.GetComponent<Altar>();
				structureScript.platformScript = platformScripts[medicalPlatforms[i]];
				PayController structurePayScript = structureObj.GetComponent<PayController>();
				structurePayScript.platformScript = platformScripts[medicalPlatforms[i]];
				if (medicalStructureBounds == null) {
					medicalStructureBounds = structureObj.GetComponent<BoxCollider2D> ().bounds;
				}
				structureObj.transform.position = new Vector3 (platformBoundsList [medicalPlatforms[i]].center.x, platformBoundsList [medicalPlatforms[i]].max.y + medicalStructureBounds.extents.y, platform.transform.position.z);
				numMedicalStructures--;
			}
		}
	}

	void PlaceNPCs ()
	{
		List<int> tempPlatformIndices = new List<int> ();
		for (int i = 0; i < platforms.Count; i++) {
			tempPlatformIndices.Add (i);
		}

		List<int> npcPlatforms = new List<int> ();

		// choose random indices to place npcs, platforms = num of NPCs
		for (int i = 0; i < (numBuilders + numFighters + numAverageJoes); i++) {
			int randomIndex = Random.Range (0, tempPlatformIndices.Count);
			npcPlatforms.Add (tempPlatformIndices [randomIndex]);
			tempPlatformIndices.RemoveAt (randomIndex);
		}


		if (numBuilders > 0 || numFighters > 0 || numAverageJoes > 0) {
			for (int i = 0; i < npcPlatforms.Count; i++) {
				// select one of the random platforms
				GameObject platform = platforms [npcPlatforms [i]];

				// instantiate an NPC
				GameObject npcObj = Instantiate (npc, new Vector3 (platformBoundsList [npcPlatforms [i]].center.x, platformBoundsList [npcPlatforms [i]].max.y + 20.0f, platform.transform.position.z), Quaternion.identity) as GameObject;
				if (npcBounds == null) {
					npcBounds = npcObj.GetComponent<BoxCollider2D> ().bounds;
				}

				// place the NPC at a location on the platform
				npcObj.transform.position = new Vector3 (platformBoundsList [npcPlatforms [i]].center.x, platformBoundsList [npcPlatforms [i]].max.y + npcBounds.extents.y, platform.transform.position.z);
				NPC npcScript = npcObj.GetComponent<NPC> ();
				npcScript.platformScript = platformScripts [npcPlatforms [i]];
				if (numBuilders > 0) {
					npcScript.npcType = 2;
					platformScripts [npcPlatforms [i]].builders.Add (npcObj);
					numBuilders -= 1;
				} else if (numFighters > 0) {
					npcScript.npcType = 3;
					platformScripts [npcPlatforms [i]].fighters.Add (npcObj);
					numFighters -= 1;
				} else if (numAverageJoes > 0) {
					npcScript.npcType = 1;
					platformScripts [npcPlatforms [i]].averageJoes.Add (npcObj);
					numAverageJoes -= 1;
				}
				npcScript.SetType ();
				// TODO: maybe add npc to a public list somewhere
			}
		}


	}

}
