//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

//similar class to humandrag drop item, where they both inherit from UIDragDropItem and override the on release function to spawn a tower
[AddComponentMenu("NGUI/Examples/Drag and Drop Item (Example)")]
public class TowerDragDropItem : UIDragDropItem
{
	/// <summary>
	/// Prefab object that will be instantiated on the DragDropSurface if it receives the OnDrop event.
	/// </summary>

	public GameObject prefab;
	public string towerType;

	//a reference to the tower spawner game object
	public GameObject towerSpawner;

	/// <summary>
	/// Drop a 3D game object onto the surface.
	/// </summary>

	protected override void OnDragDropRelease (GameObject surface)
	{
	//in this method, the player is releasing the draggable object 
	//the inherited boolean from UIDragDropItem
	placeATower = true;
	//spawn a tower if it is a valid place for one
	towerSpawner.GetComponent<TowerSpawnerScript>().spawnTower(towerType);
		if (surface != null)
		{
			ExampleDragDropSurface dds = surface.GetComponent<ExampleDragDropSurface>();

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
