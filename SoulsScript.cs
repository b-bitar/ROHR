using UnityEngine;
using System.Collections;

//manages the currency of the defending player (aliens) and displays it on the UI Label
public class SoulsScript : MonoBehaviour {

	private DefenderManagerScript dms;

	void Start () {
			dms = GameObject.Find("DefenderManager").GetComponent<DefenderManagerScript>();
	}

	void Update () {
		//safety check
		if (dms == null) {
			dms = GameObject.Find("DefenderManager").GetComponent<DefenderManagerScript>();
			Debug.Log("dms was null");
		}
		//display the updated number of souls that the defender has
		gameObject.GetComponent<UILabel>().text = ("Souls: " + dms.souls);
		//Detect the winning condition of the aliens
		//TODO: game designer to choose threshold
		int winningSoulsThreshold= 5000;
		if(dms.souls >= winningSoulsThreshold){
		//run the remote procedure call so that both players run the function
			GameObject.Find("Label - Winner").GetComponent<PhotonView>().RPC("DeclareVictory",PhotonTargets.All,new object[] {"aliens"});
		}
	}
}
