using UnityEngine;
using System.Collections;

public class ExitButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//when the exit button is clicked, close the application
	public void OnExit ()
	{
		Debug.Log("trying to exit");
		Application.Quit();
	}
}
