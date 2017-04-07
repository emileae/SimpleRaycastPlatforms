using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandGenerator : MonoBehaviour {

	public GameObject[] platformPrefabs;
	public GameObject ladderPrefab;

	public float[] islandHeights;
	public int maxPlatformsPerIsland = 5;
	public int numberOfIslands;
	public float distanceBetweenIslands;

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
				GameObject platform = (GameObject)Instantiate (platformPrefabs [0], new Vector3 (currentXPos, islandHeights [islandHeightIndex], 0), Quaternion.identity);
				platforms.Add (platform);
				Bounds platformBounds = platform.GetComponent<EdgeCollider2D> ().bounds;
				// save some getComponent calls
				platformBoundsList.Add (platformBounds);

				// add a ladder to each platform
				GameObject ladder = (GameObject)Instantiate (ladderPrefab, new Vector3 (platformBounds.min.x, islandHeights [islandHeightIndex], 0), Quaternion.identity);
				// TODO: can make this more efficient by only calling the GetComponent for the ladder once.... only true if the ladder stays the same for all platforms, i.e. no rope with different bounds, or alternative ladders
				Bounds ladderBounds = ladder.GetComponent<BoxCollider2D> ().bounds;


				// modify ladder position
				// TODO: scaling ladder deforms the sprite, might need to use a quad and set a repeating texture to keep sprite looking right
				if (j > 0) {
					if (platformBoundsList [currentPlatformIndex - 1].max.y > platformBounds.max.y) {
						Debug.Log("ladder needs to go up");
						float heightDifference = platformBoundsList [currentPlatformIndex - 1].max.y - platformBounds.max.y;
//						ladder.transform.position = new Vector3 (ladder.transform.position.x + ladderBounds.extents.x, platformBounds.max.y + ladderBounds.extents.y, ladder.transform.position.z);
						ladder.transform.position = new Vector3 (ladder.transform.position.x + ladderBounds.extents.x, platformBounds.max.y + (heightDifference * 0.5f), ladder.transform.position.z);
						float ladderScaleFactor = (heightDifference) / ladderBounds.size.y;
						ladder.transform.localScale += new Vector3(1, ladderScaleFactor, 1);
					}else if (platformBoundsList [currentPlatformIndex - 1].max.y < platformBounds.max.y){
						Debug.Log("ladder needs to go down");
						float heightDifference = platformBounds.max.y - platformBoundsList [currentPlatformIndex - 1].max.y;
//						ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - ladderBounds.extents.y, ladder.transform.position.z);
						ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - (heightDifference * 0.5f), ladder.transform.position.z);
						float ladderScaleFactor = (heightDifference) / ladderBounds.size.y;
						ladder.transform.localScale += new Vector3(1, ladderScaleFactor, 1);
					}else if (platformBoundsList [currentPlatformIndex - 1].max.y == platformBounds.max.y){
						Debug.Log("No ladder needed");
					}
				} else {
					// make sure ladder reaches into the sea, so stranded player can get back to platform...
					ladder.transform.position = new Vector3 (ladder.transform.position.x - ladderBounds.extents.x, platformBounds.max.y - ladderBounds.extents.y, ladder.transform.position.z);
				}

				currentXPos += platformBounds.size.x;


				currentPlatformIndex++;// update to keep track of where we are in the list
			}
			currentXPos += distanceBetweenIslands;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
