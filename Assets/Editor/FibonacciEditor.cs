using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Fibonacci))]
public class FibonacciEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Fibonacci generatedFibonacci = target as Fibonacci;

        DrawDefaultInspector();

        if (GUILayout.Button("Apply"))
        {
            
            generatedFibonacci.GenerateFibonacci();
        }

        
    }
}
