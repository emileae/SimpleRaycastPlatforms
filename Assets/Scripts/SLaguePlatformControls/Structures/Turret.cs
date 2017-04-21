using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Turret : MonoBehaviour {

	public bool active = false;

	public bool manned;
	public NPC npcGunner;

	private Fighter fighterScript;

	private bool reloading = false;
	private float reloadTimeElapsed = 0.0f;
	public float reloadTime = 1.0f;
	public float turretRange = 60.0f;
	public LayerMask enemyLayer;

	public Structure structureScript;

	// Use this for initialization
	void Start () {
		structureScript = GetComponent<Structure>();
	}

	void Update ()
	{
		if (manned) {
			if (structureScript.active) {
				Debug.Log ("Ready to fire at enemies...");
				Collider2D enemy = Physics2D.OverlapCircle (transform.position, turretRange, enemyLayer);
				if (enemy != null && !reloading) {
					Debug.Log ("Fire at enemy");
					Attack attackScript = enemy.GetComponent<Attack> ();
					attackScript.Hit ();
					reloading = true;
				}

				if (reloading) {
					reloadTimeElapsed += Time.deltaTime;
					if (reloadTimeElapsed >= reloadTime) {
						reloading = false;
						reloadTimeElapsed = 0.0f;
					}
				}
			} else {
				Unman();
			}
		}
	}
	
	public void ActivateStructure (Platform structurePlatformScript)
	{
		Debug.Log("Activated turret....");
		active = true;
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (!manned && active) {
			if (col.CompareTag ("NPC")) {
				fighterScript = col.gameObject.GetComponent<Fighter>();
				if (fighterScript != null){
					Debug.Log("Fighter has entered unmanned turret...");
					fighterScript.ManTurret(this);
					manned = true;
				}
			}
		}
	}

	void Unman(){
		manned = false;
		fighterScript.UnmanTurret(this);
	}

//	void OnDrawGizmosSelected()
//    {
//        // Display the explosion radius when selected
//        Gizmos.color = new Color(1, 1, 0, 0.75F);
//        Gizmos.DrawSphere(transform.position, turretRange);
//    }

}
