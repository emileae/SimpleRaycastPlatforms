using UnityEngine;
using System.Collections;

public class Farmer : MonoBehaviour {

	private NPCController controller;

	public GameObject resource;

	private bool harvesting = false;
	private Resource resourceScript;


	// Use this for initialization
	void Start () {
		controller = GetComponent<NPCController>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (harvesting && resourceScript != null) {
			controller.direction = 0;
			resourceScript.StartHarvest ();
			if (resourceScript.harvestCompleted) {
				harvesting = false;
				controller.direction = 1;// can make this more intelligent ... so find the drop off point and head in that direction
				resource = resourceScript.FetchResource();
			}
		}

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Resource")) {
			if (resource == null) {
				harvesting = true;
				resourceScript = col.GetComponent<Resource> ();
			}
		}else if (col.CompareTag ("DropOffPoint")) {
			if (resource != null){
				GameObject resourceObj = Instantiate(resource, transform.position, Quaternion.identity) as GameObject;
				resource = null;
			}
		}
	}

}
