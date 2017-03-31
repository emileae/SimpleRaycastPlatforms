using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Keep track of the playerScript
	private PlayerInteractions playerScript;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			SetPickupable ();
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
