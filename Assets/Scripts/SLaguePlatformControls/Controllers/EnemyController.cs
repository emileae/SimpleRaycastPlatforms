using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private Controller2D controller;
	private Enemy enemyScript;

	public float moveSpeed = 3;
	private float gravity = -50;
	private Vector3 velocity;

	public bool stopForPlayer = false;
	public bool stopForNPC = false;
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


		// Make sure enemy is still alive to attack
		if (enemyScript.hp > 0) {

			if (stopForPlayer || stopForNPC) {
				velocity.x = 0;
				if (!enemyScript.attacking) {
					Debug.Log ("start attack...");
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