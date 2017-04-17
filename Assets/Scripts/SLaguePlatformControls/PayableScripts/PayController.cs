using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PayController : MonoBehaviour {

	public bool pickupable = false;
	public int cost = 3;
	public int costBlock = 5;
	public int numPaidTiers = 0;// accessed in PlayerInteractions
	public int payTiers;// number of separate payments to
	public int amountPaid = 0;
	public bool purchased;

	// keep track of resident platform in the case of altar and edgescripts... since they dont move and stay on 1 platform
	// -- added manually in prefab
	public Platform platformScript;

	public NPC npcScript;
	public Altar altarScript;
	public Edge edgeScript;
	private PickUp pickUpScript;
	private PlayerInteractions playerScript;
	private PayController payScript;

	public GameObject costIndicatorPrefab;
	private List<GameObject> costIndicators = new List<GameObject>();
	private List<CostIndicator> costIndicatorScripts = new List<CostIndicator>();
	private List<GameObject> heldCoinObjects = new List<GameObject>();
	private Bounds bounds;

	// Use this for initialization
	void Start ()
	{
		bounds = GetComponent<BoxCollider2D> ().bounds;
		npcScript = GetComponent<NPC> ();
		altarScript = GetComponent<Altar> ();
		edgeScript = GetComponent<Edge> ();

		pickUpScript = GetComponent<PickUp> ();
		payScript = GetComponent<PayController> ();
		if (pickUpScript == null) {
			pickupable = false;
		} else {
			pickupable = true;
		}

		if (cost <= costBlock) {
			for (int i = 0; i < cost; i++) {
				GameObject costIndicator = Instantiate (costIndicatorPrefab, new Vector3 (transform.position.x, transform.position.y + bounds.extents.y + i * 2, transform.position.z), Quaternion.identity) as GameObject;
				costIndicator.transform.parent = transform;
				costIndicators.Add (costIndicator);
				costIndicatorScripts.Add (costIndicator.GetComponent<CostIndicator> ());
				costIndicator.SetActive (false);
			}
		} else {
			for (int i = 0; i < costBlock; i++) {
				GameObject costIndicator = Instantiate (costIndicatorPrefab, new Vector3 (transform.position.x, transform.position.y + bounds.extents.y + i * 2, transform.position.z), Quaternion.identity) as GameObject;
				costIndicator.transform.parent = transform;
				costIndicators.Add (costIndicator);
				costIndicatorScripts.Add (costIndicator.GetComponent<CostIndicator> ());
				costIndicator.SetActive (false);
			}
		}

		// Set the number of pay tiers and reset cost block if necessary
		if (cost < costBlock) {
			costBlock = cost;// this makes the costBlock more usable when palyer is paying, so check if player has the correct amount of coins
		}
		payTiers = Mathf.CeilToInt(cost/costBlock);
	}
	
	void OnTriggerEnter2D (Collider2D col)
	{
//		Debug.Log("Approached the payable item, it costs: " + cost);
		if (col.CompareTag ("Player") && !purchased) {
//			Debug.Log ("Collided with player!!!@");
			if (payScript != null) {
				playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
				playerScript.payScript = payScript;
				DisplayCost ();
			}
		}
	}
	void OnTriggerExit2D (Collider2D col)
	{
//		Debug.Log("Approached the altar, it costs: " + payScript.cost);
		if (col.CompareTag ("Player") && playerScript != null) {

			if (payScript != null) {
				if (payScript == playerScript.payScript) {
					if (payScript.amountPaid < payScript.costBlock) {
						payScript.ReturnFunds (playerScript);
					}
					playerScript.payScript = null;
				}
				;
			}

			playerScript = null;
			HideCost ();// only hide cost if player exits trigger
		}
	}

	void DisplayCost ()
	{
		Debug.Log("Display cost...");
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
		costIndicatorScripts[coinIndex % costBlock].Pay();
	}

	public bool Pay (GameObject coinObject = null)
	{
//		Debug.Log ("Paid Pay.cs");
		if (!purchased) {
			amountPaid += 1;
			if (coinObject != null) {
				heldCoinObjects.Add (coinObject);
			}
			PayCoin (amountPaid - 1);

			// maybe set specific states while player is busy paying
			if (npcScript != null) {
				npcScript.beingPaid = true;
			} else if (altarScript != null) {
			} else if (edgeScript != null) {
			}

		}

		if (amountPaid >= cost) {
//			SetPickupable ();
			// paying for an NPC... (11-04-2017 this isn't being used, NPCs are employed on trigger enter from player)
			if (npcScript != null) {
				npcScript.beingPaid = false;
				npcScript.SetPickupable ();
				npcScript.attackable = true;
			} else {
				Build ();
			}

			purchased = true;
			HideCost ();

			// fully paid for so can delete the coin objects
			for (int i = 0; i < heldCoinObjects.Count; i++) {
				GameObject deletedCoinObject = heldCoinObjects[i];
				heldCoinObjects.Remove(heldCoinObjects[i]);
				Destroy(deletedCoinObject);
			}

		}else if (amountPaid >= (costBlock * numPaidTiers)){
			Debug.Log("Paid for a tier.......");
			numPaidTiers++;
			// fully paid for so can delete the coin objects
			for (int i = 0; i < heldCoinObjects.Count; i++) {
				GameObject deletedCoinObject = heldCoinObjects[i];
				heldCoinObjects.Remove(heldCoinObjects[i]);
				Destroy(deletedCoinObject);
			}
			// reset coin indicators
			for (int i = 0; i < costIndicatorScripts.Count; i++) {
				costIndicatorScripts[i].Refund();
			}
//			DisplayCost ();
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

	void Build(){
		Debug.Log("# Builders on platform: " + platformScript.builders.Count);
		Debug.Log("# crew on platform: " + platformScript.averageJoes.Count);
		if (altarScript != null){
			Debug.Log("Need a builder for Altar.................");
			altarScript.ActivateAltar();
		}
		else if (edgeScript != null){
			Debug.Log("Need a builder for Edge.................");
			edgeScript.ActivatePayment();
		}
	}

}