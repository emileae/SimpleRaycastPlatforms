using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// general type
	// 0 -> NPC
	// 1 -> coin
	// 2 -> killed animal
	public int generalType = 0;

	public NPC npcScript;
	public Item itemScript;

	void Start () {
		npcScript = GetComponent<NPC>();
		itemScript = GetComponent<Item>();
	}

	public void PickUpItem (Platform platformScript = null)
	{
		switch (generalType) {
			case 0:
				// remove NPC from platform list
				npcScript.working = false;
				if (npcScript.npcType == 1) {
					platformScript.averageJoes.Remove(gameObject);
				}else if (npcScript.npcType == 2){
					platformScript.builders.Remove(gameObject);
				}else if (npcScript.npcType == 3){
					platformScript.fighters.Remove(gameObject);
				}
				break;
			case 1:
				Debug.Log("Deactivate item");
				break;
			case 2:
				Debug.Log("Deactivate dead animal item... or something???");
				break;
			default:
				Debug.Log("fell through switch statement in pickup - Pickup.cs");
				break;
		}
		gameObject.SetActive(false);
	}
	public void DroppOffItem (Platform platformScript = null)
	{
		switch (generalType) {
			case 0:
			Debug.Log("__________'Dropped NPC on GROUND'________ " + npcScript.npcType);
				npcScript.platformScript = platformScript;
				if (npcScript.npcType == 1) {
					platformScript.averageJoes.Add(gameObject);
				}else if (npcScript.npcType == 2){
					platformScript.builders.Add(gameObject);
				}else if (npcScript.npcType == 3){
					platformScript.fighters.Add(gameObject);
				}
				npcScript.KeepMoving ();
				break;
			case 1:
				Debug.Log("Do Something with item... drop on ground");
				break;
			case 2:
				Debug.Log("Do Something with item... drop on ground");
				break;
			default:
				Debug.Log("fell through switch statement in DropOffItem - Pickup.cs");
				break;
		}
	}
}
