using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public Blackboard blackboard;

	public float maxSpeed = 10f;

	private bool facingRight = true;
	private Animator anim;
	private Rigidbody2D rBody;

	// jumping/falling
//	private bool climbing = false;
//	public Transform climbCheck;
//	private float climbRadius = 0.2f;
//	public LayerMask whatIsClimbable;
//	public float releaseForce = 100;

	private bool grounded = false;
	public Transform groundCheck;
	private float groundRadius = 0.2f;
	public LayerMask whatIsGround;
	public float jumpForce = 300;

	// Inventory
		// inventory ui
	public List<GameObject> coinInventoryUI = new List<GameObject>();
	public List<GameObject> inventoryUI = new List<GameObject>();

	public GameObject pickupableItem;
	public int coinInventorySize = 5;
	public int inventorySize = 3;

	public List<GameObject> coinInventory = new List<GameObject>();
	public List<GameObject> inventory = new List<GameObject>();
		// inventory prefabs
	public GameObject inventoryCoin;
	public GameObject inventoryNPC;
	public Sprite uiEmptySprite;
	public Sprite uiCoinSprite;

	// Paying
	public GameObject coinPrefab;
	private bool passingCurrency = false;
	public int initialCurrency = 3;
	public int currency = 0;
	public GameObject payTarget = null;

	// Boat
	public bool onBoat = false;
	public GameObject boat = null;
	public Rigidbody2D boatRbody;
	private float boatXVelocity;

	// ladders
	public float climbSpeed = 3.0f;
	public bool overLadder = false;

	// Paying an NPC to employ them
	public PayController payScript;

	// Use this for initialization
	void Start ()
	{
		rBody = GetComponent<Rigidbody2D> ();

		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}

		InitializeCoinInventory();

//		Physics2D.IgnoreLayerCollision(8, 9, true);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{

		// check if grounded
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGround);

		float move = Input.GetAxisRaw ("Horizontal");
		float inputV = Input.GetAxisRaw ("Vertical");

		if (onBoat) {
//			rBody.velocity = new Vector2 (move * maxSpeed + boatXVelocity, rBody.velocity.y);
			rBody.isKinematic = true;
			boatRbody.velocity = new Vector2 (move * maxSpeed, rBody.velocity.y);
//			rBody.position = boat.transform.position;
		} else {
			if (overLadder) {
				Debug.Log ("Climb");
				if (inputV == 0) {
//					rBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
				} else {
//					rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
					rBody.velocity = new Vector2 (move * maxSpeed, inputV * climbSpeed);
				}
				rBody.velocity = new Vector2 (move * maxSpeed, inputV * climbSpeed);

			} else {
//				rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
				rBody.velocity = new Vector2 (move * maxSpeed, rBody.velocity.y);
			}
		}

		// prevent sliding
//		if (move == 0) {
//			rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
//		} else {
//			rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
//		}


		if (move > 0 && !facingRight) {
			Flip ();
		} else if (move < 0 && facingRight) {
			Flip ();
		}
	}

	// put inputs here because if we use Fixedupdate, we may miss an input
	void Update ()
	{
		if (grounded && Input.GetButtonDown ("Jump")) {
			rBody.AddForce (new Vector2 (0, jumpForce));
			Debug.Log ("Jump!!");
		}

		float inputV = Input.GetAxisRaw ("Vertical");
//		float inputH = Input.GetAxisRaw ("Horizontal");

		if (payScript != null && !overLadder) {
			if (inputV < 0) {
				//				payScript.StopForPlayer();
				Pay ();
			}
		}



		// call NPCs
		bool action = Input.GetButton ("Fire3");
		bool actionButtonDown = Input.GetButtonDown ("Fire3");

		if (pickupableItem != null && actionButtonDown) {
			Debug.Log("PICK UP");
			PickUpItem ();
		}else if (pickupableItem == null && actionButtonDown) {
			Debug.Log("DROP OFF");
			DropOffItem();
		}

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Ladder")) {
			overLadder = true;
		}
	}
	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Ladder")) {
			overLadder = false;
		}
	}

	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void Pay ()
	{
		Debug.Log ("passingCurrency? " + passingCurrency);
		if (!passingCurrency) {
			if (coinInventory.Count >= (payScript.cost - payScript.amountPaid)) {
				passingCurrency = true;
				Debug.Log ("Pay.....");
				StartCoroutine (PassCoin ());
			} else {
				Debug.Log("Not Enough money!!!!!!!!");
			}
		}
	}

	IEnumerator PassCoin ()
	{
		yield return new WaitForSeconds (0.2f);
		currency -= 1;
		// first remove ui in last position
		Image uiImage = coinInventoryUI[coinInventory.Count-1].GetComponent<Image>();
		uiImage.sprite = uiEmptySprite;
		// remove last actual coin object
		coinInventory.RemoveAt(coinInventory.Count-1);
		bool purchased = payScript.Pay ();
		passingCurrency = false;
		if (purchased && payScript.pickupable) {
			PickUpItem ();
		}
	}

	public void ReturnPayment(int returnedCurrency){
		currency += returnedCurrency;
		GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity) as GameObject;
		coinInventory.Add (coin);
		coin.SetActive(false);
		Image uiImage = coinInventoryUI[coinInventory.Count-1].GetComponent<Image>();
		uiImage.sprite = uiCoinSprite;
	}

	void PickUpItem ()
	{
		PickUp pickupScript = pickupableItem.GetComponent<PickUp> ();
		 switch(pickupScript.generalType) {
		 	case 0:
				if (inventory.Count < inventorySize) {
					pickupScript.PickUpItem ();
					inventory.Add (pickupableItem);
					Image uiImage = inventoryUI [inventory.Count - 1].GetComponent<Image> ();
					uiImage.sprite = uiCoinSprite;
					pickupableItem = null;
				} else {
					Debug.Log ("Inventory Full!!!!");
				}
				break;
			case 1:
				if (coinInventory.Count < coinInventorySize) {
					pickupScript.PickUpItem ();
					coinInventory.Add (pickupableItem);
					Image uiImage = coinInventoryUI [coinInventory.Count - 1].GetComponent<Image> ();
					uiImage.sprite = uiCoinSprite;
					pickupableItem = null;
				} else {
					Debug.Log ("Inventory - money Full!!!!");
				}
				break;
			default:
				Debug.Log("Switch fall through PickUpItem PlayerController.cs");
				break;
		}
	}
	void DropOffItem ()
	{
		Debug.Log("Try to drop off item...");
		if (inventory.Count > 0) {
			GameObject item = inventory[inventory.Count-1];// last item in list
			item.SetActive(true);
			item.transform.position = transform.position;
			PickUp pickupScript = item.GetComponent<PickUp>();
			inventory.Remove(item);
			Image uiImage = inventoryUI[inventory.Count].GetComponent<Image>();
			uiImage.sprite = uiEmptySprite;
			pickupScript.DroppOffItem();
		} else {
			Debug.Log("Nothing in INVENTORY!!!!");
		}
	}

	void InitializeCoinInventory ()
	{
		for (int i = 0; i < initialCurrency; i++) {
			currency += 1;
			GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity) as GameObject;
			coinInventory.Add (coin);
			coin.SetActive(false);
			Image uiImage = coinInventoryUI[coinInventory.Count-1].GetComponent<Image>();
			uiImage.sprite = uiCoinSprite;
		}
	}

	public void KeepPlayerOnBoat(float xVelocity){
		onBoat = true;
		boatXVelocity = xVelocity;
	}

	public void FreezePlayerX(){
		rBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
	}

	public void UnFreezePlayerX(){
		rBody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

}