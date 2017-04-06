using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Player))]
[RequireComponent (typeof(Controller2D))]
public class PlayerInteractions : MonoBehaviour {

	public Blackboard blackboard;

	// Other player scripts
	private Player playerMovement;
	private Controller2D controller;

	// Inventory
		// inventory ui
	public List<GameObject> coinInventoryUI = new List<GameObject>();
	public List<GameObject> inventoryUI = new List<GameObject>();

	public GameObject pickupableItem;
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

		InitializeCoinInventory();


	}


	// put inputs here because if we use Fixedupdate, we may miss an input
	void Update ()
	{

		float inputV = Input.GetAxisRaw ("Vertical");

		if (payScript != null && controller.collisions.below) {
			if (inputV < 0) {
//				npcPayScript.StopForPlayer();
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
			playerMovement.overLadder = true;
			playerMovement.ladderTransform = col.transform;
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
			if (coinInventory.Count >= payScript.cost - payScript.amountPaid && amountToPay > 0) {
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

	public void PickUpItem ()
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


}