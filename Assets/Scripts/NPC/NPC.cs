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
//	public int cost = 3;// update cost once NPC is upgraded, by landing on a platform, then if NPC is lost by falling in the water, it will cost more to get them back... skilled

	// handy to use for quick lookup
//	public bool purchased = false;

	public bool isPackage = false;

	// payments from player
	private PayController payScript;
//	public int amountPaid = 0;

	// NPC movement script
	public float timeHovered = 0.0f;
	public float npcHoverTime = 2.0f;
	public NPCController npcController;// this was moveScript of type NPCMove

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

	// Attacking
	public bool attacking = false;
	public Enemy enemyScript;
	public float attackTime = 1.5f;

	// trigger trackers
	private bool changingDirection = false;

	// Use this for initialization
	void Start () {
		npcController = GetComponent<NPCController>();
		payScript = GetComponent<PayController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (npcController.stopForPlayer) {
			timeHovered += Time.deltaTime;
			if (payScript.amountPaid > 0) {
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
			Debug.Log ("Collided with player!!!@");
//			playerScript = col.gameObject.GetComponent<PlayerController> ();
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (!payScript.purchased) {
				// if player isn't currently dealing with another NPC, then stop
				if (playerScript.payScript == null) {
					StopForPlayer ();
				}
			} else {
				SetPickupable ();
			}
			// set npcScript to this script
			playerScript.payScript = payScript;
		} else if (col.CompareTag ("Edge")) {
			if (!changingDirection) {
				npcController.direction *= -1;
				changingDirection = true;
			}
		}

		if (col.CompareTag("Enemy")){
			npcController.stopForEnemy = true;
			enemyScript = col.gameObject.GetComponent<Enemy>();
		}

		// This is where NPCs are assigned a task
		if (col.CompareTag ("Platform")) {
			if (payScript.purchased && npcType == 0 && !working) {
				npcType = col.gameObject.GetComponent<Platform> ().platformType;
				Debug.Log ("Start working");
				Work();
			}else if (payScript.purchased && npcType != 0 && !working){
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
//			NPC npcScript = gameObject.GetComponent<NPC> ();
			if (payScript == playerScript.payScript) {
				if (payScript.amountPaid < payScript.cost) {
					payScript.ReturnFunds (playerScript);
					KeepMoving ();
				}
				playerScript.payScript = null;
			};
			if (gameObject == playerScript.pickupableItem) {
				playerScript.pickupableItem = null;
			};
			if (!payScript.purchased) {
				KeepMoving ();
			}
			playerScript = null;
			timeHovered = 0;
		}else if (col.CompareTag ("Edge")) {
			if (changingDirection) {
				changingDirection = false;
			}
		}

		if (col.CompareTag("Enemy")){
			npcController.stopForEnemy = false;
			enemyScript = null;
		}

	}


	public void StopForPlayer ()
	{
		Debug.Log("Show unit cost when stopped");
		npcController.stopForPlayer = true;
	}
	public void KeepMoving ()
	{
		npcController.stopForPlayer = false;
	}

//	public bool Pay ()
//	{
//		Debug.Log ("Paid NPC");
//		if (!purchased) {
//			amountPaid += 1;
//		}if (amountPaid >= cost) {
//			SetPickupable ();
//			purchased = true;
//		}
//
//		return purchased;
//	}

//	void ReturnFunds(PlayerInteractions playerScript)
//	{
//		Debug.Log("Return funds.....");
//		playerScript.currency += amountPaid;
//		amountPaid = 0;
//	}

	public void SetPickupable ()
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

	public IEnumerator Attack ()
	{
		attacking = true;
		yield return new WaitForSeconds (attackTime);
		Debug.Log ("Attack!!@!@ -----===========33333333>>>>>");
		if (enemyScript != null) {
			Debug.Log ("HIT Enemy!!!");
			enemyScript.hp -= ap;
		}
		if (enemyScript.hp <= 0) {
			Debug.Log ("Killed Enemy");
			npcController.stopForEnemy = false;
			enemyScript.Die();
			enemyScript = null;
			attacking = false;// stop attack and now, maybe pursue?
		} else {
			attacking = false;
		}
	}

	public void Die(){
		Debug.Log("play NPC death aniamtion");
		Destroy(gameObject);
	}
}
