using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour {

	// WORK LIST
	// - a list ofunoccupied work structures that need manning
	public float workerCallTime = 1.0f;
	private bool callingWorkers = false;
	public List<GameObject> workList = new List<GameObject>();

	public List<GameObject> npcs = new List<GameObject>();
	public List<NPC> npcScripts = new List<NPC>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddToWorkList (GameObject go){
		workList.Add(go);
	}

	// ocnvert this to direct trigger when using sprites
//	public void StopNearestNPC (GameObject player)
//	{
//
//		Debug.Log("HALT!!");
//
//		bool foundAvailableNPC = false;
//		float npcDistance = Mathf.Infinity;
//		int nearestNPCIndex = 0;
//
//		for (int i = 0; i < npcs.Count; i++) {
//			if (!npcScripts [i].purchased) {
//				Vector3 offset = player.transform.position - npcs [i].transform.position;
//				float sqrMagDistance = offset.sqrMagnitude;
//				if (sqrMagDistance < npcDistance) {
//					npcDistance = sqrMagDistance;
//					nearestNPCIndex = i;
//					foundAvailableNPC = true;
//				}
//			}
//		}
//
//		if (foundAvailableNPC) {
//			PlayerController playerScript = player.GetComponent<PlayerController>();
//			playerScript.npcPayScript = npcScripts [nearestNPCIndex];
//			npcScripts [nearestNPCIndex].StopForPlayer ();
//		}
//	}
}
