using UnityEngine;
using System.Collections;

//the script attacked on each human
public class HumanScript : MonoBehaviour {

	//how many health points this human has
	public float health;
	//the speed in which this mercenary walks/ runs
	public float speed;
	//damage per second this human deals when he reaches a turret (tower)
	public float dps;

	//if running, speed will double
	public bool isRunning;
	//if fighting with a tower, speed will be 0
	public bool isHittingTower;

	//will hold where the human was created
	public Vector3 creationPosition;

	//the time in which this human was created
	double timeCreated;
	//the time of the last motion update
	double timeSinceLastMoved;

	void Start () {
	isHittingTower = false;
		
		//this makes humans become tankier as the game progresses
		health = health + 6*Time.realtimeSinceStartup;
		//and faster
		speed = speed + (float)(0.1f*Time.realtimeSinceStartup);
		//and stronger
		dps = dps + (float)(0.5*Time.realtimeSinceStartup);

		timeCreated = Time.realtimeSinceStartup;
		timeSinceLastMoved = timeCreated;
		//initializing the creation position to where the human is at the start
		creationPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float actualSpeed;
		//human dies if his HP reach 0 (or less)
		if (health <= 0) {
			//die
			//Debug.Log ("Romeo must die");
			if (gameObject.GetComponent<PhotonView> ().isMine) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
		//when a human is hitting a tower, he/she stops in place so the actual speed will be 0
		if (isHittingTower) {
			//standing in place and attacking a tower
			actualSpeed = 0;
		} else if (isRunning) {
			//to make runners faster or slower modify the 2  TODO: game designer
			actualSpeed = 2 * speed;
		} else {
		//walking towards the mothership
			actualSpeed = speed;
		}
		double timePassed = Time.realtimeSinceStartup - timeSinceLastMoved;

		timeSinceLastMoved = Time.realtimeSinceStartup;
		//calculates the correct position for this human based on a simple speed=(distance/time) formula
		Vector3 correctPositionNow = gameObject.transform.position - new Vector3 (((float)(timePassed * actualSpeed) * 1.0f), 0f, 0f);
		//Debug.Log("correct position now: " +correctPositionNow.x);
 
		//linearly interpolates where the player should be, given his current position and his correct position to be in
		//TODO: game designer tweak with the 0.5f factor to make things smoother or more correct
		gameObject.transform.position = Vector3.Lerp (transform.position, correctPositionNow, 0.5f);

	}


	void OnCollisionStay (Collision col)
	{
		
		if (col.collider.tag == "Tower") {
			//Debug.Log ("attack animation placed here .... if I had one!!");
			//damage the tower
			isHittingTower = true;
			col.collider.GetComponent<TowerScript> ().towerHealth -= dps * Time.deltaTime;

		} else {
			isHittingTower = false;
		}
		//if a human reached the end of the map and hit the winning condition object (the mothership)
		//the game is over and the attacker wins
		 //end conditions
		if (col.collider.tag == "MotherShip") {
			//Debug.Log("THE GAME HAS ENDED");
			GameObject.Find("Label - Winner").GetComponent<PhotonView>().RPC("DeclareVictory",PhotonTargets.All,new object[] {"humans"});
		}

	}

}
