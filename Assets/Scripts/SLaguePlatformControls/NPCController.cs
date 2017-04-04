using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour {

	private Controller2D controller;
	private NPC npcScript;

	public float moveSpeed = 3;
	private float gravity = -50;
	private Vector3 velocity;

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
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		velocity.x = direction * moveSpeed;
		velocity.y += gravity * Time.deltaTime;

		if (stopForPlayer) {
			velocity.x = 0;	
		}


		if (npcScript.hp > 0) {

			if (stopForEnemy) {
				velocity.x = 0;
				if (!npcScript.attacking) {
					Debug.Log("start attack...");
					StartCoroutine(npcScript.Attack ());
				}
			}

		} else {
			Debug.Log("npc - DIE - NPCController.cs");
		}

		controller.Move(velocity*Time.deltaTime);
	}
}
