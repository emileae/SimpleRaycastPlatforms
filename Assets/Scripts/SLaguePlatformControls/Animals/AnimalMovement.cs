using UnityEngine;
using System.Collections;

public class AnimalMovement : MonoBehaviour {

	public Sprite aliveSprite;
	public Sprite deadSprite;

	private SpriteRenderer sprite;
	private Bounds spriteBounds;

	// collider
	private BoxCollider2D collider;

	// Platform aware, knows where home is
	[HideInInspector]
	public Platform platformScript;

	// pickup item scripts
	private Item itemScript;
	private PickUp pickupScript;

	public float idleSpeed = 3;
	public float stopSpeed = 0;
	public float runSpeed = 25;
	private float speed;
	private Vector3 velocity = Vector3.zero;

	// wander movement
	private bool wandering = false;
	public bool fleeing = false;
	public bool fleeFromEdge =false;// is animal fleeing and moving away form an edge
	public float fleeFromEdgeTotalTime = 0.3f;// time to flee form edge
	public float fleeFromEdgeTime = 0.0f;// time elapsed fleeing form edge

	// danger detection
	public float dangerRadius = 3.0f;

	// Hunting
	private bool dead = false;

	public LayerMask enemyLayer;
	public LayerMask npcLayer;
	public LayerMask playerLayer;
	public LayerMask itemLayer;// dead animal turns into an item

	public bool stopForPlayer = false;
	public bool stopForNPC = false;
	public int direction = -1;

	private bool facingRight = true;

	// trigger trackers
	private bool changingDirection = false;



	void Start () {
		sprite = GetComponent<SpriteRenderer>();
		spriteBounds = sprite.bounds;

		sprite.sprite = aliveSprite;

		collider = GetComponent<BoxCollider2D>();

		// get item/pickup scripts
		itemScript = GetComponent<Item>();
		pickupScript = GetComponent<PickUp>();

		StartCoroutine(Hesitate());
		wandering = true;
	}

	void Update ()
	{

		velocity = new Vector3 (direction * speed, 0, 0);

		if (!wandering && !dead) {
			wandering = true;
			StartCoroutine (Hesitate ());
		}
		// Attacking animal
		Collider2D overlapEnemy = Physics2D.OverlapCircle (transform.position, dangerRadius, enemyLayer);
		Collider2D overlapNPC = Physics2D.OverlapCircle (transform.position, dangerRadius, npcLayer);
		if (overlapEnemy != null || overlapNPC != null && !dead) {
			if (fleeFromEdge) {
				fleeFromEdgeTime += Time.deltaTime;
				if (fleeFromEdgeTime >= fleeFromEdgeTotalTime) {
					fleeFromEdge = false;
				}
			}
			if (!fleeing && !fleeFromEdge) {
				Debug.Log ("FLEE!!!");
				if (overlapEnemy != null) {
					Flee (overlapEnemy.transform);
				}else if (overlapNPC != null){
					Flee (overlapNPC.transform);
				}
			}
		} else {
			fleeing = false;
		}

		if (velocity.x > 0) {
			sprite.flipX = true;
		} else if (velocity.x < 0) {
			sprite.flipX = false;
		}

		transform.Translate(velocity * Time.deltaTime);

	}

	void OnTriggerEnter2D (Collider2D col)
	{

		if (col.CompareTag ("Edge")) {
			if (!changingDirection) {
				direction *= -1;
				changingDirection = true;
				fleeFromEdge = true;
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
	}

	IEnumerator Hesitate ()
	{
		yield return new WaitForSeconds (5.0f);// wander for 5 seconds before hesitating
		speed = stopSpeed;
		if (!dead) {
			StartCoroutine (Wander ());
		}
	}
	IEnumerator Wander ()
	{
		yield return new WaitForSeconds(1.0f);// hesitate for 1 second before moving again
		speed = idleSpeed;
		if (Random.Range (0f, 1f) > 0.5f) {
			direction = 1;
		} else {
			direction = -1;
		}
		wandering = false;
	}

	void Flee (Transform fleeFrom)
	{
		fleeing = true;
		speed = runSpeed;
		if (fleeFrom.position.x > transform.position.x) {
			direction = -1;
		}else if (fleeFrom.position.x < transform.position.x){
			direction = 1;
		}
	}

	public void Die ()
	{
		dead = true;
		direction = 0;
		speed = stopSpeed;
//		sprite.flipY = true;
//		transform.position = new Vector3(transform.position.x, transform.position.y + spriteBounds.extents.y, transform.position.z);

		sprite.sprite = deadSprite;

		// convert box collider to a trigger so that player can detect to pick it up
		collider.isTrigger = true;

		// TODO: use a layermask with this
		// change the layer so that NPC don't keep hunting a dead animal
		//gameObject.layer = LayerMask.NameToLayer("Item");
		gameObject.layer = 10;

		// drops a coin
//		if (platformScript.coins.Count > 0) {
//			Debug.Log("Animal drops a coin");
//			Vector3 coinFallPosition = new Vector3(transform.position.x + spriteBounds.extents.x, transform.position.y, transform.position.z);
//			platformScript.FindCoin (coinFallPosition);
//		}s

		itemScript.enabled = true;
		pickupScript.enabled = true;

	}



}