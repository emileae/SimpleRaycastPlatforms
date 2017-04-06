using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// general type
	// 0 -> NPC
	// 1 -> coin
	// 2 -> special item?
	public int generalType = 0;

	public NPC npcScript;
	public Item itemScript;

	void Start () {
		npcScript = GetComponent<NPC>();
		itemScript = GetComponent<Item>();
	}

	public void PickUpItem(){
		switch (generalType) {
			case 0:
				npcScript.working = false;
				break;
			case 1:
				Debug.Log("Deactivate item");
				break;
			default:
				Debug.Log("fell through switch statement in pickup - Pickup.cs");
				break;
		}
		gameObject.SetActive(false);
	}
	public void DroppOffItem ()
	{
		switch (generalType) {
			case 0:
				npcScript.KeepMoving ();
				break;
			case 1:
				Debug.Log("Do Something with item... drop on ground");
				break;
			default:
				Debug.Log("fell through switch statement in DropOffItem - Pickup.cs");
				break;
		}
	}
}
