using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Altar : MonoBehaviour {

	public bool active;

	private Platform platformScript;

	private PlayerInteractions playerScript;

	public GameObject costIndicatorPrefab;
	private List<GameObject> costIndicators = new List<GameObject>();
	private List<CostIndicator> costIndicatorScripts = new List<CostIndicator>();
	private Bounds bounds;

	// payments
	private PayController payScript;
//	public bool purchased = false;
//	public int amountPaid = 0;

	private Animator anim;

	// Use this for initialization
	void Start ()
	{
		payScript = GetComponent<PayController> ();
		bounds = GetComponent<BoxCollider2D>().bounds;
		if (platformScript == null) {
			platformScript = transform.parent.GetComponent<Platform> ();
		}

		for (int i = 0; i < payScript.cost; i++) {
			GameObject costIndicator = Instantiate(costIndicatorPrefab, new Vector3(transform.position.x, transform.position.y + bounds.extents.y + i*2, transform.position.z), Quaternion.identity) as GameObject;
			costIndicator.transform.parent = transform;
			costIndicators.Add(costIndicator);
			costIndicatorScripts.Add(costIndicator.GetComponent<CostIndicator>());
			costIndicator.SetActive(false);
		}

		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		Debug.Log("Approached the altar, it costs: " + payScript.cost);
		if (col.CompareTag ("Player") && !payScript.purchased) {
			Debug.Log ("Collided with player!!!@");
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			playerScript.payScript = payScript;
			DisplayCost ();
		}
	}
	void OnTriggerExit2D(Collider2D col){
		Debug.Log("Approached the altar, it costs: " + payScript.cost);
		if (col.CompareTag ("Player") && playerScript != null) {
			if (payScript == playerScript.payScript) {
				if (payScript.amountPaid < payScript.cost) {
					payScript.ReturnFunds (playerScript);
				}
				playerScript.payScript = null;
			};
			playerScript = null;
		}
		HideCost ();
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

	public void ActivateAltar(){
		HideCost ();
		active = true;
		anim.SetBool("active", active);
	}

}
