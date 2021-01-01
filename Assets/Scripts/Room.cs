﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
	public Vector2 gridPos;
	public int type;
	public bool doorN, doorS, doorE, doorW;
	public Room(Vector2 _gridPos, int _type)
	{
		gridPos = _gridPos;
		type = _type;
	}
}
