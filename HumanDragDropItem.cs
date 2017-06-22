//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------
//Bassel Bitar
using UnityEngine;

//this class inherits from UIDragDropItem which are items in a dockable Next-Gen UI Selection Menu inside a scrollview
//The OnRelease function has been overriden to spawn humans
[AddComponentMenu("NGUI/Examples/Drag and Drop Item (Example)")]
public class HumanDragDropItem : UIDragDropItem
{
	/// <summary>
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public GameObject prefab;
	//the type of this item. The item holds an icon of the picture of the human to display to the player
	public string humanType;

	//a reference to the tower spawner game object
	public GameObject humanSpawner;

	/// <summary>
	/// Drop a 3D game object onto the surface.
	/// </summary>
	//overriding the release of a drag and drop object 
	protected override void OnDragDropRelease (GameObject surface)
	{
	//in this method, the player is releasing the draggable object, so we call the spawnHuman method and pass it the type of
	//human that the player was dragging 
	humanSpawner.GetComponent<HumanSpawnerScript>().spawnHuman(humanType);

		if (surface != null)
		{
			ExampleDragDropSurface dds = surface.GetComponent<ExampleDragDropSurface>();
			//not Bassel Bitar's code. Ignore this if block
			if (dds != null)
			{
				GameObject child = NGUITools.AddChild(dds.gameObject, prefab);
				child.transform.localScale = dds.transform.localScale;

				Transform trans = child.transform;
				trans.position = UICamera.lastWorldPosition;

				if (dds.rotatePlacedObject)
				{
					trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
				}
				
				// Destroy this icon as it's no longer needed
				NGUITools.Destroy(gameObject);
				return;
			}
		}
		base.OnDragDropRelease(surface);
	}
}
