using System;
using UnityEngine;
using System.Collections.Generic;

//
// INTERNAL TILE
//
// An internal tile is an implementation of abstract tile MonoBehaviour.
// The system internally uses this type of tile to store data about a map.
// This class contains all the logic for working out how to select the correct
// combination of cladding meshes.
//
public class InternalTile
    : Tile
{

    #region PUBLIC MEMEBERS FOR THE INSPECTOR

    // References to prefabs in project view that represent different types of cladding mesh.
    // GameObjects of these prefabs get instantiated when needed.
    public GameObject corePrefab;
    public GameObject cladding1Prefab;
    public GameObject cladding2Prefab;
    public GameObject cladding3Prefab;
    public GameObject cladding4Prefab;
    public GameObject cladding5Prefab;

    // Each of the prefabs above has a SegmentManager script on it.  When the GameObjects are
    // instantiated we cache references to that script.
    public SegmentManager coreManager;
    public SegmentManager cladding1Manager;
    public SegmentManager cladding2Manager;
    public SegmentManager cladding3Manager;
    public SegmentManager cladding4Manager;
    public SegmentManager cladding5Manager;

    // Each internal tile is aware of all of the tiles that surround it and exist on the same tier.
    public TileType TypeOfNorthernNeighbour;
    public TileType TypeOfNorthEasternNeighbour;
    public TileType TypeOfEasternNeighbour;
    public TileType TypeOfSouthEasternNeighbour;
    public TileType TypeOfSouthernNeighbour;
    public TileType TypeOfSouthWesternNeighbour;
    public TileType TypeOfWesternNeighbour;
    public TileType TypeOfNorthWesternNeighbour;


    #endregion

    #region PRIVATE MEMBERS
    
    // Temp value to specify if we want to hide the core segements and display the user segments instead.
    public static bool HideSomeColours = true;

    #endregion

    #region PRIVATE VOID FUNCTIONS

    // INIT INTERNAL DATA
    // This gets called after a new user tile is instantiated.
    // - location represents the grid space that the tile sits in
    // - type
    // - pos is the position in world space of the tile, really this is just be calculated here from the Point3 location.
    public override void InitInternalData(Point3 location, TileType type, Vector3 pos)
    {
        this.gameObject.transform.position = pos;
        Location = location;
        _typeID = type;

        this.gameObject.name += location.ToString();
        CreatePrefabAndSetManager(corePrefab, ref coreManager);
        CreatePrefabAndSetManager(cladding1Prefab, ref cladding1Manager);
        CreatePrefabAndSetManager(cladding2Prefab, ref cladding2Manager);
        CreatePrefabAndSetManager(cladding3Prefab, ref cladding3Manager);
        CreatePrefabAndSetManager(cladding4Prefab, ref cladding4Manager);
        CreatePrefabAndSetManager(cladding5Prefab, ref cladding5Manager);
    }

    // CREATE PREFAB AND SET MANAGER
    // Helper function for the InitINternalDate function to save repeating code.
    public void CreatePrefabAndSetManager(GameObject prefab, ref SegmentManager manager)
    {
        if (manager == null)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            go.transform.parent = gameObject.transform;
            go.transform.localPosition = Vector3.zero;
            SegmentManager mng = go.GetComponent<SegmentManager>();
            manager = mng;
        }
    }

    // UPDATE MESHES
    // Takes the current state of the tile and sets its internal state to show the correct
    // combination of meshes.
    public void UpdateMeshes()
    {
        // disable all
        coreManager.DisableAll();
        cladding1Manager.DisableAll();
        cladding2Manager.DisableAll();
        cladding3Manager.DisableAll();
        cladding4Manager.DisableAll();
        cladding5Manager.DisableAll();

        switch (_typeID)
        {
            case TileType.CladdingTile: DoCladdingTile(); break;
            case TileType.Cuboid: DoCuboid(); break;
            case TileType.TriangularPrismNW: TriangularPrismNW(); break;
            case TileType.TriangularPrismNE: TriangularPrismNE(); break;
            case TileType.TriangularPrismSE: TriangularPrismSE(); break;
            case TileType.TriangularPrismSW: TriangularPrismSW(); break;
            default: break;
        }
    }

    // DO CLADDING TILE
    // Work out what sub meshes to show if this tile is a cladding tile.
    void DoCladdingTile()
    {
        //1
        if (IsNorthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NNE, true);
        else if (IsEasternNeighbourSolid || IsNorthEasternNeighbourSolid || DoesNorthernNeighbourMeetATrianglesTipOnItsRight || DoesEasternNeighbourMeetATrianglesTipOnItsTop)
            cladding4Manager.SetVisible(SegmentID.NNE, true);

        //2
        if (IsEasternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NEE, true);
        else if (IsNorthernNeighbourSolid || IsNorthEasternNeighbourSolid || DoesNorthernNeighbourMeetATrianglesTipOnItsRight || DoesEasternNeighbourMeetATrianglesTipOnItsTop)
            cladding4Manager.SetVisible(SegmentID.NEE, true);

        //3
        if (IsEasternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SEE, true);
        else if (IsSouthernNeighbourSolid || IsSouthEasternNeighbourSolid || DoesEasternNeighbourMeetATrianglesTipOnItsBottom || DoesSouthernNeighbourMeetATrianglesTipOnItsRight)
            cladding4Manager.SetVisible(SegmentID.SEE, true);

        //4
        if (IsSouthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SSE, true);
        else if (IsEasternNeighbourSolid || IsSouthEasternNeighbourSolid || DoesEasternNeighbourMeetATrianglesTipOnItsBottom || DoesSouthernNeighbourMeetATrianglesTipOnItsRight)
            cladding4Manager.SetVisible(SegmentID.SSE, true);

        //5
        if (IsSouthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SSW, true);
        else if (IsWesternNeighbourSolid || IsSouthWesternNeighbourSolid || DoesSouthernNeighbourMeetATrianglesTipOnItsLeft || DoesWesternNeighbourMeetATrianglesTipOnItsBottom)
            cladding4Manager.SetVisible(SegmentID.SSW, true);

        //6
        if (IsWesternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SWW, true);
        else if (IsSouthernNeighbourSolid || IsSouthWesternNeighbourSolid || DoesSouthernNeighbourMeetATrianglesTipOnItsLeft || DoesWesternNeighbourMeetATrianglesTipOnItsBottom)
            cladding4Manager.SetVisible(SegmentID.SWW, true);

        //7
        if (IsWesternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NWW, true);
        else if (IsNorthernNeighbourSolid || IsNorthWesternNeighbourSolid || DoesWesternNeighbourMeetATrianglesTipOnItsTop || DoesNorthernNeighbourMeetATrianglesTipOnItsLeft)
            cladding4Manager.SetVisible(SegmentID.NWW, true);

        //8
        if (IsNorthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NNW, true);
        else if (IsWesternNeighbourSolid || IsNorthWesternNeighbourSolid || DoesWesternNeighbourMeetATrianglesTipOnItsTop || DoesNorthernNeighbourMeetATrianglesTipOnItsLeft)
            cladding4Manager.SetVisible(SegmentID.NNW, true);



    }

    // DO CUBOID
    // Work out what sub meshes to show if this tile is a cuboid.
    void DoCuboid()
    {
        if (!HideSomeColours)
            coreManager.EnableAll();
    }

    // DO TRIANGULAR PRISM NW
    // Work out what sub meshes to show if this tile is a north western triangular prism.
    void TriangularPrismNW()
    {
        //1
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NNE, true);


        //2
        if (IsEasternNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.NEE, true);
        else
            cladding2Manager.SetVisible(SegmentID.NEE, true);

        //3
        cladding3Manager.SetVisible(SegmentID.SEE, true);
        if (IsEasternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SEE, true);
        else if (IsSouthernNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.SEE, true);

        //4
        cladding3Manager.SetVisible(SegmentID.SSE, true);
        if (IsSouthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SSE, true);
        else if (IsEasternNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.SSE, true);

        //5
        if (IsSouthernNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.SSW, true);
        else
            cladding2Manager.SetVisible(SegmentID.SSW, true);


        //6
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SWW, true);

        //7
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NWW, true);

        //8
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NNW, true);

    }

    // DO TRIANGULAR PRISM NE
    // Work out what sub meshes to show if this tile is a north eastern triangular prism.
    void TriangularPrismNE()
    {
        //1
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NNE, true);

        //2
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NEE, true);

        //3
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SEE, true);

        //4
        if (IsSouthernNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.SSE, true);
        else
            cladding2Manager.SetVisible(SegmentID.SSE, true);

        //5
        cladding3Manager.SetVisible(SegmentID.SSW, true);
        if (IsSouthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SSW, true);
        else if (IsWesternNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.SSW, true);

        //6
        cladding3Manager.SetVisible(SegmentID.SWW, true);
        if (IsWesternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.SWW, true);
        else if (IsSouthernNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.SWW, true);

        //7
        if (IsWesternNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.NWW, true);
        else
            cladding2Manager.SetVisible(SegmentID.NWW, true);

        //8
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NNW, true);

    }

    // DO TRIANGULAR PRISM SE
    // Work out what sub meshes to show if this tile is a south eastern triangular prism.
    void TriangularPrismSE()
    {
        //1
        if (IsNorthernNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.NNE, true);
        else
            cladding2Manager.SetVisible(SegmentID.NNE, true);

        //2
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NEE, true);

        //3
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SEE, true);

        //4
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SSE, true);

        //5
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SSW, true);

        //6
        if (IsWesternNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.SWW, true);
        else
            cladding2Manager.SetVisible(SegmentID.SWW, true);

        //7
        cladding3Manager.SetVisible(SegmentID.NWW, true);
        if (IsWesternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NWW, true);
        else if (IsNorthernNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.NWW, true);

        //8
        cladding3Manager.SetVisible(SegmentID.NNW, true);
        if (IsNorthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NNW, true);
        else if (IsWesternNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.NNW, true);
    }

    // DO TRIANGULAR PRISM SW
    // Work out what sub meshes to show if this tile is a south western triangular prism.
    void TriangularPrismSW()
    {
        //1
        cladding3Manager.SetVisible(SegmentID.NNE, true);
        if (IsNorthernNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NNE, true);
        else if (IsEasternNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.NNE, true);

        //2
        cladding3Manager.SetVisible(SegmentID.NEE, true);
        if (IsEasternNeighbourSolid)
            cladding1Manager.SetVisible(SegmentID.NEE, true);
        else if (IsNorthernNeighbourSolid)
            cladding4Manager.SetVisible(SegmentID.NEE, true);

        //3
        if (IsEasternNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.SEE, true);
        else
            cladding2Manager.SetVisible(SegmentID.SEE, true);

        //4
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SSE, true);

        //5
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SSW, true);

        //6
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.SWW, true);

        //7
        if (!HideSomeColours)
            coreManager.SetVisible(SegmentID.NWW, true);

        //8
        if (IsNorthernNeighbourSolid)
            cladding5Manager.SetVisible(SegmentID.NNW, true);
        else
            cladding2Manager.SetVisible(SegmentID.NNW, true);
    }

    #endregion

    #region PRIVATE GET FUNCTIONS
    
    // IS NORTHERN NEIGHBOUR SOLID
    // Returns true if this tile's northern neighbour backs directly on to this tile.
    bool IsNorthernNeighbourSolid
    {
        get
        {
            return (
                TypeOfNorthernNeighbour == TileType.Cuboid ||
                TypeOfNorthernNeighbour == TileType.TriangularPrismSE ||
                TypeOfNorthernNeighbour == TileType.TriangularPrismSW);
        }
    }

    // IS EASTERN NEIGHBOUR SOLID
    // Returns true if this tile's eastern neighbour backs directly on to this tile.
    bool IsEasternNeighbourSolid
    {
        get
        {
            return (
                TypeOfEasternNeighbour == TileType.Cuboid ||
                TypeOfEasternNeighbour == TileType.TriangularPrismNW ||
                TypeOfEasternNeighbour == TileType.TriangularPrismSW);
        }
    }

    // IS SOUTHERN NEIGHBOUR SOLID
    // Returns true if this tile's southern neighbour backs directly on to this tile.
    bool IsSouthernNeighbourSolid
    {
        get
        {
            return (
                TypeOfSouthernNeighbour == TileType.Cuboid ||
                TypeOfSouthernNeighbour == TileType.TriangularPrismNE ||
                TypeOfSouthernNeighbour == TileType.TriangularPrismNW);
        }
    }

    // IS WESTERN NEIGHBOUR SOLID
    // Returns true if this tile's western neighbour backs directly on to this tile.
    bool IsWesternNeighbourSolid
    {
        get
        {
            return (
                TypeOfWesternNeighbour == TileType.Cuboid ||
                TypeOfWesternNeighbour == TileType.TriangularPrismNE ||
                TypeOfWesternNeighbour == TileType.TriangularPrismSE);
        }
    }

    // IS NORTH EASTERN NEIGHBOUR SOLID
    // Returns true if this tile's north eastern neighbour backs directly on to this tile.
    bool IsNorthEasternNeighbourSolid
    {
        get
        {
            return (
                TypeOfNorthEasternNeighbour == TileType.Cuboid ||
                TypeOfNorthEasternNeighbour == TileType.TriangularPrismNW ||
                TypeOfNorthEasternNeighbour == TileType.TriangularPrismSE ||
                TypeOfNorthEasternNeighbour == TileType.TriangularPrismSW);
        }
    }

    // IS SOUTH EASTERN NEIGHBOUR SOLID
    // Returns true if this tile's south eastern neighbour backs directly on to this tile.
    bool IsSouthEasternNeighbourSolid
    {
        get
        {
            return (
                TypeOfSouthEasternNeighbour == TileType.Cuboid ||
                TypeOfSouthEasternNeighbour == TileType.TriangularPrismNW ||
                TypeOfSouthEasternNeighbour == TileType.TriangularPrismNE ||
                TypeOfSouthEasternNeighbour == TileType.TriangularPrismSW);
        }
    }

    // IS SOUTH WESTERN NEIGHBOUR SOLID
    // Returns true if this tile's south western neighbour backs directly on to this tile.
    bool IsSouthWesternNeighbourSolid
    {
        get
        {
            return (
                TypeOfSouthWesternNeighbour == TileType.Cuboid ||
                TypeOfSouthWesternNeighbour == TileType.TriangularPrismNW ||
                TypeOfSouthWesternNeighbour == TileType.TriangularPrismNE ||
                TypeOfSouthWesternNeighbour == TileType.TriangularPrismSE);
        }
    }

    // IS NORTH WESTERN NEIGHBOUR SOLID
    // Returns true if this tile's north western neighbour backs directly on to this tile.
    bool IsNorthWesternNeighbourSolid
    {
        get
        {
            return (
                TypeOfNorthWesternNeighbour == TileType.Cuboid ||
                TypeOfNorthWesternNeighbour == TileType.TriangularPrismNE ||
                TypeOfNorthWesternNeighbour == TileType.TriangularPrismSE ||
                TypeOfNorthWesternNeighbour == TileType.TriangularPrismSW);
        }
    }

    // DOES NORTHERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS LEFT
    // Returns true if the left nothern side of this tile backs directly on to a triangle's tip.
    bool DoesNorthernNeighbourMeetATrianglesTipOnItsLeft
    {
        get
        {
            return (TypeOfNorthernNeighbour == TileType.TriangularPrismNW);
        }
    }

    // DOES NORTHERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS RIGHT
    // Returns true if the right nothern side of this tile backs directly on to a triangle's tip.
    bool DoesNorthernNeighbourMeetATrianglesTipOnItsRight
    {
        get
        {
            return (TypeOfNorthernNeighbour == TileType.TriangularPrismNE);
        }
    }

    // DOES EASTERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS TOP
    // Returns true if the top eastern side of this tile backs directly on to a triangle's tip.
    bool DoesEasternNeighbourMeetATrianglesTipOnItsTop
    {
        get
        {
            return (TypeOfEasternNeighbour == TileType.TriangularPrismNE);
        }
    }

    // DOES EASTERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS BOTTOM
    // Returns true if the bottom eastern side of this tile backs directly on to a triangle's tip.
    bool DoesEasternNeighbourMeetATrianglesTipOnItsBottom
    {
        get
        {
            return (TypeOfEasternNeighbour == TileType.TriangularPrismSW);
        }
    }

    // DOES SOUTHERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS LEFT
    // Returns true if the left southern side of this tile backs directly on to a triangle's tip.
    bool DoesSouthernNeighbourMeetATrianglesTipOnItsLeft
    {
        get
        {
            return (TypeOfSouthernNeighbour == TileType.TriangularPrismSW);
        }
    }

    // DOES SOUTHERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS RIGHT
    // Returns true if the right southern side of this tile backs directly on to a triangle's tip.
    bool DoesSouthernNeighbourMeetATrianglesTipOnItsRight
    {
        get
        {
            return (TypeOfSouthernNeighbour == TileType.TriangularPrismSE);
        }
    }

    // DOES WESTERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS TOP
    // Returns true if the top western side of this tile backs directly on to a triangle's tip.
    bool DoesWesternNeighbourMeetATrianglesTipOnItsTop
    {
        get
        {
            return (TypeOfWesternNeighbour == TileType.TriangularPrismNW);
        }
    }

    // DOES WESTERN NEIGHBOUR MEET A TRAIANGLE'S TIP ON ITS BOTTOM
    // Returns true if the bottom western side of this tile backs directly on to a triangle's tip.
    bool DoesWesternNeighbourMeetATrianglesTipOnItsBottom
    {
        get
        {
            return (TypeOfWesternNeighbour == TileType.TriangularPrismSW);
        }
    }



    #endregion



}



