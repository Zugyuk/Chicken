using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour 
{
	public Sprite 	spN, spS, spE, spW,	spNS, spEW, spNE, spNW, spSE, spSW,	spNSW, spNEW, spNSE, spSEW, spNSEW;
	public bool n, s, e, w;
	public int type; // 0: normal, 1: enter
	public Color normalColor, enterColor;
	Color mainColor;
	SpriteRenderer rend;
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();
		mainColor = normalColor;
		PickSprite();
		PickColor();
	}
	void PickSprite() //if statements, my switch cases won't work :( 
	{
		if (n)
		{
			if (s)
			{
				if (e)
				{
					if (w)
					{rend.sprite = spNSEW; }
					else
					{rend.sprite = spNSE; }
				}
				else if (w)
				{rend.sprite = spNSW; }
				else
				{rend.sprite = spNS; }
			}
			else
			{
				if (e)
				{
					if (w)
					{rend.sprite = spNEW; }
					else
					{rend.sprite = spNE; }
				}
				else if (w)
				{rend.sprite = spNW; }
				else
				{rend.sprite = spN; }
			}
			return;
		}
		if (s)
		{
			if (e)
			{
				if(w)
				{rend.sprite = spSEW; }
				else
				{rend.sprite = spSE; }
			}
			else if (w)
			{rend.sprite = spSW; }
			else
			{rend.sprite = spS; }
			return;
		}
		if (e)
		{
			if (w)
			{rend.sprite = spEW; }
			else
			{rend.sprite = spE; }
		}
		else
		{rend.sprite = spW; }
	}

	void PickColor() //changes map room color based on value
	{ 
		if (type == 0)
		{mainColor = normalColor;}
		else if (type == 1)
		{mainColor = enterColor;}
		rend.color = mainColor;
	}
}