using UnityEngine;
using System.Collections;

//Manages everything related to Humans (the attackers)
public class AttackerManagerScript : MonoBehaviour {

	//currency of humans is dollars, this allows them to hire mercenaries
	public int balance;
	//references to the grid manager and the grid manager script so we can spawn humans at certain cells
	public GameObject gridManager;
	private GridManagerScript gms;

	void Start () {
		//starting amount of money (in dollars)
		balance = 500; //arbitrarily set to 500 to make the demo more interesting
		//TODO: ask a game designer for a good starting amount and incremental value
		//find the script and hold it in gms (grid manager script)
		gms = gridManager.GetComponentInChildren<GridManagerScript> ();

	}

	//creates a human using Photon Network's Instantiate, so it is visible to all other players
	//takes in the name of the human to be spawned (aka his/her type)
	//and the coordinates of where the human is to be created
	public GameObject spawnHuman (string inName, int Xcoordinate, int Ycoordinate)
	{
		//instantiate a new human on the photon network  
		GameObject newHuman = PhotonNetwork.Instantiate (inName,gms.GetWorldPosition (new Vector2 (Xcoordinate, Ycoordinate)), Quaternion.identity, 0);
		//to head to the left
		newHuman.transform.Rotate (0, -90, 0);

		return newHuman;
	}
}
