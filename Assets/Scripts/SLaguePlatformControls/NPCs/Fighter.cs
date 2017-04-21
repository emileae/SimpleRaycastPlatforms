using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour {

	private NPCController controller;

	public Turret turretScript;

	// Use this for initialization
	void Start () {
		controller = GetComponent<NPCController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ManTurret (Turret availableTurretScript)
	{
		if (availableTurretScript.structureScript.active) {
			turretScript = availableTurretScript;
			controller.direction = 0;
			Debug.Log ("I have manned a turret.");
		}
	}

	public void UnmanTurret (Turret mannedTurretScript)
	{
		if (turretScript == mannedTurretScript) {
			turretScript = null;
			if (Random.Range (0, 1) > 0.5f) {
				controller.direction = -1;
			} else {
				controller.direction = 1;
			}
			Debug.Log ("I have left a turret.");
		}
	}

//	void OnTriggerEnter(Collider2D col){
//		if (col.CompareTag ("Platform")) {
//			Platform platformScript = col.gameObject.GetComponent<Platform>();
//			platformScript.fighters.Add(gameObject);
//		}
//	}
}
