using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PayController : MonoBehaviour {

	public bool pickupable = false;
	public int cost = 3;
	public int amountPaid = 0;
	public bool purchased;

	public NPC npcScript;
	public Altar altarScript;
	public Edge edgeScript;
	private PickUp pickUpScript;
	private PlayerInteractions playerScript;
	private PayController payScript;

	public GameObject costIndicatorPrefab;
	private List<GameObject> costIndicators = new List<GameObject>();
	private List<CostIndicator> costIndicatorScripts = new List<CostIndicator>();
	private Bounds bounds;

	// Use this for initialization
	void Start ()
	{
		bounds = GetComponent<BoxCollider2D>().bounds;
		npcScript = GetComponent<NPC> ();
		altarScript = GetComponent<Altar> ();
		edgeScript = GetComponent<Edge>();

		pickUpScript = GetComponent<PickUp> ();
		payScript = GetComponent<PayController> ();
		if (pickUpScript == null) {
			pickupable = false;
		} else {
			pickupable = true;
		}

		for (int i = 0; i < cost; i++) {
			GameObject costIndicator = Instantiate(costIndicatorPrefab, new Vector3(transform.position.x, transform.position.y + bounds.extents.y + i*2, transform.position.z), Quaternion.identity) as GameObject;
			costIndicator.transform.parent = transform;
			costIndicators.Add(costIndicator);
			costIndicatorScripts.Add(costIndicator.GetComponent<CostIndicator>());
			costIndicator.SetActive(false);
		}
	}
	
	void OnTriggerEnter2D(Collider2D col){
//		Debug.Log("Approached the payable item, it costs: " + cost);
		if (col.CompareTag ("Player") && !purchased) {
//			Debug.Log ("Collided with player!!!@");
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			playerScript.payScript = payScript;
			DisplayCost ();
		}
	}
	void OnTriggerExit2D(Collider2D col){
//		Debug.Log("Approached the altar, it costs: " + payScript.cost);
		if (col.CompareTag ("Player") && playerScript != null) {
			if (payScript == playerScript.payScript) {
				if (payScript.amountPaid < payScript.cost) {
					payScript.ReturnFunds (playerScript);
				}
				playerScript.payScript = null;
			};
			playerScript = null;
			HideCost ();// only hide cost if player exits trigger
		}
	}

	void DisplayCost ()
	{
		for (int i = 0; i < costIndicators.Count; i++) {
			costIndicators[i].SetActive(true);
		}
	}
	void HideCost ()
	{
		for (int i = 0; i < costIndicators.Count; i++) {
			costIndicators[i].SetActive(false);
		}
	}

	public void PayCoin (int coinIndex)
	{
		costIndicatorScripts[coinIndex].Pay();
	}

	public bool Pay ()
	{
//		Debug.Log ("Paid Pay.cs");
		if (!purchased) {
			amountPaid += 1;
			PayCoin(amountPaid-1);

			// maybe set specific states while player is busy paying
			if (npcScript != null) {
				npcScript.beingPaid = true;
			}else if (altarScript != null){
			}
			else if (edgeScript != null){
			}

		}
		if (amountPaid >= cost) {
//			SetPickupable ();
			if (npcScript != null) {
				npcScript.beingPaid = false;
				npcScript.SetPickupable();
				npcScript.attackable = true;
			}else if (altarScript != null){
//				Debug.Log("-.-.-.--.-. Activate Altar");
				altarScript.ActivateAltar();
			}
			else if (edgeScript != null){
//				Debug.Log("--------|   Activate Edge item");
				edgeScript.ActivatePayment();
			}
			purchased = true;
			HideCost ();
		}

		return purchased;
	}

	public void ReturnFunds (PlayerInteractions playerScript)
	{
//		Debug.Log ("Return funds.....");

		// return coins to player inventory
		for (int i = 0; i < amountPaid; i++) {
//			Debug.Log("Return a coin....");
			playerScript.ReturnCoin();
		}

		// reset coin indicators
		for (int i = 0; i < costIndicatorScripts.Count; i++) {
			costIndicatorScripts[i].Refund();
		}

		// reset amountPaid last
		amountPaid = 0;
	}
}