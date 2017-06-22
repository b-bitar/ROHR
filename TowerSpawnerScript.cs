using UnityEngine;
using System.Collections;

//calculates where the tower will be built and feeds the cell coordinates to the DefenderManager
public class TowerSpawnerScript : MonoBehaviour {
	public GameObject defenderManager;
	public DefenderManagerScript dms;
	// Use this for initialization
	void Start () {
		dms = defenderManager.GetComponent<DefenderManagerScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void spawnTower (string towerType)
	{
		//make sure the player is in a room
		if (!PhotonNetwork.inRoom) {
			return;
		}
		//safety check. if defender manager script is null, look for it
		if (dms == null) {
			dms = defenderManager.GetComponent<DefenderManagerScript> ();
		}

		//create a ray to see where the user is pointing
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hitInfo;
		//check on what the ray hits
		if (Physics.Raycast (ray, out hitInfo)) {
			//Debug.Log("Mouse is over: " + hitInfo.collider.name);
			float x, y, z;
			//storing the location of the collision
			x = hitInfo.point.x;
			y = hitInfo.point.y;
			z = hitInfo.point.z;

			//Debug.Log("at position: " + x + "," + y + "," + z);
			GameObject hitObject = hitInfo.transform.root.gameObject;

			//now we determine if they ray casted is on which of the cells. The details are in this comment:
			//the droppable area goes from (233,0.01,251)  till (412,0.01,366)
			//it is divided into a grid of 6 cells by 9 cells. IE along the x-z axis it looks like:
			// (233,366)                                       (412,366)
			//
			// (233, 251) 									   (412,251)
			//if the mouse is on the grid dedicated for towers, then try to place a tower
			if (hitInfo.collider.name == "Droppable Area") {
				float xDifference = x - 233; //goes from 0 to 179
				float zDifference = z - 251; //goes from 0 to 115
				//adding a 5 to maximum differences as a safety margin
				const float maxXDifference = 412 - 233 + 5; //179  --> 184
				const float maxZDifference = 366 - 251 + 5; //115  --> 120

				int xCoordinate = (int)(xDifference * 9 / maxXDifference);//going form 0 to 8
				int zCoordinate = (int)(zDifference * 6 / maxZDifference); //going from 0 to 5

				//if (Input.GetMouseButtonDown (0)) {
				//TODO: ask a game designer for specific tower prices. This will do for now
				int towerBuildCost = 50;
				//if the tower is affordable, build it
				if (dms.souls >= towerBuildCost) {
						dms.souls -= towerBuildCost; 
						dms.buildTower (towerType, 1, xCoordinate, zCoordinate);
				}
				//}
			} 

		}



	}
}
