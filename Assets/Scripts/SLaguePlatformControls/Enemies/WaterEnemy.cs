using UnityEngine;
using System.Collections;
using Steer2D;

public class WaterEnemy : MonoBehaviour {

	public Blackboard blackboard;
	public Transform target;
	private Seek seekScript;

	public Transform player;

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

		if (blackboard.playerInSea) {
			target = player;
		}

		if (transform.position.y >= (blackboard.seaLevel - 1)) {
			seekScript.TargetPoint = new Vector2 ((transform.position.x + evadeDistance), blackboard.seaLevel - evadeDistance);
		} else {
			seekScript.TargetPoint = new Vector2 (target.position.x, target.position.y);
		}

	}

}
