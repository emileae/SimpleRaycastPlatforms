using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {

	public int value = 5;
	public float workTime = 10.0f;// time NPC works to get resource
	private float timeWorked = 0.0f;// track work time in resource script so that if NPC is killed or removed then another NPC can take over and not have to start over again
	public float regenerateTime = 100.0f;

	public GameObject resourcePrefab;

	private bool harvesting = false;
	public bool harvestCompleted = false;
	
	// Update is called once per frame
	void Update ()
	{
		if (harvesting) {
			timeWorked += Time.deltaTime;
			if (timeWorked >= workTime) {
				Harvest();
				timeWorked = 0.0f;
			}
		}
	}

	public void StartHarvest(){
		harvesting = true;
	}

	public void Harvest(){
		harvestCompleted = true;
	}

	public GameObject FetchResource(){
		harvestCompleted = false;
		harvesting = false;
		value --;
		return resourcePrefab;
	}



}
