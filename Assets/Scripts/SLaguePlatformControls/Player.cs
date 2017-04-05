using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

	public float dismountSpeed = 1.0f;
	public bool overLadder = false;
	public Transform ladderTransform;
	public bool mountedLadder = false;
	public bool jumpedFromLadder = false;

	private float moveSpeed = 2;
	private float climbSpeed = 1.8f;
	private float gravity = -50;
	private Vector3 velocity;
	private float jumpVelocity = 8;

	private Controller2D controller;

	void Start () {
		controller = GetComponent<Controller2D>();
	}

	void Update ()
	{

		if (controller.collisions.above || controller.collisions.below) {
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

		controller.Move(velocity*Time.deltaTime);
	}

}
