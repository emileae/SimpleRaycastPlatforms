using UnityEngine;
using System.Collections;

public class SeekPoint : MonoBehaviour {

	private Blackboard blackboard;

	void Start ()
	{
		blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Enemy")) {
			blackboard.MoveSeekPoints();
		}
	}
}
