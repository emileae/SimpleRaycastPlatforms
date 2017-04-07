using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour {

	[Range(-1, 1)]
	public int facingDirection;
	public GameObject movingPlatform;

	public void ActivatePayment(){
		GameObject movingPlatformObj = Instantiate(movingPlatform) as GameObject;
		MovingPlatform movingPlatformScript = movingPlatformObj.GetComponent<MovingPlatform>();
		movingPlatformScript.edgeTransform = transform;
		Debug.Log("edge transform position: " + transform.position);
		movingPlatformScript.edgeFacingDirection = facingDirection;
		movingPlatformScript.InitialisePlatform ();
	}
}
