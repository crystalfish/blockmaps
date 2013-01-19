using UnityEngine;
using System.Collections.Generic;


//
// SEGEMENT MANAGER
//
// This behaviours sits at the top level one type of possible generated mesh segements.  
// It's jobs is to get references to all 8 of the segements and then provide a
// simple interface for enabling and disabling them.
//
public class SegmentManager 
	: MonoBehaviour 
{
	public const int NUM_SEGMENTS = 8;

	// Manually set from the inspector and save in a prefab.
	public GameObject[] _segmentGOs = new GameObject[NUM_SEGMENTS];

	// A list of flags that define which segements are visible
	[UnityEngine.SerializeField]
	bool[] _segmentEnabled = new bool[NUM_SEGMENTS] { true, true, true, true, true, true, true, true };

	// IS VISIBLE
	// Find out if a given segment is visible
	public bool IsVisible(SegmentID zId) { return _segmentEnabled[(int)zId]; }

	// SET VISIBLE
	// Set the visiblity of a given segment
	public void SetVisible(SegmentID zId, bool zValue) { _segmentEnabled[(int)zId] = zValue; UpdateSubMeshes(); }
	
	// UPDATE SUB MESHES
	// This function passes through the array of flags and sets the visiblity of 
	// each segment to match.
	void UpdateSubMeshes()
	{
		for (int i = 0; i < NUM_SEGMENTS; ++i)
		{
			_segmentGOs[i].SetActiveRecursively(_segmentEnabled[i]);
		}
	}
	
	// DISABLE ALL
	// Hide all segments.
	public void DisableAll()
	{
		for (int i = 0; i < NUM_SEGMENTS; ++i)
			_segmentEnabled[i] = false;

		UpdateSubMeshes();
	}

	// ENABLE ALL
	// Show all segments.
	public void EnableAll()
	{
		for (int i = 0; i < NUM_SEGMENTS; ++i)
			_segmentEnabled[i] = true;

		UpdateSubMeshes();
	}



}
