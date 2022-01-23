using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DestructionManager))]
public class DestructionManagerEditor : Editor
{
    void OnSceneGUI()
    {
        // Don't forget to turn on Gizmos
        DestructionManager DM = (DestructionManager)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(DM.SpawnPoint.position, Vector3.up, Vector3.forward, 360, DM.SpawnPointRadius);
        Handles.DrawWireArc(DM.DestPoint.position, Vector3.up, Vector3.forward, 360, DM.DestMaxRadius);
        Handles.DrawWireArc(DM.DestPoint.position, Vector3.up, Vector3.forward, 360, DM.DestMinRadius);
    }
}
