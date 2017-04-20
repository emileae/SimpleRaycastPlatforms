using UnityEngine;
using System.Collections;

public class Bridge : MonoBehaviour {

	private Platform platformScript;

	private SpriteRenderer sprite;

	public GameObject edge;
	public Edge edgeScript;

	public GameObject bridgeSectionPrefab;
	public GameObject bridgePillarPrefab;
	public GameObject bridgeEndPrefab;

	// TODO: check for another platform then terminate the bridge there... i.e. need to find the end condition
	// terminating a bridge may mean getting rid of the internal edges and then making one giant platform... hopefully that is destroyable...

	public void ActivateStructure (Platform structurePlatformScript)
	{
		platformScript = structurePlatformScript;
		Debug.Log ("Structure-specific activation..... bridge");

		// set BRidge section's position
		GameObject bridgeSection = Instantiate (bridgeSectionPrefab, transform.position, Quaternion.identity) as GameObject;
		Bounds sectionBounds = bridgeSection.GetComponent<BoxCollider2D> ().bounds;
		bridgeSection.transform.position = new Vector3 (platformScript.transform.position.x + platformScript.GetComponent<BoxCollider2D> ().bounds.extents.x, platformScript.transform.position.y, platformScript.transform.position.z);

		// set new edge position
		edge.transform.position = new Vector3(bridgeSection.transform.position.x + (edgeScript.facingDirection * sectionBounds.size.x), bridgeSection.transform.position.y, bridgeSection.transform.position.z);

		// set new build point position
		transform.position = new Vector3(bridgeSection.transform.position.x + (edgeScript.facingDirection * sectionBounds.size.x), bridgeSection.transform.position.y, bridgeSection.transform.position.z);
	}

}
