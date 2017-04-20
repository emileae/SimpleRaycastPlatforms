using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {

	private int oldDirection = 0;
	private NPCController controller;

	public LayerMask carryLayer;
	public LayerMask structureLayer;
	public float itemDetectRadius = 1.0f;
	public float structureDetectRadius = 1.0f;
	public List<GameObject> carriedResource = new List<GameObject>();

	public List<Structure> structures = new List<Structure>();

	// working specific
	private bool working = false;
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
			if (!carriedResource.Contains (carriableItem.gameObject)) {
				carriableItem.gameObject.SetActive (false);
				carriedResource.Add (carriableItem.gameObject);
			}
		}


		if (working) {
			workTime += Time.deltaTime;
			if (workTime > workingStructureScript.workTime) {
				workingStructureScript.PayStructure();
				GameObject resourceSpent = carriedResource[0];
				carriedResource.Remove (resourceSpent);
				Destroy(resourceSpent);// destoy the resource
				Debug.Log("Finish work");
				working = false;
				controller.direction = oldDirection;
				workTime = 0.0f;
				workingStructureScript = null;
			}
		}

		// add a function to incrementally add payment to structures, then wheneventually they are paid for they activate... simpler version of player pay script
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		// When entering a new platform... check for structures to build
		if (col.CompareTag ("Platform")) {
			CheckForStructures (col.gameObject);
		}

		if (col.gameObject.layer == LayerMask.NameToLayer ("Structure")) {

			if (workingStructureScript == null) {
				workingStructureScript = col.gameObject.GetComponent<Structure> ();
			}

			if (structures.Contains (workingStructureScript) && !working) {
				if (workingStructureScript.cost > 0 && carriedResource.Count > 0) {
					Debug.Log ("Start work");
					working = true;

					if (oldDirection == 0 && controller.direction != 0) {
						oldDirection = controller.direction;
					}

					controller.direction = 0;
				}
			}

		}

	}
	void OnTriggerExit2D (Collider2D col)
	{
		// When entering a new platform... check for structures to build
		if (col.CompareTag ("Platform")) {
			ClearStructures ();
		}

		if (col.gameObject.layer == LayerMask.NameToLayer ("Structure")) {
			working = false;
			workingStructureScript = null;
			workTime = 0.0f;
			// if NPC was accidentally removed form working and is still then give it the old direction and hopefully ti will find its way back
			if (controller.direction == 0) {
				controller.direction = oldDirection;
			}
		}

	}

	void CheckForStructures (GameObject platformGameObject)
	{
		Platform platformScript = platformGameObject.GetComponent<Platform> ();
		if (platformScript.structures.Count > 0) {
			for (int i = 0; i < platformScript.structures.Count; i++) {
				if (platformScript.structures [i].active) {
					structures.Add(platformScript.structures [i]);
				}
			}
		}
	}

	void ClearStructures ()
	{
		structures = new List<Structure>();
	}

}
