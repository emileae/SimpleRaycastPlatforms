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

			// if its a coin... pick it up
			if (pickUpScript.generalType == 1) {
				playerScript.PickUpItem(gameObject);
			}
		}

	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (playerScript.pickupableItems.Contains(gameObject)) {
				playerScript.pickupableItems.Remove(gameObject);
			};
			playerScript = null;
		}
	}

	void SetPickupable ()
	{
		if (playerScript != null) {
			playerScript.pickupableItems.Add(gameObject);
		}
	}
}
