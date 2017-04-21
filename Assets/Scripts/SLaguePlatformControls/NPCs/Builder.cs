using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Builder : MonoBehaviour {

	public int oldDirection = 0;
	private NPCController controller;

	public LayerMask carryLayer;
	public LayerMask structureLayer;
	public float itemDetectRadius = 1.0f;
	public float structureDetectRadius = 1.0f;
	public List<GameObject> carriedResource = new List<GameObject>();

	public List<Structure> structures = new List<Structure>();

	// detect when entering a new structure
	public Collider2D previousStructureToBuild;

	// working specific
	private Structure structureScript;
	private bool building = false;
	private Structure workingStructureScript;
	private float workTime = 0.0f;

	// Use this for initialization
	void Start () {
		controller = GetComponent<NPCController>();
	}
	
	// Update is called once per frame
	void Update ()
	{

		Collider2D carriableItem = Physics2D.OverlapCircle (transform.position, itemDetectRadius, carryLayer);
		if (carriableItem != null) {
			// can only add 1 item to builder's inventory
			if (!carriedResource.Contains (carriableItem.gameObject) && carriedResource.Count == 0) {
				carriableItem.gameObject.SetActive (false);
				carriedResource.Add (carriableItem.gameObject);
			}
		}
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer ("Structure")) {

			if (carriedResource.Count > 0) {
				Debug.Log ("Have resource... can build");
//				if (structureScript == null) {
				structureScript = col.gameObject.GetComponent<Structure> ();
//				}
				if (structureScript.active) {
					Debug.Log ("Active Structure... can build here: " + structureScript.gameObject.name);
					if (structureScript.cost > 0) {
						Debug.Log ("Still needs to be built: " + structureScript.gameObject.name);
						oldDirection = controller.direction;
						controller.direction = 0;
						StartCoroutine (BuildStructure ());
					} else {
						Debug.Log ("Already fully built");
					}
				} else {
					Debug.Log ("Inactive Structure... cant build here");
				}
			} else {
				Debug.Log ("No resource... cant build");
			}

		}

//		if (col.CompareTag ("Platform")) {
//			Platform platformScript = col.gameObject.GetComponent<Platform>();
//			platformScript.builders.Add(gameObject);
//		}

	}

//	void OnTriggerEnter2D (Collider2D col)
//	{
//		Debug.Log("Builder entered trigger........");
//		// When entering a new platform... check for structures to build
//		if (col.CompareTag ("Platform")) {
//			Debug.Log("Checking for structures...");
//			CheckForStructures (col.gameObject);
//		}
//
//		if (col.gameObject.layer == LayerMask.NameToLayer ("Structure")) {
//
//			if (workingStructureScript == null) {
//				workingStructureScript = col.gameObject.GetComponent<Structure> ();
//			}
//
//			if (structures.Contains (workingStructureScript) && !working) {
//				if (workingStructureScript.cost > 0 && carriedResource.Count > 0) {
//					Debug.Log ("Start work");
//					working = true;
//
//					if (oldDirection == 0 && controller.direction != 0) {
//						oldDirection = controller.direction;
//					}
//
//					controller.direction = 0;
//				}
//			}
//
//		}
//
//	}
//	void OnTriggerExit2D (Collider2D col)
//	{
//		// When entering a new platform... check for structures to build
//		if (col.CompareTag ("Platform")) {
//			ClearStructures ();
//		}
//
//		if (col.gameObject.layer == LayerMask.NameToLayer ("Structure")) {
//			working = false;
//			workingStructureScript = null;
//			workTime = 0.0f;
//			// if NPC was accidentally removed form working and is still then give it the old direction and hopefully ti will find its way back
//			if (controller.direction == 0) {
//				controller.direction = oldDirection;
//			}
//		}
//	}

//	public void CheckForStructures (Platform currentPlatformScript)
//	{
//		Debug.Log ("Check for structures... " + gameObject.name);
//
//		Platform platformScript = currentPlatformScript;//platformGameObject.GetComponent<Platform> ();
//		if (platformScript.structures.Count > 0) {
//			Debug.Log ("Structures present... " + gameObject.name);
//			for (int i = 0; i < platformScript.structures.Count; i++) {
//				if (platformScript.structures [i].active) {
//					structures.Add (platformScript.structures [i]);
//				}
//			}
//		}
//
//		// loof for active structures on platform
//		// attend to the first one on the list that needs to be attended to
//		if (structures.Count > 0 && !building) {
//			// go to the first item in build list
//			for (int i = 0; i < structures.Count; i++) {
//				if (structures [i].cost > 0) {
//					workingStructureScript = structures [i];
//					GoToStructure (workingStructureScript);
//					break;// break stops loop but continues with the rest of the function, return stops entire function
//				}
//			}
//		}
//
//	}

	public void ClearStructures ()
	{
		structures = new List<Structure>();
	}

	IEnumerator BuildStructure(){
		Debug.Log("---START BUILDING STRUCTURE---" + structureScript.gameObject.name);
		building = true;
		yield return new WaitForSeconds(structureScript.workTime);
		structureScript.PayStructure();
		GameObject resourceSpent = carriedResource[0];
		carriedResource.Remove (resourceSpent);
		Destroy(resourceSpent);// destoy the resource
		Debug.Log("Finish work");
		building = false;
		controller.direction = oldDirection;
//		workTime = 0.0f;
		structureScript = null;
		Debug.Log("---FINISH BUILDING STRUCTURE---");
	}

}
