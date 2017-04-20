using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(BoxCollider2D))]
public class PackageController2D : RaycastController {

	public Blackboard blackboard;

	// specific to the sea game
	public bool inSea = false;
	public float fallDepth = 200.0f;
	private float fallDepthincrease = 0.0f;
	public bool swimming = false;

//	public LayerMask collisionMask;
//
//	const float skinWidth = 0.15f;
//	public int horizontalRayCount = 4;
//	public int verticalRayCount = 4;
//
//	private float horizontalRaySpacing;
//	private float verticalRaySpacing;
//
//	private BoxCollider2D collider;
//	private RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	// the animation sprites
	private bool facingRight = true;
	private SpriteRenderer sprite;

	public bool ignoreVerticalCollisions = false;

	// animations
	private Animator anim;

	public override void Start () {
		base.Start();
//		collider = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
//		CalculateRaySpacing ();

		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}
	}

	// detect collisions then move character
	public void Move (Vector3 velocity, bool standingOnPlatform = false)
	{

//		Debug.Log("calling box move....");

		// lots of optimisation in UpdateRaycastOrigins in RAycast controller
		UpdateRaycastOrigins ();
		collisions.Reset ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0 && !ignoreVerticalCollisions) {
			VerticalCollisions (ref velocity);
		}

		if (anim != null) {
			if (velocity.x != 0) {
				anim.SetBool ("isMoving", true);
			} else {
				anim.SetBool ("isMoving", false);
			}
		}

		if (velocity.x > 0 && !facingRight) {
			Flip ();
		} else if (velocity.x < 0 && facingRight) {
			Flip ();
		}

		if (standingOnPlatform) {
			collisions.below = true;
		}

		// falling into the sea
		if (!collisions.below && !inSea) {
			fallDepthincrease += 0.1f;
		}
		if (collisions.below) {
			fallDepthincrease = 0.0f;
		}
		if (inSea) {
//			Debug.Log ("fallDepthincrease: " + fallDepthincrease);
			if (!swimming) {
				float fracFallen = Mathf.Abs (transform.position.y - blackboard.seaLevel) / fallDepthincrease;
				if (fracFallen >= 0.99f) {
					swimming = true;
					fallDepthincrease = 0.0f;
				} else {
					velocity.y = Mathf.Lerp (velocity.y, 0.0f, fracFallen);
				}
//				Debug.Log ("velocity.y going down: " + velocity.y);
			} else {
//				Debug.Log ("Move uppppp");
				if (transform.position.y < blackboard.seaLevel) {
					velocity.y = 0.3f;
				} else {
					velocity.y = 0;
				}
			}
		}
		Debug.Log("Moving vertically");
		transform.Translate(velocity);
	}

	void Flip ()
	{
		if (sprite != null) {
			facingRight = !facingRight;
			sprite.flipX = !sprite.flipX;
		}
	}

	void HorizontalCollisions (ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
//			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) {

				// use the next ray if colliding within a collider/platform moving horizontally
				if (hit.distance == 0) {
					continue;
				}

				// adjust the ray distance
				velocity.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;

				if (hit.transform == blackboard.box) {
					Debug.Log("colliding with Box: " + directionX);
					hit.transform.gameObject.GetComponent<Controller2D>().Move(velocity);
				}

			}

		}
	}
	void VerticalCollisions (ref Vector3 velocity)
	{
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
		for (int i = 0; i < verticalRayCount; i++) {
			
			// if moving up cast ray from top, if moving down cast rays from bottom
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
//			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
//			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {
				// adjust the ray distance
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

				if (collisions.above) {
//					Debug.Log("ABOVE ------- HIT!!!!!");
				}

				if (collisions.below) {
//					Debug.Log("ABOVE ------- HIT!!!!!");
				}

			}

		}
	}


//	void UpdateRaycastOrigins(){
//		Bounds bounds = collider.bounds;
//		bounds.Expand(skinWidth * -2);// inset bound by skin width
//		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
//		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
//		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
//		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
//	}
//
//	void CalculateRaySpacing(){
//		Bounds bounds = collider.bounds;
//		bounds.Expand(skinWidth * -2);// inset bound by skin width
//
//		// make sure there are at least 2 rays
//		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
//		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
//
//		// calculate spacing between rays
//		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
//		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
//
//	}
//
//	struct RaycastOrigins{
//		public Vector2 topLeft, topRight;
//		public Vector2 bottomLeft, bottomRight;
//	}

	public struct CollisionInfo{
		public bool above, below;
		public bool left, right;

		public void Reset ()
		{
			above = below = false;
			left = right = false;
		}

	}

}
