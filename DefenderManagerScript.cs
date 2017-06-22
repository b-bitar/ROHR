using UnityEngine;
using System.Collections;

public class DefenderManagerScript : MonoBehaviour {

	//currency of the aliens is human souls, this will allow them to purchase towers
	public int souls;
	public GameObject gridManager;
	private GridManagerScript gms;
	//indicates whether a tower is filling that cell. this will be a 6 by 9 grid of booleans
	//a true indicates an empty cell, a false indicates a tower existing on it
	public bool[,] availableSpots;

	//for testing purposes only:
	GameObject myCrossbow2;

	// Use this for initialization
	void Start ()
	{
		//starting amount of souls
		//TODO: game designer
		souls = 3000;
		//needed only for creating towers with the keyboard to test out functionalities quickly
		myCrossbow2 =null;
		//2d array of booleans (when they're all true, the grid is empty and all cells are available)
		availableSpots = new bool[9, 6];
		gms = gridManager.GetComponentInChildren<GridManagerScript> ();
		for (int i = 0; i < 9; i++) {
			for (int j = 0; j < 6; j++) {
			//initializing all cells to be empty at first
				availableSpots[i,j] = true;
			}
		}
	}

	void OnGUI ()
	{
	//at first, display the detailed state of the Photon Network connection
		string status = PhotonNetwork.connectionStateDetailed.ToString ();
	//then when a room is joined and status stabilizes as "joined", display the ping
		if (status != "Joined") {
			return;
		}
		//GUILayout.Label ("Souls: " + souls);
	}
	// Update is called once per frame
	void Update ()
	{
		//Only the master is the defender
		if (!PhotonNetwork.isMasterClient) {
			return;
		}

		//feel free to uncomment this code and test with it
		/*
		//the z button will place a level 3 cannon on the first cell (by creating one and upgrading it twice)
		if (Input.GetKey (KeyCode.Z)) {
			if (availableSpots [0, 0]) {
				GameObject myCannon = buildTower ("Tow_Cannon", 1, 0, 0);
				availableSpots [0, 0] = false;
				//upgrading the cannon twice
				myCannon = myCannon.GetComponent<TowerScript>().upgradeTower();
				myCannon = myCannon.GetComponent<TowerScript>().upgradeTower();
			}
		}

		if (Input.GetKeyDown (KeyCode.X)) {
			//GameObject myCrossbow =null;
			//if (availableSpots [0, 1]) {
			if (myCrossbow2 == null) {
				myCrossbow2 = buildTower ("Tow_Crossbow", 1, 0, 1);
				Debug.Log("tower built");
			}
			availableSpots [0, 1] = false;
			//myCrossbow.GetComponent<PhotonView>().RPC("upgradeTower",PhotonTargets.All);
			myCrossbow2 =  myCrossbow2.GetComponent<TowerScript> ().upgradeTower ();
			//}
		}
		//the C button tests the destroyTower function
		if (Input.GetKey (KeyCode.C)) {
			//GameObject myCube = PhotonNetwork.Instantiate("cube", gms.GetWorldPosition(new Vector2(3,3)), Quaternion.identity, 0);
			if (availableSpots [0, 2]) {
				GameObject myCrossbow = buildTower("Tow_Crossbow",1,0,2);
				availableSpots [0, 2] = false;
				destroyTower(myCrossbow);
			}
		}
		*/

	}

	//builds a tower of type 'inName' and level 'inLevel' at the cell (Xcoordinate,Ycoordinate)
	public GameObject buildTower (string inName, int inLevel, int Xcoordinate, int Ycoordinate) {
		//Only the master client can build defenses
		if (!PhotonNetwork.isMasterClient) {
			return null;
		}

	//if this spot is available, build a tower there
		if (availableSpots[Xcoordinate, Ycoordinate]) {
			//use the photon network instantiate so that other players can see the tower
			GameObject newTower = PhotonNetwork.Instantiate (inName + inLevel, gms.GetWorldPosition (new Vector2 (Xcoordinate, Ycoordinate)), Quaternion.identity, 0);
			//rotate tower to face the right direction that humans come from
			newTower.transform.Rotate (0, 90, 0);
			//assign its values in tower script
			TowerScript ts = newTower.GetComponent<TowerScript> ();
			ts.towerLevel = inLevel;
			ts.towerName = inName;
			ts.gridPositionX = Xcoordinate;
			ts.gridPositionY = Ycoordinate;
			//to prevent other towers from being built at the same cell
			availableSpots[Xcoordinate,Ycoordinate] = false;
			return newTower;

		}
		return null;
	}
	//destroys a tower and frees it's cell spot in the 2D boolean array "availableSpots"
	public void destroyTower (GameObject tower)
	{
		//Only the defender can destroy towers
		if (!PhotonNetwork.isMasterClient) {
			return;
		}
		TowerScript ts = tower.GetComponent<TowerScript> ();
		int xCoo, yCoo;
		xCoo = ts.gridPositionX;
		yCoo = ts.gridPositionY;
		//free up the spot in the grid
		availableSpots [xCoo, yCoo] = true;
		//an optional if check to ensure safety and prevent errors
		if (tower.GetComponent<PhotonView> ().isMine) {
			PhotonNetwork.Destroy (tower);
		}
	}
}
