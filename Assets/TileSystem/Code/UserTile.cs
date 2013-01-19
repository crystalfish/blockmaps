using System;
using UnityEngine;
using System.Collections.Generic;


//
// USER TILE
//
// A user tile is an implementation of abstract tile MonoBehaviour.
// The system uses this type of tile as a simplified representation for the user.
//
public class UserTile
    : Tile
{
    // This is a list of GameObjects each of which represent one of the user tile types.
    // This is public so that the values can be assigned directly in the inspectors and
    // then saved to a prefab.
    public GameObject[] _typeGOs = new GameObject[Tile.NUM_USER_TYPES];

    // INIT INTERNAL DATA
    // This gets called after a new user tile is instantiated.
    // - location represents the grid space that the tile sits in
    // - type
    // - pos is the position in world space of the tile, really this is just be calculated here from the Point3 location.
    public override void InitInternalData(Point3 location, TileType type, Vector3 pos)
    {
        this.gameObject.name += location.ToString();
        this.gameObject.transform.position = pos;
        Location = location;
        _typeID = type;

        for (int i = 0; i < _typeGOs.Length; ++i)
        {
            _typeGOs[i].GetComponentInChildren<MeshRenderer>().enabled = (_typeID == (TileType)i);
        }
    }
}