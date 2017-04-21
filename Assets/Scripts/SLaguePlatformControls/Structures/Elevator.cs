using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour {

	public Transform originPlatform;
	private Bounds originPlatformBounds;
	public Transform destinationPlatform;
	private Bounds destinationPlatformBounds;

	private BoxCollider2D collider;
	private Bounds elevatorBaseBounds;

	public GameObject elevatorPrefab;

	private Structure scructureScript;

	void Start(){
		collider = GetComponent<BoxCollider2D>();
		elevatorBaseBounds = collider.bounds;
		scructureScript = GetComponent<Structure>();

		originPlatformBounds = originPlatform.GetComponent<BoxCollider2D>().bounds;
		destinationPlatformBounds = destinationPlatform.GetComponent<BoxCollider2D>().bounds;
	}

	public void ActivateStructure (Platform structurePlatformScript)
	{

		Vector3[] waypoints = new Vector3[2];

		// Up / Down
		if (scructureScript.structureType == 1 || scructureScript.structureType == 2){
			waypoints[0] = new Vector3(transform.position.x - elevatorBaseBounds.extents.x, originPlatform.position.y, transform.position.z);//originPlatform.position;
			waypoints[1] = new Vector3(waypoints[0].x, destinationPlatform.position.y, waypoints[0].z);

			GameObject elevator = (GameObject)Instantiate(elevatorPrefab, transform.position, Quaternion.identity);

			PlatformController elevatorControllerScript = elevator.GetComponent<PlatformController>();
			elevatorControllerScript.localWaypoints = waypoints;
		
		}
		// Move right...
		else if (scructureScript.structureType == 3){

			GameObject elevator = (GameObject)Instantiate(elevatorPrefab, transform.position, Quaternion.identity);

			Bounds elevatorBounds = elevator.GetComponent<BoxCollider2D>().bounds;

			waypoints[0] = new Vector3(originPlatform.position.x + originPlatformBounds.extents.x  + elevatorBounds.extents.x, originPlatform.position.y + originPlatformBounds.extents.y, originPlatform.position.z);//originPlatform.position;
			waypoints[1] = new Vector3(destinationPlatform.position.x - destinationPlatformBounds.extents.x - elevatorBounds.extents.x, destinationPlatform.position.y + originPlatformBounds.extents.y, destinationPlatform.position.z);

			PlatformController elevatorControllerScript = elevator.GetComponent<PlatformController>();
			elevatorControllerScript.localWaypoints = waypoints;

		}

	}
}
