using UnityEngine;
using System.Collections;

public class PixelPlayer : MonoBehaviour {

	public float speedH = 1.0f;
	public float speedV = 1.0f;
	public float gravity = 1.0f;
	public bool onLadder = false;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float inputH = Input.GetAxisRaw ("Horizontal");
		float inputV = Input.GetAxisRaw ("Vertical");

		float gravityForce = -1 * gravity;

		if (onLadder) {
			gravityForce = 0;
		}

		if (!onLadder) {
			if (inputV > 0) {
				inputV = 0;
			}
		}

		Vector2 direction = new Vector2(inputH * speedH, inputV * speedV + gravityForce);
		transform.Translate(direction * Time.deltaTime);

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		Debug.Log("ENtered trigger.......");
		if (col.CompareTag ("Ground")) {
			speedV = 0;
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Ground")) {
			speedV = 1.0f;
		}
	}


}
