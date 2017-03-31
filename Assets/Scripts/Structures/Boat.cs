using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			PlayerController playerScript = col.gameObject.GetComponent<PlayerController>();
			playerScript.boat = gameObject;
			playerScript.boatRbody = gameObject.GetComponent<Rigidbody2D>();
			playerScript.onBoat = true;
		}
	}
}
