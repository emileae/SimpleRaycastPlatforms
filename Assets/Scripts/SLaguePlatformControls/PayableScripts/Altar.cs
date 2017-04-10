using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class Altar : MonoBehaviour {

	public bool active;
	public Platform platformScript;
	private Animator anim;

	// Regulate the build
	private bool needToBuild = false;
	public float callWaitTime = 5.0f;
	private bool building = false;
	private float progressToBuild = 10;// seconds using Time.deltaTime to build
	private float currentBuildProgress = 0;

	private NPC currentBuilder;

	// Use this for initialization
	void Start ()
	{
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
		Debug.Log ("Call a builder.....");
		Debug.Log ("Builders on platform... " + platformScript.builders.Count);
		if (platformScript.builders.Count > 0) {
			platformScript.builders [0].GetComponent<NPC> ().GoToBuildSite (transform);
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
			NPC npcScript = col.gameObject.GetComponent<NPC> ();

			if (needToBuild && currentBuilder == null) {
				// only builders can build the Altars... or special builds...
				if (platformScript.builders.Count > 0) {
					if (npcScript.npcType == 2) {
						currentBuilder = npcScript;
						Build ();
					}
				}
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
	}

	void Build ()
	{
		building = true;
		currentBuilder.StopToBuild();
		Debug.Log("Start building");
	}
	void FinishBuild ()
	{
		needToBuild = false;
		currentBuildProgress = 0;
		currentBuilder.FinishBuild();// tell NPC to move in opposite direction to the build direction

		// here is the custom build logic.....
		active = true;
		anim.SetBool("active", active);

		currentBuilder = null;
	}


}
