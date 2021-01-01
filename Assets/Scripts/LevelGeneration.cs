using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour 
{
	Vector2 worldSize = new Vector2(4,4);
	Room[,] rooms;
	List<Vector2> takenPositions = new List<Vector2>();
	int gridSizeX, gridSizeY, numberOfRooms = 20;
	public GameObject roomWhiteObj;
	public Transform mapRoot;
	void Start () 
	{
		if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) //rooms only exist in the grid.
		{
			numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
		}
		gridSizeX = Mathf.RoundToInt(worldSize.x); //grid value is 2x this size due to positive and negative numbers
		gridSizeY = Mathf.RoundToInt(worldSize.y);
		CreateRooms();
		SetRoomDoors();
		DrawMap(); //instantiates map objects
		GetComponent<SheetAssigner>().Assign(rooms); //passes room info to another script which generates the level geometry
	}
	void CreateRooms()
	{
		rooms = new Room[gridSizeX * 2,gridSizeY * 2];
		rooms[gridSizeX,gridSizeY] = new Room(Vector2.zero, 1);
		takenPositions.Insert(0,Vector2.zero);
		Vector2 checkPos = Vector2.zero;
		float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f; //Random Number Generator
		for (int i =0; i < numberOfRooms -1; i++)
		{
			float randomPerc = ((float) i) / (((float)numberOfRooms - 1));
			randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
			checkPos = NewPosition();
			if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
			{
				int iterations = 0;
				do
				{
					checkPos = SelectiveNewPosition();
					iterations++;
				}while(NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
				if (iterations >= 50)
				{
					print("error: could not create with fewer neighbors than : " + NumberOfNeighbors(checkPos, takenPositions));
				}
					
			}
			//finalize position
			rooms[(int) checkPos.x + gridSizeX, (int) checkPos.y + gridSizeY] = new Room(checkPos, 0);
			takenPositions.Insert(0,checkPos);
		}	
	}
	Vector2 NewPosition()
	{
		int x = 0, y = 0;
		Vector2 checkingPos = Vector2.zero;
		do //pick a room at random, get its grid location, randomly pick NSEW, then generate room and add it to the vector list.
		{
			int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); // pick a random room
			x = (int) takenPositions[index].x;//capture its x, y position
			y = (int) takenPositions[index].y;
			bool UpDown = (Random.value < 0.5f);//randomly pick wether to look on hor or vert axis
			bool positive = (Random.value < 0.5f);//pick whether to be positive or negative on that axis
			if (UpDown){ //find the position bnased on the above bools
				if (positive)
				{y += 1;}
				else
				{y -= 1;}
			}
			else
			{
				if (positive)
				{x += 1;}
				else
				{x -= 1;}
			}
			checkingPos = new Vector2(x,y);
		}
		while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY); //if something breaks it's because of this
		return checkingPos;
	}
	Vector2 SelectiveNewPosition()
	{ 
		int index = 0, inc = 0;
		int x =0, y =0;
		Vector2 checkingPos = Vector2.zero;
		do
		{
			inc = 0;
			do
			{
				//instead of getting a room to find an adject empty space, we start with one that only 
				//as one neighbor. This will make it more likely that it returns a room that branches out
				index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
				inc ++;
			}
			while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
			x = (int) takenPositions[index].x;
			y = (int) takenPositions[index].y;
			bool UpDown = (Random.value < 0.5f);
			bool positive = (Random.value < 0.5f);
			
			if (UpDown)
			{
				if (positive)
				{y += 1;}
				else{y -= 1;}
			}
			else
			{
				if (positive)
				{x += 1;}
				else
				{x -= 1;}
			}
			checkingPos = new Vector2(x,y);
		}
		while (takenPositions.Contains(checkingPos) || x >= gridSizeX || x < -gridSizeX || y >= gridSizeY || y < -gridSizeY);
		if (inc >= 100){ // break loop if it takes too long: this loop isnt garuanteed to find solution, which is fine for this
			print("Error: could not find position with only one neighbor");
		}
		return checkingPos;
	}
	int NumberOfNeighbors(Vector2 checkingPos, List<Vector2> usedPositions) //count Vector.[direction] as short hand to make room checks easier.
	{
		int ret = 0;
		if (usedPositions.Contains(checkingPos + Vector2.right))  
		{ ret++; }
		if (usedPositions.Contains(checkingPos + Vector2.left))
		{ ret++; }
		if (usedPositions.Contains(checkingPos + Vector2.up))
		{ ret++; }
		if (usedPositions.Contains(checkingPos + Vector2.down))
		{ ret++; }
		return ret;
	}
	void DrawMap()
	{
		foreach (Room room in rooms)
		{
			if (room == null)
			{
				continue;
			}
			Vector2 drawPos = room.gridPos;
			drawPos.x *= 16;//aspect ratio of map sprite -- if this changes, you also need to update MapSpriteSelector, RoomInstance and SheetAssigner.
			drawPos.y *= 8;
			
			//create map object and assign variables here
			MapSpriteSelector mapper = Object.Instantiate(roomWhiteObj, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
			mapper.type = room.type;
			mapper.n = room.doorN;
			mapper.s = room.doorS;
			mapper.e = room.doorE;
			mapper.w = room.doorW;
			mapper.gameObject.transform.parent = mapRoot;
		}
	}
	void SetRoomDoors()
	{
		for (int x = 0; x < ((gridSizeX * 2)); x++)
		{
			for (int y = 0; y < ((gridSizeY * 2)); y++)
			{
				if (rooms[x,y] == null)
				{
					continue;
				}
				Vector2 gridPosition = new Vector2(x,y);  //check each direction for doors in each direction

				if (y - 1 < 0)
				{rooms[x,y].doorS = false;}
				else
				{rooms[x,y].doorS = (rooms[x,y-1] != null);}
				if (y + 1 >= gridSizeY * 2)
				{rooms[x,y].doorN = false;}
				else
				{rooms[x,y].doorN = (rooms[x,y+1] != null);}
				if (x - 1 < 0)
				{rooms[x,y].doorW = false;}
				else
				{rooms[x,y].doorW = (rooms[x - 1,y] != null);}
				if (x + 1 >= gridSizeX * 2)
				{rooms[x,y].doorE = false;}
				else
				{rooms[x,y].doorE = (rooms[x+1,y] != null);}
			}
		}
	}
}
