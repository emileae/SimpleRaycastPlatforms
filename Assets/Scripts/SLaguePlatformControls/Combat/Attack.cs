using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {


	public NPC npcScript;
	public Enemy enemyScript;
	public AnimalMovement animalScript;

	public void GetHit ()
	{
		Debug.Log ("Got hit: " + gameObject.name);

		if (npcScript != null) {
			npcScript.Die ();
		} else if (enemyScript != null) {
			enemyScript.Die ();
		} else if (animalScript != null) {
			animalScript.Die();
		}

	}

	public void DidHit ()
	{
		Debug.Log("Did hit: " + gameObject.name);
	}

}
