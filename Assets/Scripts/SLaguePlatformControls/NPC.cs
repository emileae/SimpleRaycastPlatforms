using UnityEngine;
using System.Collections;

// TODO: maybe set a timeout so that if player hovers over NPC for too long then NPC just keeps walking around
//using System.ComponentModel;

public class NPC : MonoBehaviour {

	public Blackboard blackboard;

	// Type integer specifies the type of NPC
	// 0 -> not purchased
	// 1 -> average Joe
	// 2 -> builder
	// 3 -> fighter
	// etc... expand if there's interest (boat upgrader... platform expander...)
	public int npcType = 0;
	public int maxHP;
	public int hp = 5;// health points
	public int maxAP;
	public int ap = 1;// attack points
	public int production = 10;
//	public int cost = 3;// update cost once NPC is upgraded, by landing on a platform, then if NPC is lost by falling in the water, it will cost more to get them back... skilled

	// handy to use for quick lookup
//	public bool purchased = false;

	// STRUCTURE interactions
	private float healTime;
	public bool stopToHeal = false;

	// PLATFORM
	public Platform platformScript;

	public bool isPackage = false;

	// payments from player
	public bool beingPaid = false;
	private PayController payScript;
//	public int amountPaid = 0;

	// NPC movement script
	public float timeHovered = 0.0f;
	public float npcHoverTime = 2.0f;
	public NPCController npcController;// this was moveScript of type NPCMove

	// different skins
	public GameObject averageJoeSkin;
	public GameObject builderSkin;
	public GameObject fighterSkin;

	// Keep track of the playerScript
//	private PlayerController playerScript;
	private PlayerInteractions playerScript;

	// NPC WORK
	public bool working = false;
	public GameObject coin;
	public float workTimeElapsed = 0.0f;
	public float workTime = 5.0f;

	// Attacking
	public bool attackable = false;
	public bool attacking = false;
	public Enemy enemyScript;
	public float attackTime = 1.5f;
	public float attackRadius = 3.0f;

	// trigger trackers
	private bool changingDirection = false;

	// TODO: moved this initialisation to Awake because errors were being thrown during island generation, where triggers were fired before NPC scripts were loaded
	void Awake () {
		npcController = GetComponent<NPCController>();
		payScript = GetComponent<PayController> ();
	}

	void Start ()
	{
		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}
		SetType();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// no need to hover if player doesnt have to pay for NPC
//		if (npcController.stopForPlayer) {
//			timeHovered += Time.deltaTime;
//			if (payScript.amountPaid > 0) {
//				timeHovered = 0;
//			}
//			if (timeHovered >= npcHoverTime) {
//				KeepMoving();
//			}
//		}
	}

	void OnTriggerEnter2D (Collider2D col)
	{

		if (col.CompareTag ("Player")) {
			Debug.Log ("Collided with player!!!@");
//			playerScript = col.gameObject.GetComponent<PlayerController> ();
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (!payScript.purchased) {
				payScript.purchased = true;
				attackable = true;// this must be removed if reverting to paying for NPCs
				Work ();// this must be removed if reverting to paying for NPCs
				SetPickupable ();
				// if player isn't currently dealing with another NPC, then stop
//				if (playerScript.payScript == null) {
//					StopForPlayer ();
//				}
			} else {
				// allow NPC to be picked up by player
				SetPickupable ();
			}
			// set npcScript to this script
			// Add this if player is going to pay for NPCs
//			playerScript.payScript = payScript;
		} 

//		if (col.CompareTag ("Enemy")) {
//			if (npcController != null && payScript.purchased) {
//				npcController.stopForEnemy = true;
//				enemyScript = col.gameObject.GetComponent<Enemy> ();
//			}
//		}

		// Structure interaction
		if (col.CompareTag ("Structure")) {
			Altar structureScript = col.gameObject.GetComponent<Altar> ();
			if (structureScript.active) {
				switch (structureScript.structureType) {
					case 0:
						Debug.Log ("Defense structure... do nothing");
						break;
					case 1:
						Debug.Log ("Medical structure, heal here");
						if (hp < maxHP) {
							healTime = structureScript.healTime;
							stopToHeal = true;
							Heal ();
						}
						break;
					default:
						Debug.Log ("Fell through structure trigger NPC.cs");
						break;
				}
			}
		}

		
		// now NPCs start working as soon as they come in contact with the player
		// This is where NPCs are assigned a task
//		if (col.CompareTag ("Platform")) {
//			if (payScript.purchased && !working) {
//				Work();
//			}
//		}

	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerScript = col.gameObject.GetComponent<PlayerInteractions> ();
			if (payScript == playerScript.payScript) {
				if (payScript.amountPaid < payScript.cost) {
					payScript.ReturnFunds (playerScript);
					KeepMoving ();
				}
				playerScript.payScript = null;
			};
			// remove NPC from being able to be picked up by player
			if (playerScript.pickupableItems.Contains (gameObject)) {
				playerScript.pickupableItems.Remove(gameObject);
			}
			if (!payScript.purchased) {
				KeepMoving ();
			}
			playerScript = null;
			timeHovered = 0;
			beingPaid = false;
		}
//		else if (col.CompareTag ("Edge")) {
//			if (changingDirection) {
//				changingDirection = false;
//			}
//		}

//		if (col.CompareTag("Enemy")){
//			npcController.stopForEnemy = false;
//			enemyScript = null;
//		}

		// Structure interaction
		if (col.CompareTag ("Structure")) {
			Altar structureScript = col.gameObject.GetComponent<Altar> ();
			switch (structureScript.structureType) {
				case 0:
					Debug.Log("Defense structure... do nothing");
					break;
				case 1:
					Debug.Log("Medical structure, heal here");
					stopToHeal = false;
					break;
				default:
					Debug.Log("Fell through structure trigger NPC.cs");
					break;
			}
		}

	}

	public void ChangeDirection ()
	{
		npcController.direction *= -1;
	}

	public void SetType(){
		if (npcType == 1) {
			averageJoeSkin.SetActive(true);
			builderSkin.SetActive(false);
			fighterSkin.SetActive(false);
			hp = 3;
			maxHP = hp;
			ap = 1;
			maxAP = ap;
			payScript.cost = 3;
			workTime = 10;
		}else if (npcType == 2){
			averageJoeSkin.SetActive(false);
			builderSkin.SetActive(true);
			fighterSkin.SetActive(false);
			hp = 2;
			maxHP = hp;
			ap = 2;
			maxAP = ap;
			payScript.cost = 5;
			workTime = 30;
		}else if (npcType == 3){
			averageJoeSkin.SetActive(false);
			builderSkin.SetActive(false);
			fighterSkin.SetActive(true);
			hp = 7;
			maxHP = hp;
			ap = 3;
			maxAP = ap;
			payScript.cost = 4;
			workTime = 60;
		}
	}


	public void StopForPlayer ()
	{
		Debug.Log("Show unit cost when stopped");
		npcController.stopForPlayer = true;
	}
	public void KeepMoving ()
	{
		if (npcController.direction == 0) {
			if (Random.Range (0.0f, 1.0f) >= 0.5f) {
				npcController.direction = 1;
			} else {
				npcController.direction = -1;
			}
		}
		if (payScript.purchased && !working) {
			Work();
		}
		npcController.stopForPlayer = false;
	}

	public void SetPickupable ()
	{
		// set pickupableItem on playerScript
		if (playerScript != null) {
			if (!playerScript.pickupableItems.Contains(gameObject)){
				playerScript.pickupableItems.Add(gameObject);
			}
		}
	}

	void Work()
	{
		working = true;
		StartCoroutine(PerformWork());
	}
	IEnumerator PerformWork ()
	{
		yield return new WaitForSeconds (workTime);

		// Use the Platform script's built-in coins, so less instantiating
		if (platformScript.coins.Count > 0) {
			// TODO: be careful here, if 2 NPCs both try to activate the same coin at the same time then probably an error/bug here
			GameObject foundCoin = platformScript.coins [0];
			foundCoin.SetActive (true);
			foundCoin.transform.position = transform.position;
			platformScript.coins.Remove (foundCoin);
			Work ();
		} else {
			Debug.Log("NO MORE COINS TO FIND!!!!!!!!!!");
		}

		//Work();
	}

	void Heal(){
		StartCoroutine(PerformHealing());
	}
	IEnumerator PerformHealing ()
	{
		yield return new WaitForSeconds (healTime);
		Debug.Log("heal A: " + hp);
		hp += 1;
		Debug.Log("heal B: " + hp);
		if (hp < maxHP) {
			Heal();
		} else {
			stopToHeal = false;
		}
	}

	public IEnumerator Attack ()
	{
		attacking = true;
		yield return new WaitForSeconds (attackTime);
		if (enemyScript != null) {
			Debug.Log ("HIT Enemy!!!");
			enemyScript.hp -= ap;
			if (enemyScript.hp <= 0) {
				Debug.Log ("Killed Enemy");
				npcController.stopForEnemy = false;
				enemyScript.Die ();
				enemyScript = null;
				KeepMoving();// NPC carries on idling/working on the platform
			}
		}
		attacking = false;// set attacking to false so that the Attack coroutine is called again in NPC update loop
	}

	public void Die ()
	{
		Debug.Log ("play NPC death aniamtion");
		if (npcType == 1) {
			platformScript.averageJoes.Remove(gameObject);
		}else if (npcType == 2){
			platformScript.builders.Remove(gameObject);
		}else if (npcType == 3){
			platformScript.fighters.Remove(gameObject);
		}
		// TODO: instead of destroying make the ghost take NPC away to the jail platform...
		//Destroy (gameObject);
		Bounds npcBounds = GetComponent<BoxCollider2D>().bounds;
		gameObject.transform.position =  new Vector3 (blackboard.ghostTowerBounds.center.x,blackboard.ghostTowerBounds.max.y + npcBounds.extents.y, blackboard.ghostTower.transform.position.z);
	}

	// Building
	// called from building sites, like edges (Edge.cs) and altars/ build points (Altar.cs)
	public void GoToBuildSite (Transform buildingSite)
	{
		if (buildingSite.position.x > transform.position.x) {
			npcController.direction = 1;
		}else if (buildingSite.position.x < transform.position.x){
			npcController.direction = -1;
		}
	}
	public void StopToBuild(){
		npcController.direction = 0;
		Debug.Log("Play build animation......");
	}
	public void FinishBuild (int postBuildDirection = 1)
	{

		npcController.direction = postBuildDirection;

		KeepMoving();
		Debug.Log("Play finish build animation......");
	}
}
