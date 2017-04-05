using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	// general type
	// 0 -> NPC
	// 1 -> coin
	// 2 -> special item?
	public int generalType = 0;

	public NPC npcScript;
	public Item itemScript;

	// Use this for initialization
	void Start () {
		npcScript = GetComponent<NPC>();
		itemScript = GetComponent<Item>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PickUpItem(){
		gameObject.SetActive(false);
	}
	public void DroppOffItem ()
	{
		Debug.Log ("Dropped off");
		switch (generalType) {
			case 0:
				npcScript.KeepMoving ();
				break;
			case 1:
				Debug.Log("Do Something with item... drop on ground");
				break;
			default:
				Debug.Log("fell through switch statement - Pickup.cs");
				break;
		}
	}
}
