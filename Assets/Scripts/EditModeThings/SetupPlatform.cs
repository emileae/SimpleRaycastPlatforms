using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetupPlatform : MonoBehaviour {

	private Bounds platformbounds;

	public GameObject trigger;
	private BoxCollider2D triggerCollider;
	public GameObject edgeL;
	public GameObject edgeR;
	public GameObject ladder;

	// Use this for initialization
	void Start () {

//		platformbounds = GetComponent<BoxCollider2D>().bounds;
//		triggerCollider = trigger.GetComponent<BoxCollider2D>();

	}

	// Update is called once per frame
	void Update () {


		platformbounds = GetComponent<BoxCollider2D>().bounds;
		triggerCollider = trigger.GetComponent<BoxCollider2D>();
		trigger.GetComponent<BoxCollider2D>().size = new Vector2(1.0f, 1.0f);
//		trigger.transform.localScale += transform.localScale;

		Debug.Log("platformbounds: " + (new Vector2(platformbounds.size.x, platformbounds.size.y)));
		Debug.Log("triggerCollider.size: " + triggerCollider.size);

		triggerCollider.size = new Vector2(platformbounds.size.x, platformbounds.size.y);

		trigger.transform.position = new Vector3(transform.position.x, transform.position.y + triggerCollider.bounds.extents.y + platformbounds.extents.y, transform.position.z);
		edgeL.transform.position = new Vector3(platformbounds.min.x + edgeL.GetComponent<BoxCollider2D>().bounds.extents.x, transform.position.y, transform.position.z);
		edgeR.transform.position = new Vector3(platformbounds.max.x - edgeR.GetComponent<BoxCollider2D>().bounds.extents.x, transform.position.y, transform.position.z);


	}
}
