using UnityEngine;
using System.Collections.Generic;

//
// LEVEL
//
// Provides some information about the makeup of a level.
//
public class Level 
	: MonoBehaviour 
{
	
	// References to prefabs that are used to reprentent the various
	// types of tiles.
	public GameObject UserBlock;
	public GameObject InternalBlock;
	public GameObject BlockScale;

	// References to GameObjects in the hierarchy that act as containers for
	// various level components.
	public GameObject UserTileContainer;
	public GameObject GeneratedTileContainer;
	public GameObject CollisionContainer;

	// The current tier that the editor is focused on.
	public int _currentY;
	
	// The current tile type that is selected in the editor.
	public TileType _currentTypeID;

	// The editor's mode.
	public EditorMode _editorMode = EditorMode.Viewer;

	// Internal value for helping to work out which tiles are furthest from the origin.
	const int MAX_GRID_SIZE = 1000;
	
	//
	int _cachedHighestXPositionTileInSector = -MAX_GRID_SIZE;

	//
	int _cachedHighestZPositionTileInSector = -MAX_GRID_SIZE;

	//
	int _cachedLowestXPositionTileInSector = MAX_GRID_SIZE;

	//
	int _cachedLowestZPositionTileInSector = MAX_GRID_SIZE;

	//
	int _cachedNumberOfTiles = 0;

	
	
	// Get returns the number of tiles in the active sector.  Returns 0 if there is no active sector.
	public int NumberOfTiles
	{
		get
		{
			return _cachedNumberOfTiles;
		}
	}

	//  Get returns the maximum Z tile position of all the tiles in the active sector.  Returns 0 if there is no active sector.
	public int MaxZLocationOfTiles
	{
		get
		{
			return _cachedHighestZPositionTileInSector;
		}

	}

	//  Get returns the maximum X tile position of all the tiles in the active sector.  Returns 0 if there is no active sector.
	public int MaxXLocationOfTiles
	{
		get
		{
			return _cachedHighestXPositionTileInSector;
		}

	}

	//  Get returns the minimum Z tile position of all the tiles in the active sector.  Returns 0 if there is no active sector.
	public int MinZLocationOfTiles
	{
		get
		{
			return _cachedLowestZPositionTileInSector;
		}
	}

	//  Get returns the minimum X tile position of all the tiles in the active sector.  Returns 0 if there is no active sector.
	public int MinXLocationOfTiles
	{
		get
		{
			return _cachedLowestXPositionTileInSector;
		}
	}


	// Update is called once per frame
	void Update () {
		this.transform.localPosition = Vector3.zero;
		this.transform.localEulerAngles = Vector3.zero;
		this.transform.localScale = new Vector3(1,1,1);
	}

	public void UpdateCachedValuesForMaxAndMinTilePositions()
	{
		_cachedHighestXPositionTileInSector = -MAX_GRID_SIZE;
		_cachedHighestZPositionTileInSector = -MAX_GRID_SIZE;
		_cachedLowestXPositionTileInSector = MAX_GRID_SIZE;
		_cachedLowestZPositionTileInSector = MAX_GRID_SIZE;

		if (UserTileContainer != null)
		{
			Tile[] tiles = UserTileContainer.GetComponentsInChildren<Tile>();

			foreach (Tile tile in tiles)
			{
				Point3 p = tile.Location;

				if (p.X > _cachedHighestXPositionTileInSector)
					_cachedHighestXPositionTileInSector = p.X;
				if (p.X < _cachedLowestXPositionTileInSector)
					_cachedLowestXPositionTileInSector = p.X;

				if (p.Z > _cachedHighestZPositionTileInSector)
					_cachedHighestZPositionTileInSector = p.Z;
				if (p.Z < _cachedLowestZPositionTileInSector)
					_cachedLowestZPositionTileInSector = p.Z;
			}

			_cachedNumberOfTiles = tiles.Length;
		}
	}
}
