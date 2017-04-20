using UnityEngine;
using System.Collections;

public class SimplePlayerMovement : MonoBehaviour {

	public float speed = 10.0f;
	public float gravity = -50;

	private Vector2 velocity = Vector2.zero;

	// simple grounded
	bool grounded = false;

	// Raycasting etc.
	float rayLength;
	public LayerMask collisionMask;
	private BoxCollider2D collider;
	private Bounds playerBounds;
	private float skinWidth = 0.2f;
	public bool collisionBelow = false;
	public bool collisionAbove = false;


	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update ()
	{

		collider = GetComponent<BoxCollider2D> ();
		playerBounds = collider.bounds;
		playerBounds.Expand (skinWidth * -2);// inset bound by skin width

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

//		velocity = new Vector2 (input.x * speed, gravity);

		velocity.x = input.x * speed;
		velocity.y += gravity * Time.deltaTime;

//		velocity *= Time.deltaTime;

		VerticalCollisions (ref velocity);


		if (collisionBelow) {
			velocity.y = 0;
		}

		transform.Translate(velocity * Time.deltaTime);

	}

	void VerticalCollisions (ref Vector2 velocity)
	{
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		// if moving up cast ray from top, if moving down cast rays from bottom
		Vector2 rayOrigin = new Vector2 (transform.position.x, playerBounds.min.y);
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
		Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

		if (hit) {
			float hitDistance = hit.distance;
			// adjust the ray distance
			if (hitDistance <= playerBounds.size.y) {
				velocity.y = (hitDistance - skinWidth) * directionY;
			}
			rayLength = hit.distance;

			if (hitDistance <= 0.01f) {
				collisionBelow = directionY == -1;
				collisionAbove = directionY == 1;
			}
		}
	}

}
