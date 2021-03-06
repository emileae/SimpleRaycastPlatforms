﻿using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class Altar : MonoBehaviour {

	private Blackboard blackboard;

	// Different structures do different things
	// type 0 -> defense against ghosts... prevents the item eating ghosts from spawning there
	// type 1 -> medical area ... units restore their hp there
	// type 2 -> 
	public int structureType = 0;

	// models for each structure
	public GameObject blankModel;
	public GameObject activeModel;

	// Specific paramaters for each structure
	public float healTime = 2.0f;// for medical struture

	public bool active;
	public Platform platformScript;
	private Animator anim;

	// Regulate the build
	public bool needToBuild = false;
	public float callWaitTime = 5.0f;
	private bool building = false;
	private float progressToBuild = 10;// seconds using Time.deltaTime to build
	private float currentBuildProgress = 0;

	public NPC currentBuilder;

	// Use this for initialization
	void Start ()
	{
		blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		anim = GetComponent<Animator>();
	}

	// TODO: make sure that building stops if NPC is removed from building...
	void Update ()
	{
		if (building) {
			if (currentBuilder.gameObject.activeSelf) {
				currentBuildProgress += Time.deltaTime;
				if (currentBuildProgress >= progressToBuild) {
					building = false;
					FinishBuild ();
				}
			} else {
				currentBuilder = null;
				building = false;
			}
		}
	}

	public void ActivateAltar(){
		needToBuild = true;
		CallBuilder();
	}

	void CallBuilder ()
	{
//		Debug.Log ("Call a builder.....");
//		Debug.Log ("Builders on platform... " + platformScript.builders.Count);
		if (platformScript.builders.Count > 0) {
			platformScript.builders [0].GetComponent<NPC> ().GoToBuildSite (transform);
			Debug.Log("Tell builder to go to build site");
		}

		if (platformScript.builders.Count <= 0 && platformScript.averageJoes.Count <= 0) {
			StartCoroutine(RepeatCallBuilder());
		}

	}

	IEnumerator RepeatCallBuilder()
	{
		yield return new WaitForSeconds(callWaitTime);
		Debug.Log("Call builder again.....");
		CallBuilder();
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		
		if (col.CompareTag ("NPC")) {
			Debug.Log ("NPC entered altar trigger");
			NPC npcScript = col.gameObject.GetComponent<NPC> ();
			if (npcScript.npcType == 2) {
				Debug.Log ("Builder entered trigger Altar.cs....");
			}
			if (needToBuild && currentBuilder == null) {
				// only builders can build the Altars... or special structures...
				if (platformScript.builders.Count > 0) {
					if (npcScript.npcType == 2) {
						currentBuilder = npcScript;
						Build ();
					}
				}
			}
		}

		if (col.CompareTag ("Player")) {
			switch (structureType) {
				case 0:
					Debug.Log ("Defense structure???");
					break;
				case 1:
					Debug.Log ("Medical structure");
					break;
				case 2:
					Debug.Log ("House structure");
					if (active) {
						blackboard.playerExposed = false;
					}
					break;
				default:
					Debug.Log("fall through switch ALtar.cs ontriggerenter");
					break;
			}
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("NPC")) {
			NPC npcScript = col.gameObject.GetComponent<NPC> ();
			if (npcScript == currentBuilder) {
				currentBuilder = null;
				building = false;
			}
		}

		if (col.CompareTag ("Player")) {
			switch (structureType) {
				case 0:
					Debug.Log("Defense structure???");
					break;
				case 1:
					Debug.Log("Medical structure");
					break;
				case 2:
					Debug.Log("House structure");
					blackboard.playerExposed = true;
					break;
				default:
					Debug.Log("fall through switch ALtar.cs ontriggerenter");
					break;
			}
		}
	}

	void Build ()
	{
		building = true;
		currentBuilder.StopToBuild();
		Debug.Log("Start building");
	}
	void FinishBuild ()
	{
		building = false;
		needToBuild = false;
		currentBuildProgress = 0;
		currentBuilder.FinishBuild ();// tell NPC to move in opposite direction to the build direction

		// here is the custom build logic.....
		active = true;
//		anim.SetBool("active", active);
		blankModel.SetActive(false);
		activeModel.SetActive(true);
		switch (structureType) {
			case 0:
				Debug.Log("Ghosts wont spawn here");
				break;
			case 1:
				Debug.Log("This is now a medical structure");
				break;
			case 2:
				Debug.Log("This is now a house");
				break;
			default:
				Debug.Log("Fall through switch statement altar.cs");
				break;
		}
		currentBuilder = null;
	}


}
