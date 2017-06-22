using UnityEngine;
using Photon;
 
public class RandomMatchmaker : Photon.PunBehaviour
{
	public GameObject defenderUI;
	public GameObject attackerUI;
	public GameObject defenderManager;
	public GameObject attackerManager;

	private bool hidePing = false;
    // Use this for initialization
    void Start()
    {
   	 	//Screen.SetResolution(1280,800,true);
        PhotonNetwork.ConnectUsingSettings("0.1");
		//PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }
 
    void OnGUI ()
	{		
		//display the detailed status of the photon network, until a room is joined. 
		//When it is, display the team that the player is playing on and his/her ping

		string status = PhotonNetwork.connectionStateDetailed.ToString ();
		if (status != "Joined") {
			//while trying to connect, display the different states that the connection got stuck on
			GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
		} else {
			//once a room is joined, always display the ping to and back from the server together with the race of the player
			if (PhotonNetwork.isMasterClient) {
				//if the ping is requested to be hidden display a "show" button
				if (!hidePing) {
					GUILayout.Label ("Alien\tPing: " + PhotonNetwork.GetPing ());
					if (GUILayout.Button ("hide", GUILayout.ExpandWidth (false))) {
						hidePing = true;
					}
				} else {
					if (GUILayout.Button ("show", GUILayout.ExpandWidth (false))) {
						hidePing = false;
					}
				}
			}
        //if not a master, then must be an attacker
        else if (PhotonNetwork.isNonMasterClientInRoom) {
				if (!hidePing) {
					GUILayout.Label ("Human\tPing: " + PhotonNetwork.GetPing ());
					if (GUILayout.Button ("hide", GUILayout.ExpandWidth (false))) {
						hidePing = true;
					}
				} else {
					if (GUILayout.Button ("show", GUILayout.ExpandWidth (false))) {
						hidePing = false;
					}
				}
			}
		}
    }

    public override void OnJoinedLobby ()
	{
		//when the default lobby is joined, join a random room
		PhotonNetwork.JoinRandomRoom();
	}

	public void OnJoinedRoom ()
	{
		//GameObject myCube = PhotonNetwork.Instantiate("cube", new Vector3(330,7,250), Quaternion.identity, 0);
		//if the player is the master client, set him/her as a defender
		if (PhotonNetwork.isMasterClient) {
			defenderUI.SetActive(true);
			//defenderManager.SetActive(true);
			attackerUI.SetActive(false);
			//attackerManager.SetActive(false);
		} else {
			defenderUI.SetActive (false);
			//defenderManager.SetActive(false);
			attackerUI.SetActive (true);
			//attackerManager.SetActive(true);
		}

		//toggle, for testing purposes only
		//TODO comment the below code
		/*
		if (PhotonNetwork.isMasterClient) {
			defenderUI.SetActive(false);
			//defenderManager.SetActive(false);
			attackerUI.SetActive(true);
			//attackerManager.SetActive(true);
		} else {
			defenderUI.SetActive (true);
			//defenderManager.SetActive(true);
			attackerUI.SetActive (false);
			//attackerManager.SetActive(false);
		}
		*/
	}

	//if there are no available rooms
	public void OnPhotonRandomJoinFailed()
	{
    	//Debug.Log("Can't join random room, creating one");
		Screen.fullScreen = false;

		//to limit the number of players to 2
		RoomOptions ro = new RoomOptions();
		ro.isVisible=true;
		ro.maxPlayers=2;
		//create a room using the room options and default typed lobby
		PhotonNetwork.CreateRoom(null,ro,TypedLobby.Default);


	}
}