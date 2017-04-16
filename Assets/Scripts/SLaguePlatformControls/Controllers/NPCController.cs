using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

	private Controller2D controller;
	private NPC npcScript;

	public float moveSpeed = 3;
	private float gravity = -50;
	private Vector3 velocity;

	public LayerMask enemyLayer;
	public LayerMask animalLayer;

	public bool stopForPlayer = false;
	public bool stopForEnemy = false;
	public int direction = -1;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D>();
		npcScript = GetComponent<NPC>();
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if (controller.collisions.above || controller.collisions.below) {
		velocity.y = 0;
//		}

		velocity.x = direction * moveSpeed;
//		velocity.y += gravity * Time.deltaTime;

		if (stopForPlayer) {
			velocity.x = 0;	
		}

		if (npcScript.attackable) {
			Collider2D overlapEnemy = Physics2D.OverlapCircle (transform.position, npcScript.attackRadius, enemyLayer);
			if (overlapEnemy != null) {
				if (npcScript.enemyScript == null) {
					npcScript.enemyScript = overlapEnemy.gameObject.GetComponent<Enemy> ();
				}
				Debug.Log ("Within attack radius (npc)..........");
				stopForEnemy = true;
			} else {
				stopForEnemy = false;
				npcScript.enemyScript = null;
				npcScript.attacking = false;// stop attacking if there is no enemy
			}
		}

		// hunt animals
		Collider2D overlapAnimal = Physics2D.OverlapCircle (transform.position, npcScript.attackRadius, animalLayer);
		if (overlapAnimal != null) {
			Debug.Log("HuntAnimal");
			npcScript.Hunt(overlapAnimal.transform);
		} else {
			stopForEnemy = false;
			npcScript.enemyScript = null;
			npcScript.attacking = false;// stop attacking if there is no enemy
		}

		// if still alive
		if (npcScript.hp > 0) {

			if (stopForEnemy) {
				velocity.x = 0;
				if (!npcScript.attacking && npcScript.attackable) {
					Debug.Log ("start attack...");
					StartCoroutine (npcScript.Attack ());
				}
			}

		} else {
			Debug.Log ("npc - DIE - NPCController.cs");
		}

		if (!npcScript.attacking && npcScript.attackable && npcScript.hp < npcScript.maxHP && npcScript.stopToHeal) {
			velocity.x = 0;
		}

//		if (velocity.y != 0) {
//			controller.Move (velocity * Time.deltaTime);
//		} else {
			transform.Translate (velocity * Time.deltaTime);	
//		}
	}
}
