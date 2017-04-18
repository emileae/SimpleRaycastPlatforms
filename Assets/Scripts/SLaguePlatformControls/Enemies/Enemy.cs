using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	private EnemyController enemyController;

	// type 0 -> standard ghost
	// type 1 -> fast ghost that eats items/coins
	// type 2 -> seamonster that circles the length of
	public int enemyType = 0;
	public int hp = 3;// health points
	public int ap = 2;// attack points

	// Attacking
	public bool attacking = false;
	public NPC npcScript;
	public float attackTime = 1.0f;
	public float attackRadius = 4.0f;

	// trigger trackers
	private bool changingDirection = false;

	void Start(){
		enemyController = GetComponent<EnemyController>();
	}

	void OnTriggerEnter2D (Collider2D col)
	{

		if (col.CompareTag ("Edge")) {
			if (!changingDirection && enemyController != null) {
				enemyController.direction *= -1;
				changingDirection = true;
			}
		}

	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Edge")) {
			if (changingDirection) {
				changingDirection = false;
			}
		}

//		if (col.CompareTag ("NPC")) {
//			if (col.transform.position.x < transform.position.x) {
//				enemyController.direction = -1;
//			} else {
//				enemyController.direction = 1;
//			}
//			enemyController.stopForNPC = false;
//			npcScript = null;
//			attacking = false;// stop attack and now, maybe pursue?
//		}
	}

	public IEnumerator Attack ()
	{
		attacking = true;
		yield return new WaitForSeconds (attackTime);
		if (npcScript != null) {
			Debug.Log ("HIT NPC!!!");
			npcScript.hp -= ap;
			if (npcScript.hp <= 0) {
				Debug.Log ("Killed NPC");
				enemyController.stopForNPC = false;
				npcScript.Die ();
				npcScript = null;
			}
		}
		attacking = false;
	}

	public void Die(){
		Debug.Log("play Enemy death aniamtion");
		Destroy(gameObject);
	}

}
