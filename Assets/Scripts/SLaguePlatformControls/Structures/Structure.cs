using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour {

	// Need to know the platform for reason(s):
	// - so that platform can keep a list of structures that need to be built/ maintained
	public Platform platformScript;

	// Structure types
	// 0 -> bridge
	// 1 -> up elevator
	// 2 -> down elevator
	// 3 -> right elevator
	// 4 -> left elevator
	// 5 -> turret
	public int structureType;

	// structure specific parameters
	public int cost = 3;
	public float workTime = 2.0f;
	public float degradeTime = 50.0f;

	public bool active = false;
	private PlayerInteractions playerInteractions;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerInteractions = col.gameObject.GetComponent<PlayerInteractions>();
			playerInteractions.structureScript = this;
		}
	}
	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerInteractions = col.gameObject.GetComponent<PlayerInteractions> ();
			if (playerInteractions.structureScript == this) {
				playerInteractions.structureScript = null;
			}
		}
	}

	public void ToggleActivation ()
	{
		active = !active;
		if (active) {
			AddToBuildList ();
		} else {
			RemoveFromBuildList();
		}
	}

	public void PayStructure ()
	{
		cost--;
		Debug.Log("Pay Structure: " + cost);
		if (cost <= 0) {
			Debug.Log("Paid for structure... now activate");
			ActivateStructure();
		}
	}

	void AddToBuildList ()
	{
		Debug.Log("Add to Build List: " + platformScript);
		Debug.Log("Add to Build List..: " + platformScript.builders.Count);
		if (platformScript.builders.Count > 0) {
			for (int i = 0; i < platformScript.builders.Count; i++) {
				Builder builderScript = platformScript.builders[i].GetComponent<Builder>();
				builderScript.structures.Add(this);
			}
		}
	}

	void RemoveFromBuildList(){
		if (platformScript.builders.Count > 0) {
			for (int i = 0; i < platformScript.builders.Count; i++) {
				Builder builderScript = platformScript.builders[i].GetComponent<Builder>();
				builderScript.structures.Remove(this);
			}
		}
	}

	void ActivateStructure(){
		switch(structureType){
			case 0:
				Debug.Log("structure type Bridge");
				Bridge structureScript0 = GetComponent<Bridge>();
				structureScript0.ActivateStructure(platformScript);
				break;
			case 1:
				Debug.Log("structure type 1... Elevator Up");
				Elevator structureScript1 = GetComponent<Elevator>();
				structureScript1.ActivateStructure(platformScript);
				break;
			case 2:
				Debug.Log("structure type 0");
				break;
			case 3:
			Debug.Log("structure type 3... Elevator right");
				Elevator structureScript3 = GetComponent<Elevator>();
				structureScript3.ActivateStructure(platformScript);
				break;
			case 4:
				Debug.Log("structure type 0");
				break;
			case 5:
				Debug.Log("structure type 5.. Turret");
				Turret structureScript5 = GetComponent<Turret>();
				structureScript5.ActivateStructure(platformScript);
				break;
			default:
				Debug.Log("Fall through Structure.cs finding out which structure to build");
				break;
		}
	}

}
