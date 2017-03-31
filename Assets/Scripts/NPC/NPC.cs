using UnityEngine;
using System.Collections;

// TODO: maybe set a timeout so that if player hovers over NPC for too long then NPC just keeps walking around
//using System.ComponentModel;

public class NPC : MonoBehaviour {

	// Type integer specifies the type of NPC
	// 0 -> not purchased
	// 1 -> grass farmer
	// 2 -> tree farmer
	// 3 -> miner
	// etc... expand if there's interest (boat upgrader... platform expander...)
	public int npcType = 0;
	public int hp = 5;// health points
	public int ap = 1;// attack points
	public int production = 10;
	public int cost = 3;// update cost once NPC is upgraded, by landing on a platform, then if NPC is lost by falling in the water, it will cost more to get them back... skilled

	// handy to use for quick lookup
	public bool purchased = false;

	public bool isPackage = false;

	// payments from player
	public int amountPaid = 0;

	// NPC movement script
	public float timeHovered = 0.0f;
	public float npcHoverTime = 2.0f;
	public NPCMove moveScript;

	// different views depending on state
	public GameObject activeModel;
	public GameObject packageModel;

	// Keep track of the playerScript
//	private PlayerController playerScript;
	private PlayerInteractions playerScript;

	// NPC WORK
	public bool working = false;
	public GameObject coin;
	public float workTimeElapsed = 0.0f;
	public float workTime = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moveScript.stopForPlayer) {
			timeHovered += Time.deltaTime;
			if (amountPaid > 0) {
				timeHovered = 0;
			}
			if (timeHovered >= npcHoverTime) {
				KeepMoving();
			}
		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{

		if (col.CompareTag ("Player")) {
			Debug.Log("Collided with player!!!@");
//			playerScript = col.gameObject.GetComponent<PlayerController> ();
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (!purchased) {
				// if player isn't currently dealing with another NPC, then stop
				if (playerScript.npcPayScript == null) {
					StopForPlayer ();
				}
			} else {
				SetPickupable ();
			}
			// set npcScript to this script
			playerScript.npcPayScript = GetComponent<NPC> ();
		} else if (col.CompareTag ("Edge")) {
			moveScript.direction *= -1;
		}

		// This is where NPCs are assigned a task
		if (col.CompareTag ("Platform")) {
			if (purchased && npcType == 0 && !working) {
				npcType = col.gameObject.GetComponent<Platform> ().platformType;
				Debug.Log ("Start working");
				Work();
			}else if (purchased && npcType != 0 && !working){
				Debug.Log("Continue working as type: " + npcType);
				Work();
			}
		}

	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
//			playerScript = col.gameObject.GetComponent<PlayerController> ();
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			NPC npcScript = gameObject.GetComponent<NPC> ();
			if (npcScript == playerScript.npcPayScript) {
				if (amountPaid < cost) {
					ReturnFunds (playerScript);
					KeepMoving ();
				}
				playerScript.npcPayScript = null;
			};
			if (gameObject == playerScript.pickupableItem) {
				playerScript.pickupableItem = null;
			};
			if (!purchased) {
				KeepMoving ();
			}
			playerScript = null;
			timeHovered = 0;
		}
	}


	public void StopForPlayer ()
	{
		Debug.Log("Show unit cost when stopped");
		moveScript.stopForPlayer = true;
	}
	public void KeepMoving ()
	{
		moveScript.stopForPlayer = false;
	}
	public bool Pay ()
	{
		Debug.Log ("Paid NPC");
		if (!purchased) {
			amountPaid += 1;
		}if (amountPaid >= cost) {
			SetPickupable ();
			purchased = true;
		}

		return purchased;
	}

	void ReturnFunds(PlayerInteractions playerScript)
	{
		Debug.Log("Return funds.....");
		playerScript.currency += amountPaid;
		amountPaid = 0;
	}

	void SetPickupable ()
	{
		// set pickupableItem on playerScript
		if (playerScript != null) {
			playerScript.pickupableItem = gameObject;
		}
	}
	void Reactivate (PlayerInteractions player)
	{
		playerScript = player;
		playerScript.pickupableItem = gameObject;

	}

	void Work(){
		working = true;
		StartCoroutine(PerformWork());
	}
	IEnumerator PerformWork ()
	{
		yield return new WaitForSeconds(workTime);
		Debug.Log("Do the work animation... include some movement across platform...");
		Debug.Log("Produce a coin");
		Instantiate(coin, transform.position, Quaternion.identity);
		Work();
	}
}
