using UnityEngine;
using System.Collections;

//the script attacked to any tower of any kinds, inherits from Photon.monobehaviour instead of behaviour so that it synchs with other players
public class TowerScript : Photon.MonoBehaviour {

	//these 2 integers store on which cell the tower is located
	public int gridPositionX; //from 0 to 5 on the grid
	public int gridPositionY; //from 0 to 8 on the grid

	public int towerLevel; //form 1 to 3 since there are 3 levels for each tower
	//indicates what kind this tower is of
	public string towerName;
	//if there is a human within it's range of sight, this boolean becomes true
	public bool isFiring;
	//all towers will be set to 100 health //TODO:GameDesigner 
	public float towerHealth;

	//how fast will the tower shoot
	public float fireRate;
	//how much damage will each projectile fired deal
	public float projectileDamage;
	//how far can the tower see
	public float shootingRange;
	//how fast will the projectile go
	public float projectileSpeed;
	//the time on the current clock that the tower last fired
	public float lastShootTime;
	//how many targets can the bullet of this tower go through before getting destroyed
	public int pierceCount;

	//public GameObject gridManager;
	//private GridManagerScript gms;
	private DefenderManagerScript dms;

	//will help in spotting if humans are in range
	//RaycastHit hit;
    //Ray ray;


	// Use this for initialization
	void Start () {
		towerHealth = 100;
		isFiring = false;
		//first we need to find the defenderManager to tell it to create a tower in case of an upgrade
		DefenderManagerScript dms = GameObject.Find("DefenderManager").GetComponent<DefenderManagerScript>();
		//note: this can be set to the current time to prevent towers from shooting at their creation or upgrade, but will be kept as a feature
		//this way so that players can upgrade to get a quick shot
		lastShootTime = 0;
	}
	
	// FixedUpdate is more suitable as it performs the functionality on equal intervals
	void FixedUpdate () {
		//since each tower acts differently, some produce souls, some shoot bullets, others shoot lasers we call the perform function
		performFunctionality();
	}

	//doesnt have to be a remote procedure call, since only the defender calls such a function
	//instead we'll just make it public
	//[PunRPC]
	public GameObject upgradeTower ()
	{
		//if already at max level, we can't upgrade anymore
		if (towerLevel == 3) {
			return null;
		}

		//if we are not the defenders (owners of this tower, we cannot upgrade it)
		//this will prevent a malicious opponent from hacking and upgrading a weak tower to cost the defender money
		if (!photonView.isMine) {
			return null;
		}
		//safety check on the defender manager
		if (dms == null) {
			dms = GameObject.Find ("DefenderManager").GetComponent<DefenderManagerScript> ();
			Debug.Log ("dms was null");
		}
		//now that we're sure we want to upgrade the tower:
		int newTowerLevel = towerLevel + 1;
		//set free the spot that the old tower occupied, since we want to create a new upgraded tower
		dms.availableSpots [gridPositionX, gridPositionY] = true;

		//create a new tower (with a higher level) and destroy the old one
		GameObject newTower = dms.buildTower (towerName, newTowerLevel, gridPositionX, gridPositionY);
		if (gameObject.GetComponent<PhotonView> ().isMine) {
			PhotonNetwork.Destroy (this.gameObject);
		}
		return newTower;
	}

	//when selling a tower, destroy the tower and give the player roughly 1/4 of its price
	public void sellTower ()
	{
		//calculate money to be increased (based on tower type and level). implement formula
		//the increment was moved to the code in the "sell" button

		//safety check
		if (dms == null) {
			dms = GameObject.Find ("DefenderManager").GetComponent<DefenderManagerScript> ();
			Debug.Log ("dms was null");
		}
		//free up where the sold tower used to be
		dms.availableSpots [gridPositionX, gridPositionY] = true;
		//destroy this tower using photon network destroy
		if (gameObject.GetComponent<PhotonView> ().isMine) {
			PhotonNetwork.Destroy (this.gameObject);
		}
	}

	//this method will split all the towers based on their name (type of tower) and make each tower behave differently
	//for simplicity the 4 shooting towers are running the same code
	void performFunctionality ()
	{
		//safety check
		if (dms == null) {
			dms = GameObject.Find ("DefenderManager").GetComponent<DefenderManagerScript> ();
			Debug.Log ("dms was null");
		}

		//check to see if this tower is dead
		if (towerHealth <= 0) {
			
			//free up where the dead tower used to be
			dms.availableSpots [gridPositionX, gridPositionY] = true;
			if (gameObject.GetComponent<PhotonView> ().isMine) {
				PhotonNetwork.Destroy (this.gameObject);
			}
		}

		//each tower casts a ray of length "shootingRange" and starts firing if there are enemies in that area
		RaycastHit[] hits = Physics.RaycastAll (transform.position, gameObject.transform.forward, shootingRange);
		isFiring = false;
		if (hits != null) {
			//if the ray collided with an object AND the object is a human, start firing, else stop firing (the stop firing is set above (2 lines before this))
			foreach (RaycastHit hit in hits) {
				if (hit.collider.tag == "Human") {
					isFiring = true;
					//Debug.Log ("SUCCESS from turret " + towerName);
				}
			}
		}

		//for simplicity of the code, treat all towers that have the same logic of shooting particles together
		//and start by letting all of them fire the same particle at their perspective fireRate
		switch (towerName) {
		case "Tow_Cannon":
		case "Tow_Crossbow":
		case "Tow_Gatling":
		case "Tow_Mortar":
			//when no enemies(humans) are within this tower's range then just break
			if (!isFiring) {
				//can implement a penalty for idle towers by letting those that are not firing cost 1 soul per time interval as upkeep cost
				//this will make it harder for the defender and require more strategy from the player
				break;
			}
			//the delay between each bullet and the next, launched from this tower
			float fireDelay = 10f / fireRate; 
			//if we already fired 'relatively' soon, dont fire now
			if (Time.realtimeSinceStartup - lastShootTime < fireDelay) {
				return;
			}
			//set this shot time to be the last shot time
			lastShootTime = Time.realtimeSinceStartup;
			//now that we want to shoot we have to fire an RPC for each projectile
			//RPCs are used because there will be alot of bullets in the scene and this way we cut down on number of gameObjects to update over the netwerk
			//since RPCs are only fired once

			//PhotonView.RPC("launchProjectile",PhotonTargets.All, new object[] {} );
			gameObject.GetComponent<PhotonView>().RPC("launchProjectile",PhotonTargets.All, new object[] {} );

			break;

		case "Tow_Radar": 
		//as fireRate increases with each upgrade, the boost delay will be less with each upgrade and tower will produce souls faster
			float boostDelay = 5 / fireRate;

		//the radars produce souls randomly at a rate based on their level
			if ((Time.realtimeSinceStartup - lastShootTime) < boostDelay) {
				return;
			}
			dms.souls = dms.souls + 10;
			lastShootTime = Time.realtimeSinceStartup;
		break;

		default:
		 break;

		}
	}
	//an RPC that creates a bullet and passes all the attributes of the tower that fired it
	[PunRPC]
	public void launchProjectile (PhotonMessageInfo info)
	{
		//if (PhotonNetwork.isMasterClient) {
			GameObject bullet = PhotonNetwork.Instantiate ("Bullet", gameObject.transform.position + new Vector3 (0, 7, 0), Quaternion.identity, 0);
			//rotate the bullet into the right direction
			bullet.transform.Rotate (90, 90, 0);
			//we hold the bullet projectile script to pass it its parameters
			ProjectileScript bps = bullet.GetComponent<ProjectileScript> ();
			bps.bulletKind = towerName; //this will be useful later to give each tower a different kind of bullet
			bps.damage = projectileDamage;
			bps.speed = projectileSpeed;
			bps.pierceCount = pierceCount;

			//giving the bullet the exact time it was created on the PhotonNetwork clock
			bps.creationTime = info.timestamp;

			//the position the bullet was fired from
			bps.creationPosition = bullet.transform.position;
		//}
	}

}
