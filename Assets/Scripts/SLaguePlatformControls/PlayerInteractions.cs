using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Player))]
[RequireComponent (typeof(Controller2D))]
public class PlayerInteractions : MonoBehaviour {

	public Blackboard blackboard;

	// Platform
	public Platform platformScript;

	// Other player scripts
	private Player playerMovement;
	private Controller2D controller;

	// Player bounds
	private Bounds playerBounds;

	// Inventory
		// inventory ui
	public List<GameObject> coinInventoryUI = new List<GameObject>();
	public List<GameObject> inventoryUI = new List<GameObject>();

	public GameObject pickupableItem;
	public List<GameObject> pickupableItems = new List<GameObject>();
	public int coinInventorySize = 5;
	public int inventorySize = 3;

	private List<GameObject> coinInventory = new List<GameObject>();
	private List<GameObject> inventory = new List<GameObject>();
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

	// ladders
	public float climbSpeed = 3.0f;
//	public bool overLadder = false;

	// Paying an NPC to employ them
	public PayController payScript;

	// Use this for initialization
	void Start ()
	{

		if (blackboard == null) {
			blackboard = GameObject.Find("Blackboard").GetComponent<Blackboard>();
		}

		playerMovement = GetComponent<Player>();
		controller = GetComponent<Controller2D>();

		playerBounds = GetComponent<BoxCollider2D>().bounds;

		InitializeCoinInventory();


	}


	// put inputs here because if we use Fixedupdate, we may miss an input
	void Update ()
	{

		float inputV = Input.GetAxisRaw ("Vertical");

		if (payScript != null && controller.collisions.below) {
			if (inputV < 0) {
//			if (Input.GetKeyDown(KeyCode.DownArrow)){
//				npcPayScript.StopForPlayer();
				Pay ();
			}
		}

		// call NPCs
		bool action = Input.GetButton ("Fire3");
		bool actionButtonDown = Input.GetButtonDown ("Fire3");

//		if (pickupableItem != null && inputV > 0 && !playerMovement.overLadder) {
		if (pickupableItems.Count > 0 && actionButtonDown) {
			Debug.Log("PICK UP");
			PickUpItem ();
		}else if (inventory.Count > 0 && actionButtonDown) {
			Debug.Log("DROP OFF");
			DropOffItem();
		}

	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Ladder")) {
			playerMovement.overLadder = true;
			playerMovement.ladderTransform = col.transform;
		}
		if (col.CompareTag ("Platform")) {
			platformScript = col.gameObject.GetComponent<Platform>();
		}
	}
	void OnTriggerExit2D (Collider2D col)
	{
		if (col.CompareTag ("Ladder")) {
			playerMovement.overLadder = false;
			playerMovement.mountedLadder = false;
			playerMovement.ladderTransform = null;
		}
	}

	void Pay ()
	{
		if (!passingCurrency) {
			int amountToPay = payScript.cost - payScript.amountPaid;
			if (coinInventory.Count >= amountToPay && amountToPay > 0) {
				passingCurrency = true;
				Debug.Log ("Pay.....");
				StartCoroutine (PassCoin ());
			} else {
				Debug.Log("Not Enough money!!!!!!!!");
				Debug.Log("????: " + (coinInventory.Count >= amountToPay && amountToPay > 0));
				Debug.Log("coinInventory.Count: " + coinInventory.Count);
				Debug.Log("cost: " + payScript.cost);
				Debug.Log("amount paid: " + payScript.amountPaid);
			}
		}
	}

	IEnumerator PassCoin ()
	{
		yield return new WaitForSeconds (0.2f);
		passingCurrency = false;
		if (payScript != null) {
			currency -= 1;
			// first remove ui in last position
			Image uiImage = coinInventoryUI [coinInventory.Count - 1].GetComponent<Image> ();
			uiImage.sprite = uiEmptySprite;
			// remove last actual coin object
			coinInventory.RemoveAt (coinInventory.Count - 1);
			bool purchased = payScript.Pay ();
			if (purchased && payScript.pickupable) {
				PickUpItem (payScript.gameObject);
			}
		}

	}


	public void ReturnCoin(){
		Debug.Log("Return a coin....");
		currency++ ;
		GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity) as GameObject;
		coinInventory.Add (coin);
		coin.SetActive(false);
		Image uiImage = coinInventoryUI[coinInventory.Count-1].GetComponent<Image>();
		uiImage.sprite = uiCoinSprite;
	}

	public void PickUpItem (GameObject itemToBePickedUp = null)
	{
//		PickUp pickupScript = pickupableItem.GetComponent<PickUp> ();
		PickUp pickupScript;
		if (itemToBePickedUp != null) {
			pickupScript = itemToBePickedUp.GetComponent<PickUp> ();
		} else { 
			pickupScript = pickupableItems [0].GetComponent<PickUp> ();
		}
		 switch(pickupScript.generalType) {
		 	case 0:
		 		// picking up NPCs
				if (inventory.Count < inventorySize) {
					pickupScript.PickUpItem (platformScript);
//					inventory.Add (pickupableItem);
					inventory.Add (pickupableItems[0]);
					Image uiImage = inventoryUI [inventory.Count - 1].GetComponent<Image> ();
					uiImage.sprite = uiCoinSprite;
//					pickupableItem = null;
					if (itemToBePickedUp != null){
						pickupableItems.Remove(itemToBePickedUp);
					}else{
						pickupableItems.Remove(pickupableItems[0]);
					}
				} else {
					Debug.Log ("Inventory Full!!!!");
				}
				break;
			case 1:
				if (coinInventory.Count < coinInventorySize) {
					pickupScript.PickUpItem ();
//					coinInventory.Add (pickupableItem);
					coinInventory.Add (pickupableItems[0]);
					Image uiImage = coinInventoryUI [coinInventory.Count - 1].GetComponent<Image> ();
					uiImage.sprite = uiCoinSprite;
//					pickupableItem = null;
//					pickupableItems.Remove(pickupableItems[0]);
					if (itemToBePickedUp != null){
						pickupableItems.Remove(itemToBePickedUp);
					}else{
						pickupableItems.Remove(pickupableItems[0]);
					}
				} else {
					Debug.Log ("Inventory - money Full!!!!");
					if (itemToBePickedUp != null){
						pickupableItems.Remove(itemToBePickedUp);
					}else{
						pickupableItems.Remove(pickupableItems[0]);
					}
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
			Bounds itemBounds = item.GetComponent<BoxCollider2D>().bounds;
			float itemYPos = transform.position.y - (playerBounds.extents.y - itemBounds.extents.y);// add the difference in size to the current transform
			item.transform.position = new Vector3(transform.position.x, itemYPos, transform.position.z);
			PickUp pickupScript = item.GetComponent<PickUp>();
			inventory.Remove(item);
			Image uiImage = inventoryUI[inventory.Count].GetComponent<Image>();
			uiImage.sprite = uiEmptySprite;
			pickupScript.DroppOffItem(platformScript);
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


}