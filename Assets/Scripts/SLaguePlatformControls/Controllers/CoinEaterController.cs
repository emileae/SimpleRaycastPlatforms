using UnityEngine;
using System.Collections;

public class CoinEaterController : MonoBehaviour {

	private Controller2D controller;
	private Enemy enemyScript;

	public float moveSpeed = 3;
	private float gravity = -50;
	private Vector3 velocity;

	public LayerMask npcLayer;
	public LayerMask playerLayer;
	public LayerMask itemLayer;

	public bool stopForPlayer = false;
	public bool stopForNPC = false;
	public bool eatCoin = false;
	public int direction = -1;



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


		Collider2D overlapItem = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, itemLayer);
		if (overlapItem != null) {
			Debug.Log ("!!!!! Within attack radius (Item eating ENEMY)..........");
			// Destroy coin object now, but can possibly bury it again or move it to the tower
			Destroy(overlapItem.gameObject);
		}

		Collider2D overlapNPC = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, npcLayer);
		if (overlapNPC != null) {
			enemyScript.npcScript = overlapNPC.gameObject.GetComponent<NPC> ();
			Debug.Log ("!!!!! Within attack radius (ENEMY)..........");
			stopForNPC = true;
		} else {
			stopForNPC = false;
			enemyScript.npcScript = null;
			enemyScript.attacking = false;// stop attack and now, maybe pursue?
		}

		// Attacking player
//		Collider2D overlapPlayer = Physics2D.OverlapCircle (transform.position, enemyScript.attackRadius, playerLayer);
//		if (overlapPlayer != null) {
//			Debug.Log("YOU'RE DEAD!!!");
//			Destroy(overlapPlayer.gameObject);
//		}



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
			Debug.Log("enemy - DIE - EnemyController.cs");
		}

		transform.Translate(velocity * Time.deltaTime);
//		controller.Move (velocity * Time.deltaTime);

	}

}