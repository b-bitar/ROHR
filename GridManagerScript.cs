using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//draws a grid of cubes on the terrain (if ShowGrid is true). Note: this code is not the property of Bassel Bitar but he will comment it anyway for clarity
public class GridManagerScript : MonoBehaviour {
 
 public Terrain terrain; //terrain grid is attached to
 //determines whether to display the blocks or not
 public bool ShowGrid = false;
 //size of each cell
 public int CellSize = 10;

 //terrainsize will not be used as we only want to divide a certain section of the whole terrain
 private Vector3 terrainSize;
 private Vector3 origin;

 //parameters of the grid (in ROHR will be 9*6)
 private int width;
 private int height;
 
 private List<GameObject> objects;

  void Start ()
 {
  //this will not be used
  terrainSize = terrain.terrainData.size;
  //starting point of the terrain
  origin = terrain.transform.position;
  
  //width = (int)terrainSize.z / CellSize;
  //height = (int)terrainSize.x / CellSize;
  //replaced terrain size / cell size by the exact dimensions needed for ROHR game (a 6x9 grid)
  width = 9;
  height = 6;

  objects = new List<GameObject>();
  
  BuildGrid();  
 }

  void Update ()
 {
 //to display the grid if the boolean ShowGrid is checked
  foreach(GameObject obj in objects)
   obj.SetActive(ShowGrid);
 }

 //builds the width by height grid in O(m*n), in our case will take an order of 9*6 iterations  (O(1))
 void BuildGrid()
 {  
 //looping over width*height (9*6)
  for(int x = 0; x < width; x++)
  {
   for(int y = 0; y < height; y++)
   {
   //builds a cube and colors it red or green if it's on the first side and white if in the middle
    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Vector3 pos = GetWorldPosition(new Vector2(x,y));
    pos += new Vector3(CellSize / 2, terrain.SampleHeight(GetWorldPosition(new Vector2(x,y))), CellSize /2);
    go.transform.position = pos;
    if(x==0)
     {
      go.GetComponent<Renderer>().material.color = Color.red;
     }
    if(y==0)
     go.GetComponent<Renderer>().material.color = Color.green;
    
    go.transform.localScale = new Vector3(CellSize /2, CellSize /2, CellSize/2);
    go.transform.parent = transform;
    go.SetActive(false);
    
    objects.Add(go);
   }
  } 
 }

 //returns a vector3 of the actual position of a cell given its coordinates (x,y)
 //this is used to locate humans and towers on specific places on the terrain
 public Vector3 GetWorldPosition(Vector2 gridPosition)
 {
 //adding (240,0,260) to each cell
 //this will shift the grid from the origin of the terrain to its desired position
  return new Vector3(origin.z + (gridPosition.x * CellSize) + 240, origin.y, origin.x + (gridPosition.y * CellSize) + 260);
 }
 
 public Vector2 GetGridPosition(Vector3 worldPosition)
 {
  return new Vector2(worldPosition.z / CellSize, worldPosition.x / CellSize);
 }
}