using UnityEngine;
using System.Collections;

public class PayController : MonoBehaviour {

	public bool pickupable = false;
	public int cost = 3;
	public int amountPaid = 0;
	public bool purchased;

	public NPC npcScript;
	public Altar altarScript;
	private PickUp pickUpScript;

	// Use this for initialization
	void Start ()
	{
		npcScript = GetComponent<NPC> ();
		altarScript = GetComponent<Altar> ();
		pickUpScript = GetComponent<PickUp> ();
		if (pickUpScript == null) {
			pickupable = false;
		} else {
			pickupable = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool Pay ()
	{
		Debug.Log ("Paid Pay.cs");
		if (!purchased) {
			amountPaid += 1;
			if (npcScript != null) {
				Debug.Log("display cost indicators...");
			}else if (altarScript != null){
				altarScript.PayCoin(amountPaid-1);
			}
		}
		if (amountPaid >= cost) {
//			SetPickupable ();
			if (npcScript != null) {
				npcScript.SetPickupable();
			}else if (altarScript != null){
				Debug.Log("-.-.-.--.-. Activate Altar");
				altarScript.ActivateAltar();
			}
			purchased = true;
		}

		return purchased;
	}

	public void ReturnFunds(PlayerInteractions playerScript)
	{
		Debug.Log("Return funds.....");
		playerScript.currency += amountPaid;
		amountPaid = 0;
	}
}