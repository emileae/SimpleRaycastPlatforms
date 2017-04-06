using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Keep track of the playerScript
	private PlayerInteractions playerScript;
	private PickUp pickUpScript;

	void Start () {
		pickUpScript = GetComponent<PickUp>();
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			SetPickupable ();

			// if its a coin
			if (pickUpScript.generalType == 1) {
				playerScript.PickUpItem();
			}
		}

	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (gameObject == playerScript.pickupableItem) {
				playerScript.pickupableItem = null;
			};
			playerScript = null;
		}
	}

	void SetPickupable ()
	{
		if (playerScript != null) {
			playerScript.pickupableItem = gameObject;
		}
	}
}
