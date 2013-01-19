using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode()]
public class LevelEditorCreate
{
	[MenuItem("LevelEditor/Create Level")]
	public static void Create()
	{
		if( !GameObject.Find("Level"))
		{
			CreateLevelFromScratch();
		}

	}
	
	private static void CreateLevelFromScratch()
	{		
		GameObject go = new GameObject("Level") as GameObject;
		Level lvl = go.AddComponent<Level>();

		lvl.UserTileContainer = new GameObject("Level_User") as GameObject;

		lvl.GeneratedTileContainer = new GameObject("Level_Generated") as GameObject;

		lvl.CollisionContainer = new GameObject("Level_Collision") as GameObject;

		lvl.BlockScale = Resources.Load("Prefabs/blockscale") as GameObject;

		lvl.UserBlock = Resources.Load("Prefabs/editorblock") as GameObject;

		lvl.InternalBlock = Resources.Load("Prefabs/internalblock") as GameObject;
	}
}