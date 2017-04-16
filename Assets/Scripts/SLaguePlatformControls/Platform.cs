using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour {

	public bool active;
	public int platformType = 1;

	public int maxCoins = 10;
	public GameObject coinPrefab;
	// TODO: could maybe add a time to replenish coins... so after a time the coins return... money trees or something
	public List<GameObject> coins = new List<GameObject>();

	// keep track of who is on the paltform
	public List<GameObject> builders = new List<GameObject>();
	public List<GameObject> fighters = new List<GameObject>();
	public List<GameObject> averageJoes = new List<GameObject>();

	public GameObject edgeLeft;
	public GameObject edgeRight;

	public void ListCoins()
	{
		for (int i = 0; i < maxCoins; i++) {
			GameObject coinObj = Instantiate(coinPrefab, transform.position, Quaternion.identity) as GameObject;
			coins.Add(coinObj);
			coinObj.SetActive(false);
		}
	}

	public void FindCoin (Vector3 position)
	{
		// TODO: be careful here, if 2 NPCs both try to activate the same coin at the same time then probably an error/bug here
			GameObject foundCoin = coins [0];
			foundCoin.SetActive (true);
			foundCoin.transform.position = position;
			coins.Remove (foundCoin);
	}

}
