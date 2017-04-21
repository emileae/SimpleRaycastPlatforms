using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

	public int hp = 10;
	public NPC npcScript;
	public Enemy enemyScript;
	public AnimalMovement animalScript;

	public void Hit ()
	{
		Debug.Log ("Got hit: " + gameObject.name);

		hp--;
		if (hp <= 0) {
			gameObject.SetActive(false);
		}

//		if (npcScript != null) {
//			npcScript.Die ();
//		} else if (enemyScript != null) {
//			enemyScript.Die ();
//		} else if (animalScript != null) {
//			animalScript.Die();
//		}

	}

	public void DidHit ()
	{
		Debug.Log("Did hit: " + gameObject.name);
	}

}
