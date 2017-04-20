using UnityEngine;
using System.Collections;

public class BoxPackage : MonoBehaviour {

	private Blackboard blackboard;

	public float oceanCurrentSpeed = 5;
	public float moveSpeed = 10;
	private float gravity = -50;

	public Vector3 velocity;

	// Water movement
	public bool inSea = false;

	private PackageController2D controller;

	void Start ()
	{
		blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		controller = GetComponent<PackageController2D> ();
	}

	void Update ()
	{

//		if (controller.collisions.above || controller.collisions.below) {
//			velocity.y = 0;
//		}
		if (controller.collisions.below) {
			velocity.y = 0;
		}

		velocity.y += gravity * Time.deltaTime;


		if (transform.position.y <= blackboard.seaLevel && !controller.inSea) {
			velocity.y = 0;
			velocity.x -= oceanCurrentSpeed;
		}

		Debug.Log("Calling Moving vertically - A");

		velocity = new Vector3(0, velocity.y, 0);

		controller.Move (velocity * Time.deltaTime);

	}

}
