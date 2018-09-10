using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{

	public GameObject SectorPrefab;
	public Sector[,] WorldSectors;
	public int DimensionsX, DimensionsY;
	public int NumGroups = 0;
	public DateTime CurrentDate;
	public Group PlayerGroup;
		
	public int WorldZedCount;
	public int WorldPlayerGroupMemberCount;
	public int WorldOtherGroupMemberCount;

	public RawImage MapImage;
	public Texture Tex;
	public static Dictionary<int,int> WorldSizes;

	void Start ()
	{
		//Debug.Log ("World.Start()");

	}

	public void DoNextUpdate ()
	{
		CurrentDate = CurrentDate.AddHours (1);
				
		PlayerGroup.DoNextUpdate ();
				
		foreach (Sector s in WorldSectors) {
			s.DoNextUpdate ();
		}
							
		Debug.Log (CurrentDate);
	}

	public Sector GetSectorAtPosition (Vector3 origin)
	{
		if (origin.x < 0 || origin.y < 0 || origin.x > DimensionsX || origin.y > DimensionsY) {
			throw new System.ArgumentOutOfRangeException ("Vector3 origin", "Not in valid range: x = [0," + DimensionsX + "], y = [0," + DimensionsY + "]");
		}
				
		int x = Mathf.FloorToInt (origin.x);
		int y = Mathf.FloorToInt (origin.y);
		
        if(WorldSectors.GetUpperBound(0) < x || WorldSectors.GetUpperBound(1) < y)
        {
            throw new System.MissingMemberException("WorldSectors not initialized");
        }

		return WorldSectors [x, y];	
	}

	public Sector GetSectorFromScreenPos (Vector3 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay (screenPos);
		if (ray.origin.x > 0 && ray.origin.y > 0 && ray.origin.x < DimensionsX && ray.origin.y < DimensionsY) {			
			return GetSectorAtPosition (ray.origin);
		} 
				
		return null;
	}

    public Sector GetSectorFromCoords(int posX, int posY)
    {
        if (posX < 0 || posY < 0 || posX > DimensionsX || posY > DimensionsY)
        {
            throw new System.ArgumentOutOfRangeException("int[] position", "Not in valid range: x = [0," + DimensionsX + "], y = [0," + DimensionsY + "]");
         }

        return WorldSectors[posX, posY];
    }

    public Sector GetSectorFromCoords(int[] position)
    {
        if (position[0] < 0 || position[1] < 0 || position[0] > DimensionsX || position[1] > DimensionsY)
        {
            throw new System.ArgumentOutOfRangeException("int[] position", "Not in valid range: x = [0," + DimensionsX + "], y = [0," + DimensionsY + "]");
        }

        return WorldSectors[position[0], position[1]];
    }

	public void SetMinimapImage (Texture newTexture)
	{
		Debug.Log ("New Image Map!");
		Tex = newTexture;
		MapImage = GameObject.Find ("DebugMapImage").GetComponent<RawImage> ();
		MapImage.texture = Tex;
	}
}