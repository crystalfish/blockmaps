using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//
// LEVEL EDITOR
//
// This class provides contains all the logic for user interface.
//
[CustomEditor(typeof(Level))]
public class LevelEditor 
    : Editor
{
    #region PRIVATE VARIABLES

    // 
    int _onInspectorGuiCallCount;

    //
    int _onSceneGuiCallCount;


    // The position of the brush on the sector grid.
    Point3 _brushPosition;

    // The position where a ray from the current mouse position hits the base of the current plane.
    Vector3 _vec3OnBaseOfCurrentPlane;

    // This is used as a reference to any GameObject that may have been deactivated temporarily so it can reactivate it later.
    GameObject _tileToBeReactivated;

    Texture2D[] _typeTextures= new Texture2D[Tile.NUM_USER_TYPES];

    // A cached casted version of this editor's target.
    Level _cachedTarget;

    const int GRID_BOUNDARY = 2;


    #endregion

    #region PRIVATE PROPERTIES

    // IS BRUSH ON ACTIVE GRID
    // Returns true if the brush is on the sector grid of the active TileContainer.  
    // Also returns true if the active sector has zero tiles.  Returns false if there 
    // is no active TileContainer.
    bool IsBrushOnGrid
    {
        get
        {
            if (Target.NumberOfTiles == 0)
                return true;

            return (_brushPosition.X > Target.MinXLocationOfTiles - GRID_BOUNDARY - 1) &&
                (_brushPosition.Y > Target.MinZLocationOfTiles - GRID_BOUNDARY - 1) &&
                (_brushPosition.X < Target.MaxXLocationOfTiles + GRID_BOUNDARY + 1) &&
                (_brushPosition.Y < Target.MaxZLocationOfTiles + GRID_BOUNDARY + 1);
        }
    }

    // BASE OF CURRENT LAYER
    // Where in the Y axis is the base of the current layer?
    float BaseOfCurrentLayer
    {
        get
        {
            float tileHeight = Target.BlockScale.transform.localScale.y;
            return ((float)Target._currentY - 0.5f) * tileHeight;
        }
    }

    // TARGET
    // private property to save using 'as' all the time
    Level Target
    {
        get
        {
            if (_cachedTarget == null)
                _cachedTarget = target as Level;
            return _cachedTarget;
        }
    }

    #endregion

    #region PRIVATE GET FUNCTIONS
    // GET TILE POSITION FROM LOCATION
    //
    Vector3 GetTilePositionFromLocation(Point3 point)
    {
        Vector3 newVec;
        newVec.x = point.X * Target.BlockScale.transform.localScale.x;
        newVec.y = Target.BlockScale.transform.localScale.y * (float)point.Y;
        newVec.z = point.Z * Target.BlockScale.transform.localScale.z;
        return newVec;
    }

    // GET OFFSET X
    //
    float GetOffsetX(int brushPoint)
    {
        if (brushPoint < 0f)
            return Target.BlockScale.transform.localScale.x / 2f;
        else if (brushPoint > 0f)
            return -Target.BlockScale.transform.localScale.x / 2f;

        return 0f;

    }

    // GET OFFSET Z
    //
    float GetOffsetZ(int brushPoint)
    {
        if (brushPoint < 0f)
            return Target.BlockScale.transform.localScale.z / 2f;
        else if (brushPoint > 0f)
            return -Target.BlockScale.transform.localScale.z / 2f;

        return 0f;

    }

    // GET TYPE FROM TILE THAT COULD BE NULL
    //
    TileType GetTypeFromTileThatCouldBeNull(InternalTile it)
    {
        if (it != null)
            return it.TypeID;
        else
            return TileType.Unassigned;
    }

    // GET INTERNAL TILE AT LOCATION
    //
    InternalTile GetInternalTileAtLocation(Point3 loc)
    {
        List<InternalTile> allInternalTiles = new List<InternalTile>(Target.GeneratedTileContainer.GetComponentsInChildren<InternalTile>());
        InternalTile item = allInternalTiles.Find(
        delegate(InternalTile dTile)
        {
            if (dTile.Location == loc)
                return true;

            return false;
        }
        );

        return item;
    }

    #endregion

    #region UNITY CALLBACKS

    // ON INSPECTOR GUI
    // For adding buttons to the inspector
    public override void OnInspectorGUI()
    {
        _onInspectorGuiCallCount++;

        EditorGUILayout.Separator();

        Target.UpdateCachedValuesForMaxAndMinTilePositions();
        GUIContent[] toolbarOptions = new GUIContent[2];
        toolbarOptions[0] = new GUIContent("View");
        toolbarOptions[1] = new GUIContent("Edit");
        Target._editorMode = (EditorMode)GUILayout.SelectionGrid((int)Target._editorMode, toolbarOptions, 2);


        // Editor notice
        switch (Target._editorMode)
        {
            case EditorMode.Viewer: OnInspectorGUI_Viewer(); break;
            case EditorMode.Editor: OnInspectorGUI_Editor(); break;
        }
    }

    // ON INSPECTOR GUI: VIEWER MODE
    // Sub function
    void OnInspectorGUI_Viewer()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Generate Collision Mesh"))
        {
            // each of the temp tiles has a mesh filter
            MeshFilter[] meshFilters =
                Target.UserTileContainer.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                //meshFilters[i].gameObject.active = false;
                i++;
            }

            GameObject collisionMesh = new GameObject("CollisionMesh");
            MeshFilter mf = collisionMesh.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
            mf.sharedMesh.CombineMeshes(combine);
            collisionMesh.AddComponent<MeshCollider>();
            //transform.gameObject.active = true;
            collisionMesh.transform.parent = Target.CollisionContainer.transform;

        }
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        InternalTile.HideSomeColours = GUILayout.Toggle(InternalTile.HideSomeColours, "HideColours");
        if (GUILayout.Button("Generate Cladding"))
        {
            GenerateCladding(true);
        }

        GUILayout.EndHorizontal();
    }

    // ON INSPECTOR GUI: EDITOR MODE
    // Sub function
    void OnInspectorGUI_Editor()
    {
        // here the user can select which layer they are editing.
        GUILayout.Space(10);
        GUILayout.Label("Current Y: ");
        EditorGUILayout.BeginHorizontal();



        Target._currentY = (int)GUILayout.HorizontalSlider((int)Target._currentY, -5, 5);
        GUILayout.TextField(Target._currentY.ToString());
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < Tile.NUM_USER_TYPES; ++i)
        {
            string str = string.Format("Assets\\TileSystem\\Resources\\Textures\\type{0}.png", i + 1);

            _typeTextures[i] = (Texture2D)Resources.LoadAssetAtPath(
            str, typeof(Texture2D));

        }

        GUILayout.Space(10);
        GUILayout.Label("Current tile type: ");
        EditorGUILayout.BeginHorizontal();
        GUIContent[] toolbarOptions2 = new GUIContent[Tile.NUM_USER_TYPES];
        for (int i = 0; i < Tile.NUM_USER_TYPES; ++i)
        {
            if (_typeTextures[i] != null)
            {
                toolbarOptions2[i] = new GUIContent(
                    string.Format("Type {0}", i),
                    _typeTextures[i]);
            }
            else
            {
                toolbarOptions2[i] = new GUIContent(
                    string.Format("Type {0}", i));
            }
        }

        Target._currentTypeID =
            (TileType)GUILayout.SelectionGrid((int)Target._currentTypeID, toolbarOptions2, 3);

        EditorGUILayout.EndHorizontal();
    }

    // ON SCENE GUI
    // Unity calls this when it wants to let this Editor handle an event in the scene view.
    public void OnSceneGUI()
    {
        _onSceneGuiCallCount++;

        DrawEditorSceneOverlay();

        DrawPlacementGrid(false);

        // Editor notice
        switch (Target._editorMode)
        {
            case EditorMode.Viewer: 
                break;

            case EditorMode.Editor: 
                UpdateBrushPosition();
                CheckForEvents();
                break;
        }

        DrawDebugOverlay();

    }

    #endregion

    #region PRIVATE VOID FUNCTIONS
    
    // GENERATE CLADDING
    //
    void GenerateCladding(bool zCladding)
    {

        UserTile[] uts = Target.UserTileContainer.GetComponentsInChildren<UserTile>();


        // add a generated tile for each ut
        foreach (UserTile ut in uts)
        {
            GameObject go = GameObject.Instantiate(Target.InternalBlock) as GameObject;
            go.name = "internal";
            go.transform.parent = Target.GeneratedTileContainer.transform;
            InternalTile it = go.GetComponent<InternalTile>();
            it.InitInternalData(ut.Location, ut.TypeID, GetTilePositionFromLocation(ut.Location));
        }

        List<InternalTile> originalTiles = new List<InternalTile>(Target.GeneratedTileContainer.GetComponentsInChildren<InternalTile>());
        List<InternalTile> newTiles = new List<InternalTile>();

        // now we need to add extra cladding tiles
        foreach (InternalTile ut in originalTiles)
        {
            // does a tile exist in any of the surrounding tiles

            TryLocation(ut.Location + new Point3(-1, 0, 0), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(-1, 0, -1), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(0, 0, -1), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(-1, 0, 1), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(1, 0, -1), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(1, 0, 0), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(1, 0, 1), originalTiles, newTiles);
            TryLocation(ut.Location + new Point3(0, 0, 1), originalTiles, newTiles);
        }


        // now we need to notify each internal tile of it's neighbours
        List<InternalTile> allTiles = new List<InternalTile>(Target.GeneratedTileContainer.GetComponentsInChildren<InternalTile>());
        foreach (InternalTile ut in allTiles)
        {
            // does a tile exist in any of the surrounding tiles

            InternalTile western = GetInternalTileAtLocation(ut.Location + new Point3(-1, 0, 0));
            ut.TypeOfWesternNeighbour = GetTypeFromTileThatCouldBeNull(western);

            InternalTile southWestern = GetInternalTileAtLocation(ut.Location + new Point3(-1, 0, -1));
            ut.TypeOfSouthWesternNeighbour = GetTypeFromTileThatCouldBeNull(southWestern);

            InternalTile southern = GetInternalTileAtLocation(ut.Location + new Point3(0, 0, -1));
            ut.TypeOfSouthernNeighbour = GetTypeFromTileThatCouldBeNull(southern);

            InternalTile northWestern = GetInternalTileAtLocation(ut.Location + new Point3(-1, 0, 1));
            ut.TypeOfNorthWesternNeighbour = GetTypeFromTileThatCouldBeNull(northWestern);

            InternalTile southEastern = GetInternalTileAtLocation(ut.Location + new Point3(1, 0, -1));
            ut.TypeOfSouthEasternNeighbour = GetTypeFromTileThatCouldBeNull(southEastern);

            InternalTile eastern = GetInternalTileAtLocation(ut.Location + new Point3(1, 0, 0));
            ut.TypeOfEasternNeighbour = GetTypeFromTileThatCouldBeNull(eastern);

            InternalTile northEastern = GetInternalTileAtLocation(ut.Location + new Point3(1, 0, 1));
            ut.TypeOfNorthEasternNeighbour = GetTypeFromTileThatCouldBeNull(northEastern);

            InternalTile northern = GetInternalTileAtLocation(ut.Location + new Point3(0, 0, 1));
            ut.TypeOfNorthernNeighbour = GetTypeFromTileThatCouldBeNull(northern);
        }


        // new we ask each internal tile to update its meshes
        foreach (InternalTile it in allTiles)
            it.UpdateMeshes();

        if (!InternalTile.HideSomeColours)
        {
            foreach (UserTile ut in uts)
                ut.gameObject.SetActiveRecursively(false);
        }

    }



    // TRY LOCATION
    //
    void TryLocation(Point3 location, List<InternalTile> original, List<InternalTile> newones)
    {
        // Try and find a tile with this location
        InternalTile item = original.Find(
            delegate(InternalTile dTile)
            {
                if (dTile.Location == location)
                    return true;

                return false;
            }
            );

        // if there is not a tile in the original list that matches this location
        if (item == null)
        {
            //might not be one,check the new list

            InternalTile item2 = newones.Find(
                delegate(InternalTile dTile)
                {
                    if (dTile.Location == location)
                        return true;

                    return false;
                }
                );

            if (item2 == null)
            {
                //ok, now we add one
                GameObject go = GameObject.Instantiate(Target.InternalBlock) as GameObject;
                go.name = "new";
                go.transform.parent = Target.GeneratedTileContainer.transform;
                InternalTile it = go.GetComponent<InternalTile>();
                it.InitInternalData(location, TileType.CladdingTile, GetTilePositionFromLocation(location));
                newones.Add(it);

            }

        }


    }

    // UPDATE BRUSH POSITION
    // Works out the position of the brush on the grid
    void UpdateBrushPosition()
    {
        // Get a ray from the mouse
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Work out the point on the XZ plane that the ray hit
        _vec3OnBaseOfCurrentPlane.z = (Mathf.Tan(
            Mathf.Atan(ray.direction.z / -ray.direction.y)) * (ray.origin.y - BaseOfCurrentLayer))
            + ray.origin.z;
        _vec3OnBaseOfCurrentPlane.y = BaseOfCurrentLayer;

        _vec3OnBaseOfCurrentPlane.x = (Mathf.Tan(
            Mathf.Atan(ray.direction.x / -ray.direction.y)) * (ray.origin.y - BaseOfCurrentLayer))
            + ray.origin.x;


        Vector3 temp = _vec3OnBaseOfCurrentPlane;
        if (temp.x > 0)
        {
            temp += new Vector3(Target.BlockScale.transform.localScale.x / 2f, 0, 0);
        }
        else if (temp.x < 0)
        {
            temp += new Vector3(-Target.BlockScale.transform.localScale.x / 2f, 0, 0);
        }

        if (temp.z > 0)
        {
            temp += new Vector3(0, 0, Target.BlockScale.transform.localScale.z / 2f);
        }
        else if (temp.z < 0)
        {
            temp += new Vector3(0, 0, -Target.BlockScale.transform.localScale.z / 2f);
        }

        temp.x /= Target.BlockScale.transform.localScale.x;
        temp.y += Target.BlockScale.transform.localScale.y / 2f;
        temp.y /= Target.BlockScale.transform.localScale.y;
        temp.z /= Target.BlockScale.transform.localScale.z;

        _brushPosition = new Point3((int)temp.x, (int)temp.y, (int)temp.z);


    }

    // CHECK FOR EVENTS
    //
    void CheckForEvents()
    {
        // Only do this if we are on the grid
        if (!this.IsBrushOnGrid)
            return;

        Event currentEvnt = Event.current;

        if (currentEvnt.type == EventType.MouseUp)
        {
            //if (currentEvnt.button == 0)
            //    LeftClick(currentEvnt);

            if (currentEvnt.button == 1)
                RightClick(currentEvnt);

            if (currentEvnt.button == 2)
                MiddleClick(currentEvnt);


        }
    }


    // RIGHT CLICK
    //
    void RightClick(Event evnt)
    {

        // get a list of all tiles in the level
        List<UserTile> tileList = new List<UserTile>(
            Target.UserTileContainer.GetComponentsInChildren<UserTile>());

        // Try and find a tile with this location
        UserTile item = tileList.Find(
            delegate(UserTile dTile)
            {
                if (dTile.Location == _brushPosition)
                    return true;

                return false;
            }
            );


        // Check to see if the tile already exists, if it does
        // then early out
        if (item)
        {
            return;
        }

        Object prefab = Target.UserBlock;
        GameObject newTile = EditorUtility.InstantiatePrefab(prefab) as GameObject;

        newTile.name = "TileInstance";

        newTile.transform.parent = Target.UserTileContainer.transform;

        UserTile tempTileBehaviour = newTile.GetComponent<UserTile>();

        tempTileBehaviour.InitInternalData(_brushPosition, Target._currentTypeID, GetTilePositionFromLocation(_brushPosition));

        Target.UpdateCachedValuesForMaxAndMinTilePositions();
    }

    // MIDDLE CLICK
    //
    void MiddleClick(Event evnt)
    {
        UserTile[] tiles = Target.UserTileContainer.GetComponentsInChildren<UserTile>();
        List<UserTile> tileList = new List<UserTile>(tiles);

        UserTile item = tileList.Find(
            delegate(UserTile dTile)
            {
                if (dTile.Location == _brushPosition)
                    return true;

                return false;
            }
            );

        // Check to see if the tile already exists, if it does
        // then delete it.
        if (item != null)
        {
            GameObject.DestroyImmediate(item.gameObject);
        }

        Target.UpdateCachedValuesForMaxAndMinTilePositions();
    }

    // DRAW DEBUG OVERLAY
    //
    void DrawDebugOverlay()
    {
        Handles.BeginGUI();

        // SectorEditor OnInspectorGUI call count
        string str = string.Format("LevelEditor OnInspectorGUI call count: {0}",
            _onInspectorGuiCallCount);
        GUILayout.Label(str);

        // SectorEditor OnSceneGUI call count
        str = string.Format("LevelEditor OnSceneGUI call count: {0}", _onSceneGuiCallCount);
        GUILayout.Label(str);

        if (Target._editorMode == EditorMode.Editor)
        {
            // TileContainerBehaviour Update call count
            str = string.Format("Brush On sector grid: {0}", IsBrushOnGrid);
            GUILayout.Label(str);

            // TileContainerBehaviour Update call count
            str = string.Format("Ray On plane: {0}", _vec3OnBaseOfCurrentPlane);
            GUILayout.Label(str);

            // Brush position
            str = string.Format("Brush position x: {0}", _brushPosition);
            GUILayout.Label(str);

        }

        str = string.Format("Num tiles: {0}",
            Target.NumberOfTiles);
        GUILayout.Label(str);

        Handles.EndGUI();
    }

    // DRAW EDITOR SCENE OVERLAY
    //
    void DrawEditorSceneOverlay()
    {

        Handles.BeginGUI();

        // Editor notice
        if (Target._editorMode == 0)
        {
            GUILayout.Label("LEVEL: VIEW MODE");
        }
        else
        {
            GUILayout.Label("LEVEL: EDITOR MODE");
        }

        Handles.EndGUI();

    }

    // DRAW PLACEMENT GRID
    // This gets called from On Scene GUI and draws a grid to help the user
    // paint tiles.
    void DrawPlacementGrid(bool gridon)
    {
        // dont bother with a grid if there are zero tiles, the
        // first tile could be placed anywhere
        if (Target.NumberOfTiles == 0)
            return;

        int lowestX = Target.MinXLocationOfTiles;
        int highestX = Target.MaxXLocationOfTiles;
        int lowestZ = Target.MinZLocationOfTiles;
        int highestZ = Target.MaxZLocationOfTiles;

        lowestX -= GRID_BOUNDARY + 1;
        highestX += GRID_BOUNDARY + 1;
        lowestZ -= GRID_BOUNDARY + 1;
        highestZ += GRID_BOUNDARY + 1;

        for (int i = lowestX; i <= highestX; ++i)
        {
            float offset = GetOffsetX(i);

            if (i == 0)
                continue;

            if (!gridon)
            {
                if (i > lowestX && i < highestX)
                    continue;
            }

            Vector3 a1 = new Vector3(
                i * Target.BlockScale.transform.localScale.x + offset,
                BaseOfCurrentLayer,
                lowestZ * Target.BlockScale.transform.localScale.z + GetOffsetZ(lowestZ)
                );

            Vector3 a2 = new Vector3(
                i * Target.BlockScale.transform.localScale.x + offset,
                BaseOfCurrentLayer,
                highestZ * Target.BlockScale.transform.localScale.z + GetOffsetZ(highestZ)
                );

            Handles.DrawLine(a1, a2);
        }

        for (int i = lowestZ; i <= highestZ; ++i)
        {
            float offset = GetOffsetZ(i);
            if (i == 0)
                continue;

            if (!gridon)
            {
                if (i > lowestZ && i < highestZ)
                    continue;
            }

            Vector3 b1 = new Vector3(
                lowestX * Target.BlockScale.transform.localScale.x + GetOffsetX(lowestX),
                BaseOfCurrentLayer,
                i * Target.BlockScale.transform.localScale.z + offset 
                );

            Vector3 b2 = new Vector3(
                highestX * Target.BlockScale.transform.localScale.x + GetOffsetX(highestX),
                BaseOfCurrentLayer,
                i * Target.BlockScale.transform.localScale.z + offset
                );

            Handles.DrawLine(b1, b2);
        }

    }


    #endregion


}
