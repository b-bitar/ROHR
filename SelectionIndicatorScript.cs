using UnityEngine;
using System.Collections;

//moves the quad with a red borders image to whichever tower is selected
public class SelectionIndicatorScript : MonoBehaviour {

	TowerSelectorScript tss;
	// Use this for initialization
	void Start () {
		tss = GameObject.FindObjectOfType<TowerSelectorScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	//find out which tower is selected and move the quad to it, if nothing is selected go to (0,0,0)
		if (tss.selectedObject != null) {
			this.transform.position = tss.selectedObject.transform.position;
		} else {
			this.transform.position = new Vector3(0,0,0);
		}
	}
}
