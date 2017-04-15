using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

	public Blackboard blackboard;

	public float dismountSpeed = 1.0f;
	public bool overLadder = false;
	public Transform ladderTransform;
	public bool mountedLadder = false;
	public bool jumpedFromLadder = false;

	public float moveSpeed = 10;
	public float climbSpeed = 9.8f;
	private float gravity = -50;
	private Vector3 velocity;
	private float jumpVelocity = 20;

	// Water movement
	public bool inSea = false;

	private Controller2D controller;

	void Start ()
	{
		controller = GetComponent<Controller2D> ();

		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}
	}

	void Update ()
	{

//		if (controller.collisions.above || controller.collisions.below) {
//			velocity.y = 0;
//		}
		if (controller.collisions.below) {
			velocity.y = 0;
		}

		// Jumping from ground
		if (Input.GetButtonDown ("Jump") && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		velocity.x = input.x * moveSpeed;
		velocity.y += gravity * Time.deltaTime;

		// --- Mounting, dismounting, jumping and walking over a ladder/rope ---
		// if over a ladder, then climb/mount if vertical input
		// if just walking over a ladder then don't fall through it (unless jump is pressed form a mounted ladder)
		if (overLadder) {
			if (!controller.ignoreVerticalCollisions) {
				controller.ignoreVerticalCollisions = true;
			}
			if (input.y != 0) {
				mountedLadder = true;
				jumpedFromLadder = false;
				transform.position = new Vector3 (ladderTransform.position.x, transform.position.y, transform.position.z);
			} else {
				if (!jumpedFromLadder) {
					// stop player from falling through ladder
					velocity.y = 0;
				}
			}

			// clear a few sea parameters
			if (controller.inSea) {
				controller.inSea = false;
			}
			if (controller.swimming) {
				controller.swimming = false;
			}

		}

		if (!overLadder && controller.ignoreVerticalCollisions) {
			controller.ignoreVerticalCollisions = false;
		}

		// jumping from ladder
		if (Input.GetButtonDown ("Jump") && mountedLadder) {
//			velocity.y = jumpVelocity;
			jumpedFromLadder = true;
			mountedLadder = false;
		}
		// Jumping from standing on top of a ladder
		if (Input.GetButtonDown ("Jump") && overLadder && !mountedLadder) {
			velocity.y = jumpVelocity;
		}

		if (mountedLadder) {
			velocity.y = input.y * climbSpeed;
		}
		if (jumpedFromLadder) {
			velocity.y += gravity * Time.deltaTime;
		}

		if (transform.position.y <= blackboard.seaLevel && !controller.inSea) {
			controller.inSea = true;
			blackboard.playerInSea = true;
		}

		if (transform.position.y > blackboard.seaLevel) {
			blackboard.playerInSea = false;
		}

		if (transform.position.y >= blackboard.skyLevel) {
			blackboard.playeronTopPlatform = true;
		}

		if (transform.position.y < blackboard.skyLevel) {
			blackboard.playeronTopPlatform = false;
		}

		controller.Move (velocity * Time.deltaTime);

	}

}
