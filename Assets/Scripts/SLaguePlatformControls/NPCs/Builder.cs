using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {

	private int oldDirection;
	private NPCController controller;

	public LayerMask carryLayer;
	public LayerMask structureLayer;
	public float itemDetectRadius = 10.0f;
	public float structureDetectRadius = 1.0f;
	public List<GameObject> carriedItems = new List<GameObject>();

	public List<Structure> structures = new List<Structure>();

	// working specific
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
			if (!carriedItems.Contains (carriableItem.gameObject)) {
				carriableItem.gameObject.SetActive (false);
				carriedItems.Add (carriableItem.gameObject);
			}
		}

		Collider2D structure = Physics2D.OverlapCircle (transform.position, structureDetectRadius, structureLayer);
		if (structure != null) {
			Debug.Log("Start work");
			if (oldDirection == 0 && controller.direction != 0) {
				oldDirection = controller.direction;
			}
			controller.direction = 0;
			workTime += Time.deltaTime;
			if (workingStructureScript == null) {
				workingStructureScript = structure.gameObject.GetComponent<Structure> ();
			}
			if (workTime > workingStructureScript.workTime) {
				Debug.Log("Finish work");
				controller.direction = oldDirection;
			}
		}else{
			workTime = 0.0f;
			workingStructureScript = null;
		}

		// add a function to incrementally add payment to structures, then wheneventually they are paid for they activate... simpler version of player pay script
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		// When entering a new platform... check for structures to build
		if (col.CompareTag ("Platform")) {
			CheckForStructures(col.gameObject);
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

}
