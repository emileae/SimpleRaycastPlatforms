using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemCarry : MonoBehaviour {

	public SpriteRenderer platformSprite;
	private Bounds platformBounds;

	public PlatformController controller;

	public LayerMask carryLayer;
	public float detectRadius = 10.0f;
	private List<GameObject> carriedItems = new List<GameObject>();

	// Use this for initialization
	void Start () {
		platformBounds = platformSprite.bounds;
	}

	// Update is called once per frame
	void Update ()
	{

		if (controller.atStartPoint) {
			Collider2D carriableItem = Physics2D.OverlapCircle (transform.position, detectRadius, carryLayer);
			if (carriableItem != null) {
				if (!carriedItems.Contains (carriableItem.gameObject)) {
					carriedItems.Add (carriableItem.gameObject);
				}
			}
		}

		if (carriedItems.Count > 0) {
			for (int i = 0; i < carriedItems.Count; i++) {
				carriedItems[i].transform.position = transform.position;
				if (controller.atEndPoint) {
					carriedItems.RemoveAt(i);
				}
			}
		}
	}

//	void OnDrawGizmosSelected() {
//        Gizmos.color = Color.white;
//		Gizmos.DrawWireSphere(transform.position, detectRadius);
//    }

//	void OnTriggerEnter2D (Collider2D col)
//	{
//		if (col.gameObject.layer == LayerMask.NameToLayer ("Item")) {
//			Debug.Log ("Hit item");
//			if (controller.atStartPoint) {
//				carriedItems.Add (col.gameObject);
//			}
//		}
//	}

}
