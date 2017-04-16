using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	// Sea parameters
	[HideInInspector]
	public float seaLevel = 0.0f;
	public bool playerInSea = false;
	public Transform player;
	public Transform fishSeekPoint;// random schooling for fish

	// Sky parameters
	public float skyLevel;// based on top platform's height in islandGenerator
	public bool playeronTopPlatform = false;
	public Transform skySeekPoint;// random flocking for birds

	// Ghost tower parameters
	public GameObject ghostTower;
	public Bounds ghostTowerBounds;

	void Start ()
	{
		InvokeRepeating("MoveSeekPoints", 1.0f, 3.0f);
	}

	public void AddToWorkList ()
	{
		Debug.Log("blank add to work list function");
	}

	// TODO call this from flyingEnemy or WaterEnemy when they reach the seek point, to avoid the weird glitch behaviour
	public void MoveSeekPoints(){
//		Debug.Log("Move random target.....");
		fishSeekPoint.position = new Vector3(Random.Range(player.position.x - 200, player.position.x + 200), Random.Range(seaLevel, -100f), fishSeekPoint.position.z);
		skySeekPoint.position = new Vector3(Random.Range(player.position.x - 200, player.position.x + 200), Random.Range(skyLevel, 100f), skySeekPoint.position.z);
	}

}
