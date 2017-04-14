using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	// Sea parameters
	[HideInInspector]
	public float seaLevel = 0.0f;
	public bool playerInSea = false;
	public Transform player;

	// Ghost tower parameters
	public GameObject ghostTower;
	public Bounds ghostTowerBounds;

	// fish random schooling
	public Transform fishSeekPoint;

	void Start ()
	{
		InvokeRepeating("MoveFishSeekPoint", 1.0f, 3.0f);
	}

	public void AddToWorkList ()
	{
		Debug.Log("blank add to work list function");
	}

	void MoveFishSeekPoint(){
		fishSeekPoint.position = new Vector3(Random.Range(player.position.x - 100, player.position.x + 100), Random.Range(0, -100f), fishSeekPoint.position.z);
	}

}
