using System;
using UnityEngine;
using System.Collections.Generic;

//
// TILE (a tile occupies a single grid space)
//
// Abstract Tile MonoBehaviour.  There are two types of tiles:
// - User Tiles (these are just the shapes placed by the user)
// - Internal Tiles (these are the systems internal representation of a tiles, each internal tile is made from 8 segments )
//
public abstract class Tile
    : MonoBehaviour
{
    // All tiles have a location on the grid represented by three ints.  The reason for not
    // using the Point3 struct here is that Unity cannot serialize struct >_<
    [UnityEngine.SerializeField]
    protected int _x;
    [UnityEngine.SerializeField]
    protected int _y;
    [UnityEngine.SerializeField]
    protected int _z;

    // All tiles have a type
    [UnityEngine.SerializeField]
    protected TileType _typeID;

    // This just represents the number of different shapes the user can place.  Needs so
    // certain behaviours can create arrays for the correct number of GameObjects.
    public const int NUM_USER_TYPES = 5;

    // Public accessor for the grid space that the tile occupies via a Point3 struct.
    public Point3 Location
    {
        get
        {
            return new Point3(_x,_y,_z);
        }
        set
        {
            _x = value.X; _y = value.Y; _z = value.Z;
        }
    }

    // Public accessor for the type of the tile.
    public TileType TypeID
    {
        get
        {
            return _typeID;
        }
    }

    // INIT INTERNAL DATA
    // This gets called after a new user tile is instantiated.
    // - location represents the grid space that the tile sits in
    // - type
    // - pos is the position in world space of the tile, really this is just be calculated here from the Point3 location.
    public abstract void InitInternalData(Point3 location, TileType type, Vector3 pos);


}
