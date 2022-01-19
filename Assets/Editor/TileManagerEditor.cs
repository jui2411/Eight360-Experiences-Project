using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileManager generatedTileManager = target as TileManager;
        DrawDefaultInspector();
        if(GUILayout.Button("Apply"))
        {
            generatedTileManager.TestGenerateTiles();
        }

        if (GUILayout.Button("Create Pool"))
        {
            generatedTileManager.CreatePool();
        }
    }
}
