using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	// Sea parameters
	[HideInInspector]
	public float seaLevel = 0.0f;

	// Ghost tower parameters
	public GameObject ghostTower;
	public Bounds ghostTowerBounds;

	public void AddToWorkList ()
	{
		Debug.Log("blank add to work list function");
	}

}
