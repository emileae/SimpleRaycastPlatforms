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
		if (enabled) {
			if (col.CompareTag ("Player")) {

				Debug.Log (".....Player entered trigger......");

				playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
				SetPickupable ();

				// if its a coin... pick it up
				switch (pickUpScript.generalType) {
				case 0:
					break;
				case 1:
//					playerScript.PickUpItem (gameObject);
					break;
				case 2:
//					playerScript.PickUpItem(gameObject);
					break;
				default:
					Debug.Log ("fall through Item.cs ontriggerenter2d");
					break;
				}
			}
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (enabled) {
			if (col.CompareTag ("Player")) {
				playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
				if (playerScript.pickupableItems.Contains (gameObject)) {
					playerScript.pickupableItems.Remove (gameObject);
				};
				playerScript = null;
			}
		}
	}

	void SetPickupable ()
	{
		if (playerScript != null) {
			playerScript.pickupableItems.Add(gameObject);
		}
	}
}
