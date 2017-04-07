using UnityEngine;
using System.Collections;

public class CostIndicator : MonoBehaviour {

	public GameObject empty;
	public GameObject full;


	public void Pay(){
		full.SetActive(true);
		empty.SetActive(false);
	}
	public void Refund(){
		full.SetActive(false);
		empty.SetActive(true);
	}

}
