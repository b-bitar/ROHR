using UnityEngine;
using System.Collections;
//calculates where the human will be spawned and feeds the cell coordinates to the DefenderManager
public class HumanSpawnerScript : MonoBehaviour {
	public GameObject attackerManager;
	public AttackerManagerScript ams;
	// Use this for initialization
	void Start () {
		ams = attackerManager.GetComponent<AttackerManagerScript>();
	}

	//creates a human of type "humanType" using a raycast from the camera to the mouse and sees where it lands
	//if landed on part of the grid, then it is a legal place to spawn the human
	//Note that only the last 3 rows are allowed for spawning humans
	public void spawnHuman (string humanType)
	{
		//make sure the player is in a room (this prevents against players trying to spawn things before joining the game)
		if (!PhotonNetwork.inRoom) {
			return;
		}
		//safety check. if attacker manager script is null, look for it
		if (ams == null) {
			ams = attackerManager.GetComponent<AttackerManagerScript> ();
		}

		//the following method will be used to ease in implementing augmented reality
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

			GameObject hitObject = hitInfo.transform.root.gameObject;

			//now we determine if they ray casted is on which of the cells. The details are in this comment:
			//the droppable area goes from (233,0.01,251)  till (412,0.01,366)
			//it is divided into a grid of 6 cells by 9 cells. IE along the x-z axis it looks like:
			// (233,366)                                       (412,366)
			//
			// (233, 251) 									   (412,251)
			//if the mouse is on the grid dedicated for towers, then try to place a tower

			//ensure that the ray from the mouse collided with a droppable area
			if (hitInfo.collider.name == "Droppable Area") {
				float xDifference = x - 233; //goes from 0 to 179
				float zDifference = z - 251; //goes from 0 to 115
				//adding a 5 to maximum differences as a safety margin
				const float maxXDifference = 412 - 233 + 5; //179  --> 184
				const float maxZDifference = 366 - 251 + 5; //115  --> 120

				//determining which cell in the 9 by 6 grid to place the human on
				int xCoordinate = (int)(xDifference * 9 / maxXDifference);//going form 0 to 8
				int zCoordinate = (int)(zDifference * 6 / maxZDifference); //going from 0 to 5

				//for humans the player is only allowed to spawn them on the last 3 rows (rows 6,7, and 8)
				if (xCoordinate < 6) {
					return;
				}

				//now that the location of spawning is known, check if the human is affordable and spawn it

				//TODO: ask a game designer for specific tower prices. This will do for now
				int humanSpawnCost = 70;
				//if the tower is affordable, build it
				if (ams.balance >= humanSpawnCost) {
					ams.balance -= humanSpawnCost;

					//which will be one of three (till now) : soldier ("Soldier01"), sporty girl ("SportyGirl01"), or butcher ("TheButcher")
					ams.spawnHuman (humanType, xCoordinate, zCoordinate);
				}
			} 
		}
	}
}
