using UnityEngine;
using System.Collections;
using Steer2D;

public class FlyingEnemy : MonoBehaviour {

	public float speed = 0.5f;
	public Transform ghostTower;

	public Seek seekScript;

	public bool goToTarget = false;
	public Transform target;

	private Vector3 direction;

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
			direction = ghostTower.position - transform.position;
		}

		transform.Translate(direction * Time.deltaTime * speed);

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			Debug.Log("Hit NPC.......");
		}
	}

}
