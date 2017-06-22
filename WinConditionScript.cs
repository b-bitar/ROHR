using UnityEngine;
using System.Collections;

public class WinConditionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<UILabel>().text = "";
	}

	//Displays that the game has ended and informs of who the victor is
	[PunRPC]
	public void DeclareVictory (string winner)
	{
		if (winner == "humans") {
			gameObject.GetComponent<UILabel>().text = "MotherShip Destroyed!\n Humans are Victorious";
		}
		if (winner == "aliens") {
			gameObject.GetComponent<UILabel>().text = "Human Souls Depleted\n Aliens are Victorious";
		}
	}

}
