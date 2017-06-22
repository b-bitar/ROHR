using UnityEngine;
using System.Collections;


//glows the tower being hovered on green, and draws a red square border under the selected tower
//also holds the hoveredObject and selectedObject in 2 public GameObjectAttributes to be accessed by other classes (for upgrade and sell)
public class TowerSelectorScript : MonoBehaviour {
	//the game object (only towers) that the mouse is hovering over
	//these will glow green to indicate so
	public GameObject hoveredObject;
	//the game object (only towers) that the mouse has clicked on previousely (selected)
	//these will have a red frame indicating quad under them and can be upgraded or sold
	public GameObject selectedObject;

	
	// Update is called once per frame
	void Update ()
	{
	//creates a ray from the camera through the mouse position to detect what is being pointed at
	//this method will be useful for implementing augmented reality
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hitInfo;

		if (Physics.Raycast (ray, out hitInfo)) {
			//Debug.Log("Mouse is over: " + hitInfo.collider.name);
			//the object that was hit by the mouse
			GameObject hitObject = hitInfo.transform.root.gameObject;

			//uncommenting this if prevents hovering over humans
			//if (hitObject.tag == "Tower" ) {
				hoverOverObject (hitObject);
			//}
			//if the left mouse is clicked, select the tower that is being hovered over and draw the quad under it to indicate selection
			if (Input.GetMouseButtonDown (0)) {
				if (hoveredObject.tag == "Tower") {
					selectedObject = hoveredObject;
					//Debug.Log("ASSIGNMENT SUCCESSFUL, tower is now: " + selectedObject.name);
				} 
				//clear the selection when the player clicks at a random place in the grid
				else if(hoveredObject.name == "Grid") {
				clearSelection();
				}
			}
		} else {
			clearHover();
		}

	}
	//assign the tower being hovered on to the hoveredObject game object and give it a shade of green
	void hoverOverObject (GameObject inHoveredObject)
	{
	//Debug.Log("hovering over " + inHoveredObject.name);
		if (hoveredObject != null) {
			//if we already had an object selected
			if (inHoveredObject == hoveredObject) {
				//AND the selected object is the one we already have, no need to do anything just return
				return;
			}
			clearHover ();
		}
		hoveredObject = inHoveredObject;
		//get all the renderers of the object and color them green
		Renderer[] rs = hoveredObject.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in rs) {
			Material m = r.material;
			m.color = Color.green;
			r.material = m;
		}
	}

	//removes the green tint off of the object that was hovered on and clears hoveredObject
	void clearHover ()
	{
		if (hoveredObject == null) {
		return;
		}
		//get all the renderers of the object and color them white
		Renderer[] rs = hoveredObject.GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in rs) {
			Material m = r.material;
			m.color = Color.white;
			r.material = m;
		}
		hoveredObject = null;
	}

	//static IEnumerable<int> clearSelection ()
	void clearSelection()
	{
	//Debug.Log("selection cleared");
	//todo pause for 0.25 seconds so that the button knows what to upgrade/ sell
		selectedObject = null;
		//yield return 1;
	}
}
