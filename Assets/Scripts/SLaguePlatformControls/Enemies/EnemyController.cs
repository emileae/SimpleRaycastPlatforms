using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private Controller2D controller;
	private Enemy enemyScript;

	public float moveSpeed = 3;
	private float gravity = -50;
	private Vector3 velocity;

	// being distracted
	public bool distracted = false;
	private GameObject distractionItem;
	private float distractedTime = 0.0f;
	private float totalDistractedTime = 3.0f;

	public LayerMask itemLayer;
	public LayerMask npcLayer;
	public LayerMask playerLayer;

	public bool stopForPlayer = false;
	public bool stopForNPC = false;
	public int direction = -1;

	private bool facingRight = true;



	void Start () {
		controller = GetComponent<Controller2D>();
		enemyScript = GetComponent<Enemy>();
	}

	void Update ()
	{

//		if (controller.collisions.above || controller.collisions.below) {
		velocity.y = 0;
//		}

		velocity.x = direction * moveSpeed;
//		velocity.y += gravity * Time.deltaTime;


		// ENEMY ATTACKS & UNIQUE BEHAVIOURS
		Collider2D overlapItem = null;
		Collider2D overlapNPC = null;
		Collider2D overlapPlayer = null;

		if (!distracted) {
			overlapItem = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, itemLayer);
			overlapNPC = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, npcLayer);
			overlapPlayer = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, playerLayer);
		} else {
			velocity.x = 0;
			distractedTime += Time.deltaTime;
			if (distractedTime >= totalDistractedTime) {
				Destroy(distractionItem);
				distracted = false;
				distractedTime = 0.0f;
			}
		}


		if (enemyScript.enemyType == 0) {
			if (overlapItem != null) {
				Debug.Log("Enemy overlap with item.....");
				if (overlapItem.CompareTag ("Animal")) {
					Debug.Log("Stop to deal with dead animal.....");
					distracted = true;
					distractionItem = overlapItem.gameObject;
				}
			}
		}else if (enemyScript.enemyType == 1) {
			if (overlapItem != null) {
				Debug.Log ("Eat a coin");
				// Destroy coin object now, but can possibly bury it again or move it to the tower
				Destroy (overlapItem.gameObject);
			}
		}

		if (overlapNPC != null) {
			enemyScript.npcScript = overlapNPC.gameObject.GetComponent<NPC> ();
			Debug.Log ("!!!!! Within attack radius (ENEMY)..........");

			// enemy Type 1 doesnt stop for NPCs, so no attack, it just wants to eat those items
			if (enemyScript.enemyType != 1) {
				stopForNPC = true;
			}
		} else {
//			if (overlapNPC.transform.position.x < transform.position.x) {
//				direction = -1;
//			} else {
//				direction = 1;
//			}
			stopForNPC = false;
			enemyScript.npcScript = null;
			enemyScript.attacking = false;// stop attack and now, maybe pursue?
		}

		// Attacking player
		if (overlapPlayer != null) {
			Debug.Log ("YOU'RE DEAD!!!");
			Destroy (overlapPlayer.gameObject);
		}



		// Make sure enemy is still alive to attack
		if (enemyScript.hp > 0) {

			if (stopForPlayer || stopForNPC) {
				velocity.x = 0;
				if (!enemyScript.attacking) {
					Debug.Log ("start attack (ENEMY)...");
					if (stopForNPC && enemyScript.npcScript.attackable) {
						StartCoroutine (enemyScript.Attack ());
					}
				}
			}

		} else {
			Debug.Log ("enemy - DIE - EnemyController.cs");
		}

		// flip the model
//		if (!facingRight && velocity.x > 0) {
//			Flip();
//		}else if (facingRight && velocity.x < 0){
//			Flip();
//		}

		transform.Translate(velocity * Time.deltaTime);
//		controller.Move (velocity * Time.deltaTime);

	}

//	void Flip(){
//		transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
//		facingRight = !facingRight
//	}

}