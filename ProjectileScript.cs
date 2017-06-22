using UnityEngine;
using System.Collections;

//the script of each bullet, cannonball, spear/arrow, etc...
public class ProjectileScript : MonoBehaviour {

	//all these 4 parameters will be passed on from the tower that instantiated it
	public string bulletKind;
	public float speed;
	public float damage;
	//how many humans can this bullet go through before breaking
	public int pierceCount;

	public bool isInCollision = false;
	//note that this is the time on the PhotonNetwork (PhotonNetwork.time) not unity's current time
	public double creationTime;

	//the place the bullet was launched from
	public Vector3 creationPosition;

	// Use this for initialization
	void Start () {
		isInCollision = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//calculate the position that the bullet should be in at a certain point in time
		//this prediction method is done on each client and hence doesnt require to send data over the network
		//However, this allows us to ensure that bullets are exactly in the same place at the same time across all clients 
		//so that they can detect collisions accurately
		double timePassed = PhotonNetwork.time - creationTime;
		//since bullets fly in the x axis direction we add the distance moved to the x coordinate of creation position
		//the distance it moved is simply "timePassed" multiplied by the bullet's speed 
		Vector3 correctPositionNow = creationPosition + new Vector3 (((float)(timePassed * speed) * 1.0f), 0f, 0f);
		//set the bullet to its correct position (smoothly by linear interpolation)
		gameObject.transform.position = Vector3.Lerp (transform.position, correctPositionNow, 0.5f);

		//check for collisions with a human, and decrement its pierceCount if a collision occurs
		//Destroy the bullet when pierceCount reaches 0
		//implementing a timeout to destroy bullets after 10 seconds
		if (timePassed > 10) {
			if (gameObject.GetComponent<PhotonView> ().isMine) {
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}

	//cif the bullet collides with another object check if it is a human and damage him/her
	void OnTriggerEnter (Collider col)
	{
	isInCollision = true;
		if (col.tag == "Human") {
			//damage the human

			col.GetComponent<HumanScript> ().health -= damage;
			//now it can withstand n-1 more humans
			pierceCount--;
			//if it can no longer pierce any humans AND it hit its last target (it became negative) destroy it
			if (pierceCount < 0) {
				//if i am the owner of this bullet, then destroy it over the network
				if (gameObject.GetComponent<PhotonView> ().isMine) {
					PhotonNetwork.Destroy (gameObject);
				}
			}
		}
		//if a bullet reached the end of the map and hit terrain destroy it nomatter how much pierce it has
		//this method of destroying bullets didn't work for some reason (a collision was not detected)
		/*
		if (col.tag == "BulletDestroyer") {
			//destroy this bullet
			Debug.Log ("bullet cleaned");
			PhotonNetwork.Destroy (gameObject);
		}
		*/
	}
}
