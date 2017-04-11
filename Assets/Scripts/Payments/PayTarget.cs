using UnityEngine;
using System.Collections;

public class PayTarget : MonoBehaviour {
//
//	private Blackboard blackboard;
//
//	public int foundationIndex;
//
//	public bool payable = true;
//	public int cost = 1;
//	public GameObject costIndicator;
//	public GameObject[] costIndicators;
//	public bool showCost = true;
//
//	public bool playerPaying = false;
//	public int amountPaid = 0;
//
//	void Start ()
//	{
//		if (blackboard == null) {
//			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
//		}
//		UpdateCostIndicatorArray ();
//	}
//
//	void Update ()
//	{
//		// make sure cost indicator array is the correct length
//		if (costIndicators.Length != cost) {
//			UpdateCostIndicatorArray();
//		}
//	}
//
//	void OnTriggerEnter2D (Collider2D col)
//	{
//		if (col.CompareTag ("Player")) {
//			PlayerController playerScript = col.gameObject.GetComponent<PlayerController> ();
//			if (showCost) {
//				ShowCost ();
//			}
//		}
//	}
//
//	void OnTriggerExit2D (Collider2D col)
//	{
//		if (col.CompareTag ("Player")) {
//			PlayerController playerScript = col.gameObject.GetComponent<PlayerController> ();
//			HideCost ();
//			if (playerPaying && amountPaid < cost) {
//				Debug.Log("Return player currency");
//				playerScript.ReturnPayment(amountPaid);
//				amountPaid = 0;
//				playerPaying = false;
//			}
//		}
//	}
//
//	void UpdateCostIndicatorArray (){
//		// remove old cost indicators
//		if (costIndicators.Length > 0) {
//			for (int i = 0; i < costIndicators.Length; i++) {
//				Destroy (costIndicators [i]);
//			}
//		}
//
//		// reset cost new indicators
//		costIndicators = new GameObject[cost];
//		for (int i = 0; i < costIndicators.Length; i++) {
//			GameObject ci = Instantiate (costIndicator, transform.position + new Vector3 (2 * i, 5, 0), Quaternion.Euler (-90, 0, 0)) as GameObject;
//			costIndicators[i] = ci;
//			costIndicators[i].SetActive(false);
//		}
//	}
//
//	void ShowCost ()
//	{
//		if (payable) {
//			Debug.Log ("Cost: " + cost);
//			for (int i = 0; i < costIndicators.Length; i++) {
//				costIndicators [i].SetActive (true);
//			}
//		}
//	}
//
//	void HideCost (){
//		for (int i = 0; i < costIndicators.Length; i++) {
//			costIndicators[i].SetActive(false);
//		}
//	}
//
//	void RemoveFirstCostIndicator(){
//		costIndicators[0].SetActive(false);
//	}
//
//	public bool Pay ()
//	{
//		if (payable) {
//			amountPaid += 1;
//			costIndicators [amountPaid - 1].SetActive (false);
//			Debug.Log ("amount paid: " + amountPaid);
//			if (amountPaid < cost) {
//				Debug.Log ("Player paid a coin...");
//				playerPaying = true;
//				return true;
//			} else {
//				playerPaying = false;
//				payable = false;
//				Debug.Log ("Take action on payment.....");
////				blackboard.CallNearestNPC(gameObject);
//				blackboard.AddToWorkList(gameObject);
//				return false;
//			}
//		} else {
//			return false;
//		}
//	}
//
}
