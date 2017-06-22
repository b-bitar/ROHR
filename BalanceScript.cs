using UnityEngine;
using System.Collections;

//a script to manage the amount of money (in dollars the player has)
public class BalanceScript : MonoBehaviour {

	//reference to the attacker manager script
	private AttackerManagerScript ams;
	//TODO: ask a game designer to tweak this number
	float revenueGenerationFactor;
	//the amount of money made per second
	float revenue;
	// Use this for initialization
	void Start () {
		ams = GameObject.Find("AttackerManager").GetComponent<AttackerManagerScript>();
		revenueGenerationFactor = 2f;
		revenue = 0;

	}
	
	// Update is called once per frame
	void Update () {
		//safety check
		if (ams == null) {
			ams = GameObject.Find("AttackerManager").GetComponent<AttackerManagerScript>();
			Debug.Log("ams was null");
		}
		//increment the revenue by the time dependant factor
		revenue += (Time.deltaTime*revenueGenerationFactor);
		//how many dollars will be added this frame
		int tobeAdded = (int)revenue;
		revenue-=tobeAdded;
		//with time the humans generate revenue:
		ams.balance += (int)(tobeAdded);

		//display the updated balance in dollars that the attacker has
		gameObject.GetComponent<UILabel>().text = ("Balance: $" + ams.balance);
	}
}
