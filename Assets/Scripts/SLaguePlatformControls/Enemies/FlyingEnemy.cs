using UnityEngine;
using System.Collections;
using Steer2D;

public class FlyingEnemy : MonoBehaviour {

	public float speed = 10;
	public Transform ghostTower;

	public Seek seekScript;

	public bool goToTarget = false;
	public Transform target;

	private Vector3 direction;

	public bool captured;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (goToTarget) {
//			seekScript.enabled = true;
//			seekScript.TargetPoint = new Vector2 (target.position.x, target.position.y);
			direction = target.position - transform.position;
		} else {
//			seekScript.enabled = false;
			if (!captured) {
				if (target == null) {
					direction = ghostTower.position - transform.position;
				} else {
					goToTarget = true;
				}
			}
		}

		transform.Translate(direction.normalized * Time.deltaTime * speed);

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			Debug.Log ("Hit NPC.......");
		} else if (col.CompareTag ("AirCapture")) {
			captured = true;
			goToTarget = false;
			direction = Mathf.Sign(direction.x) * Vector3.right;
			Debug.Log("Entered flying trappppp: " + direction);
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("AirCapture")) {
			captured = false;
			direction *= -1;
		}
	}

}
