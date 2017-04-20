﻿using UnityEngine;
using System.Collections;

public class Structure : MonoBehaviour {

	public Platform platformScript;

	// structure specific parameters
	public int cost = 3;
	public float workTime = 2.0f;
	public float degradeTime = 50.0f;

	public bool active = false;
	private PlayerInteractions playerInteractions;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerInteractions = col.gameObject.GetComponent<PlayerInteractions>();
			playerInteractions.structureScript = this;
		}
	}
	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			playerInteractions = col.gameObject.GetComponent<PlayerInteractions> ();
			if (playerInteractions.structureScript == this) {
				playerInteractions.structureScript = null;
			}
		}
	}

	public void ToggleActivation ()
	{
		active = !active;
		if (active) {
			AddToBuildList ();
		} else {
			RemoveFromBuildList();
		}
	}

	public void PayStructure ()
	{
		cost--;
		if (cost <= 0) {
			Debug.Log("Paid for structure... now activate");
		}
	}

	void AddToBuildList ()
	{
		if (platformScript.builders.Count > 0) {
			for (int i = 0; i < platformScript.builders.Count; i++) {
				Builder builderScript = platformScript.builders[i].GetComponent<Builder>();
				builderScript.structures.Add(this);
			}
		}
	}

	void RemoveFromBuildList(){
		if (platformScript.builders.Count > 0) {
			for (int i = 0; i < platformScript.builders.Count; i++) {
				Builder builderScript = platformScript.builders[i].GetComponent<Builder>();
				builderScript.structures.Remove(this);
			}
		}
	}

}