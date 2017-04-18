using UnityEngine;
using System.Collections;
using UnityEditor;

public class Edge : MonoBehaviour {

	[Range(-1, 1)]
	public int facingDirection;
	public GameObject movingPlatform;
	public Platform platformScript;

	// Regulate the build
	private bool needToBuild = false;
	public float callWaitTime = 5.0f;
	private bool building = false;
	private float progressToBuild = 10;// seconds using Time.deltaTime to build
	private float currentBuildProgress = 0;

	private NPC currentBuilder;

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

	public void ActivatePayment(){
		needToBuild = true;
		CallBuilder();

	}

	void CallBuilder ()
	{
		Debug.Log ("Call a builder.....");
		Debug.Log ("Builders on platform... " + platformScript.builders.Count);
		Debug.Log ("AverageJoes on platform... " + platformScript.averageJoes.Count);
		if (platformScript.builders.Count > 0) {
			platformScript.builders [0].GetComponent<NPC> ().GoToBuildSite (transform);
		} else if (platformScript.averageJoes.Count > 0) {
			platformScript.averageJoes [0].GetComponent<NPC> ().GoToBuildSite (transform);
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
				// if its an average Joe or a builder... fighters cant build
				if (npcScript.npcType == 1 || npcScript.npcType == 2) {
					// builders take priority
					if (platformScript.builders.Count > 0) {
						if (npcScript.npcType == 2) {
							currentBuilder = npcScript;
							Build ();
						}
					} else if (platformScript.averageJoes.Count > 0 && platformScript.builders.Count <= 0) {
						if (npcScript.npcType == 1) {
							currentBuilder = npcScript;
							Build ();
						}
					}
				} else {
					npcScript.ChangeDirection();
				}
			} else {
				Debug.Log("tell NPC to change direction");
				npcScript.ChangeDirection();
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
		currentBuilder.FinishBuild(facingDirection * -1);// tell NPC to move in opposite direction to the build direction

		// here is the custom build logic.....
		Debug.Log("Finish building");
		GameObject movingPlatformObj = Instantiate(movingPlatform) as GameObject;
		MovingPlatform movingPlatformScript = movingPlatformObj.GetComponent<MovingPlatform>();
		movingPlatformScript.edgeTransform = transform;
		Debug.Log("edge transform position: " + transform.position);
		movingPlatformScript.edgeFacingDirection = facingDirection;
		movingPlatformScript.InitialisePlatform ();

		currentBuilder = null;
	}
}
