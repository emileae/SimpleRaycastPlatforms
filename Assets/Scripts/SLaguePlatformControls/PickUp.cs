using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// general type
	// 0 -> NPC
	// 1 -> coin
	// 2 -> killed animal
	public int generalType = 0;

	private PlayerInteractions playerScript;
	public NPC npcScript;
	public Item itemScript;

	void Start () {
		npcScript = GetComponent<NPC>();
		itemScript = GetComponent<Item>();
		playerScript = GameObject.Find("Player").GetComponent<PlayerInteractions> ();

	}

	public void PickUpItem (Platform platformScript)
	{
		switch (generalType) {
			case 0:
				// remove NPC from platform list
				npcScript.platformScript = platformScript;
				npcScript.working = false;
				if (npcScript.npcType == 1) {
					platformScript.averageJoes.Remove(gameObject);
				}else if (npcScript.npcType == 2){
					platformScript.builders.Remove(gameObject);
				}else if (npcScript.npcType == 3){
					platformScript.fighters.Remove(gameObject);
				}
				npcScript.platformScript = platformScript;
				npcScript.DeregisterNPCWithPlatform();
				gameObject.SetActive(false);
				break;
			case 1:
				Debug.Log("Deactivate item");
				gameObject.SetActive(false);
				break;
			case 2:
				Debug.Log("Deactivate dead animal item... or something???");
				break;
			default:
				Debug.Log("fell through switch statement in pickup - Pickup.cs");
				break;
		}
	}
	public void DroppOffItem (Platform platformScript)
	{
		switch (generalType) {
			case 0:
				npcScript.platformScript = platformScript;
				if (npcScript.npcType == 1) {
					platformScript.averageJoes.Add(gameObject);
				}else if (npcScript.npcType == 2){
					platformScript.builders.Add(gameObject);
				}else if (npcScript.npcType == 3){
					platformScript.fighters.Add(gameObject);
				}
				npcScript.RegisterNPCWithPlatform();
//				npcScript.KeepMoving ();
				break;
			case 1:
				Debug.Log("Do Something with item... drop on ground");
				gameObject.SetActive(true);
				break;
			case 2:
				Debug.Log("Do Something with item... drop on ground");
				break;
			default:
				Debug.Log("fell through switch statement in DropOffItem - Pickup.cs");
				break;
		}
	}


	/// TRIGGERS
	void OnTriggerExit2D (Collider2D col)
	{
		if (enabled) {
			if (col.CompareTag ("Player")) {
				if (playerScript.pickupableItems.Contains (gameObject)) {
					playerScript.pickupableItems.Remove (gameObject);
				};
				playerScript = null;
			}
		}
	}

}
