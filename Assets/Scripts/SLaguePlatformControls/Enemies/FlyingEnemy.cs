using UnityEngine;
using System.Collections;
using Steer2D;

public class FlyingEnemy : MonoBehaviour {


	public Blackboard blackboard;
	public Transform target;
	private Transform currentTarget;
	private Seek seekScript;

//	public Transform player;

	public float attackRadius = 2.0f;
	public LayerMask playerLayer;

	public float evadeDistance = 5;

	void Start ()
	{
		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}
		seekScript = GetComponent<Seek>();
		seekScript.TargetPoint = new Vector2(target.position.x, target.position.y);
	}

	void Update ()
	{

		if (blackboard.playerExposed) {
			currentTarget = blackboard.player;
		} else {
			currentTarget = target;
		}

		// evading keeping above a certain height
//		if (transform.position.y <= (blackboard.skyLevel + 1)) {
//			seekScript.TargetPoint = new Vector2 ((transform.position.x + evadeDistance), blackboard.skyLevel + evadeDistance);
//		} else {
//			seekScript.TargetPoint = new Vector2 (currentTarget.position.x, currentTarget.position.y);
//		}

		seekScript.TargetPoint = new Vector2 (currentTarget.position.x, currentTarget.position.y);

		// Attacking player
		Collider2D overlapPlayer = Physics2D.OverlapCircle (transform.position, attackRadius, playerLayer);
		if (overlapPlayer != null) {
			Debug.Log ("YOU'RE DEAD!!!");
			Destroy (overlapPlayer.gameObject);
		}

	}






//	public float speed = 10;
//	public Transform ghostTower;
//
//	public Seek seekScript;
//
//	public bool goToTarget = false;
//	public Transform target;
//
//	private Vector3 direction;
//
//	public bool captured;
//
//
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update ()
//	{
//		if (goToTarget) {
////			seekScript.enabled = true;
////			seekScript.TargetPoint = new Vector2 (target.position.x, target.position.y);
//			direction = target.position - transform.position;
//		} else {
////			seekScript.enabled = false;
//			if (!captured) {
//				if (target == null) {
//					direction = ghostTower.position - transform.position;
//				} else {
//					goToTarget = true;
//				}
//			}
//		}
//
//		transform.Translate(direction.normalized * Time.deltaTime * speed);
//
//	}
//
//	void OnTriggerEnter2D (Collider2D col)
//	{
//		if (col.CompareTag ("NPC")) {
//			Debug.Log ("Hit NPC.......");
//		} else if (col.CompareTag ("AirCapture")) {
//			captured = true;
//			goToTarget = false;
//			direction = Mathf.Sign(direction.x) * Vector3.right;
//			Debug.Log("Entered flying trappppp: " + direction);
//		}
//	}
//
//	void OnTriggerExit2D (Collider2D col)
//	{
//		if (col.CompareTag ("AirCapture")) {
//			captured = false;
//			direction *= -1;
//		}
//	}

}
