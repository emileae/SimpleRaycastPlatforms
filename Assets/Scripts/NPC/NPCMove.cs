using UnityEngine;
using System.Collections;

public class NPCMove : MonoBehaviour {

	private Rigidbody2D rb;

	public bool stopForPlayer = false;

	public float stopDistance = 0.5f;
	public int direction = 1;
	public float idleSpeed = 1.5f;
	private bool idling = true;
	public float minSpeed = 1;
	public float maxSpeed = 2;
	private bool facingRight = true;

	public Transform targetGridUnit;



	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		float speed = 0f;
		if (idling) {
			speed = idleSpeed + Random.Range (0.0f, 1.0f);
		}

		if (stopForPlayer) {
			rb.velocity = new Vector2 (0, rb.velocity.y);
		} else {
			rb.velocity = new Vector2 (direction * speed, rb.velocity.y);
		}


		if (rb.velocity.x > 0 && !facingRight) {
			Flip ();
		} else if (rb.velocity.x < 0 && facingRight) {
			Flip ();
		}

	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
