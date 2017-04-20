using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public Transform edgeTransform;
	public LayerMask collisionMask;

	public float maxMoveDistance = 100;

	public int edgeFacingDirection = 1;
	private Bounds platformBounds;
	private PlatformController platformController;

	public void InitialisePlatform () {
		platformController = GetComponent<PlatformController>();
		platformBounds = GetComponent<BoxCollider2D>().bounds;

		// horizontally moving platform
		Vector3 startLocation = new Vector3(edgeTransform.position.x, edgeTransform.position.y, edgeTransform.position.z);
		Vector3 endLocation = new Vector3(edgeTransform.position.x + (edgeFacingDirection * maxMoveDistance), edgeTransform.position.y, edgeTransform.position.z);

		Debug.Log("startLocation: " + startLocation);

		platformController.localWaypoints = new Vector3[2];
		platformController.localWaypoints[0] = startLocation;
		platformController.localWaypoints[1] = endLocation;
		platformController.SetGlobalWaypoints();
		platformController.speed = 5.0f;// guess speed
	}

	// Try to set the moving platform's destination beforehand
//	void FindNextPlatform(){
//		RaycastHit2D hit = Physics2D.Raycast (edgeTransform.position, Vector2.right * edgeFacingDirection, float.MaxValue, collisionMask);
//		if (hit) {
//				Debug.Log("Hit something");
//
//			}
//	}

}
