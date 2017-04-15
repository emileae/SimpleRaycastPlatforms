using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround(Camera.main.transform.position, Vector3.forward, -2 * Time.deltaTime);
	}
}
