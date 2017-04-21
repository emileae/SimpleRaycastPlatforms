using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

	private Blackboard blackboard;

	public LayerMask passengerMask;

	public Vector3[] localWaypoints;
	private Vector3[] globalWaypoints;

	public bool atEndPoint = false;
	public bool atStartPoint = false;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0, 2)]// this will clamp a between 1 & 3 --> a+ 1
	public float easeFactor;

	private int fromWaypointIndex;
	private float percentBetweenWaypoints;
	private float nextMoveTime;

	private List<PassengerMovement> passengerMovement;
	private Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
	private PackageController2D boxPackageController;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		SetGlobalWaypoints();
		blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
	}

	public void SetGlobalWaypoints()
	{
		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++) {
			// Sebastian's old code for manually setting the waypoints.. takes
//			globalWaypoints[i] = localWaypoints[i] + transform.position;
			globalWaypoints[i] = localWaypoints[i];
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRaycastOrigins();

		Vector3 velocity = CalculatePlatformMovement();

		CalculatePassengerMovement(velocity);
		MovePassengers(true);
		transform.Translate(velocity);
		MovePassengers(false);
	}

	float Ease (float x)
	{
		float a = easeFactor + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1-x, a));
	}

	Vector3 CalculatePlatformMovement ()
	{

		// the delay time
		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		// keep tracked indices within array length
		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);


		// indicate start/end points
		if (percentBetweenWaypoints < 1) {
			atEndPoint = false;// reset this so that its not registered as being at the end pointall the way
		}
		if (percentBetweenWaypoints > 0) {
			atStartPoint = false;
		}

		if (percentBetweenWaypoints >= 1) {
			if (fromWaypointIndex == 0) {
				Debug.Log("Hit end point");
				atEndPoint = true;
			}else if (fromWaypointIndex == (globalWaypoints.Length-1)){
				Debug.Log("Hit start point");
				atStartPoint = true;
			}

			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length - 1) {
					fromWaypointIndex = 0;
					System.Array.Reverse (globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}
		return newPos - transform.position;

	}

	void MovePassengers (bool beforeMovePlatform)
	{
		foreach (PassengerMovement passenger in passengerMovement) {
			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				if (passenger.transform != blackboard.box) {
					if (!passengerDictionary.ContainsKey (passenger.transform)) {
						passengerDictionary.Add (passenger.transform, passenger.transform.GetComponent<Controller2D> ());
					}

					// goal is to use the controller2D to move passengers so that they detect collisions
					passengerDictionary [passenger.transform].Move (passenger.velocity, passenger.standingOnPlatform);
				} else if (passenger.transform == blackboard.box) {
					PackageController2D boxPackageController = passenger.transform.GetComponent<PackageController2D>();
					Debug.Log("Calling Moving vertically - B");
					boxPackageController.Move(passenger.velocity, passenger.standingOnPlatform);
				}
			}
		}
	}

	// a passenger is any controller2D on moving platform
	void CalculatePassengerMovement (Vector3 velocity)
	{
		// keep track of passenger movement to know if passenger should be moved before or after platform
		passengerMovement = new List<PassengerMovement>();

		// keep a list of all passengers on platform (maybe enemies or AI units)
		// so that we don't move a unit multiple times, once translated by 1 raycast hit then stop
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		// vertically moving platform
		if (velocity.y != 0) {
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;
			for (int i = 0; i < verticalRayCount; i++) {
			
				// if moving up cast ray from top, if moving down cast rays from bottom
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
					}
					float pushX = (directionY == 1) ? velocity.x : 0;
					float pushY = velocity.y - (hit.distance - skinWidth) * directionY;


					//hit.transform.Translate (new Vector3 (pushX, pushY));
					passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
				}

			}
		}

		// horizontally moving platform
		if (velocity.x != 0) {
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
			for (int i = 0; i < horizontalRayCount; i++) {
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (hit) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
					}
					float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
					float pushY = skinWidth;// add a small downward force so that passenger knows they're on the ground and so can still jump
//					hit.transform.Translate (new Vector3 (pushX, pushY));
					passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
				}

			}
		}

		// Passenger on top of a horizontally or downward moving platofmr
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0) {
			float rayLength = skinWidth * 2;
			for (int i = 0; i < verticalRayCount; i++) {
			
				// if moving up cast ray from top, if moving down cast rays from bottom
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, rayLength, passengerMask);

				if (hit) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
					}
					float pushX = velocity.x;
					float pushY = velocity.y;
//					hit.transform.Translate (new Vector3 (pushX, pushY));
					passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
				}

			}
		}
	}

	struct PassengerMovement {
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement( Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform){
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;

		}
	}

	void OnDrawGizmos ()
	{
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = 1.0f;
			for (int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying)? globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos  - Vector3.up * size, globalWaypointPos  + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos  - Vector3.right * size, globalWaypointPos  + Vector3.right* size);
			}
		}
	}

}
