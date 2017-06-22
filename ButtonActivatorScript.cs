using UnityEngine;
using System.Collections;

//simple class to activate a button (used for the play button after the typewriter effect ends)
public class ButtonActivatorScript : MonoBehaviour {
	//at first the play button is not active
	public bool isActive = false;

	public void Activate ()
	{
		isActive = true;
		//Activate the button
		gameObject.SetActive (isActive);
	}
}
