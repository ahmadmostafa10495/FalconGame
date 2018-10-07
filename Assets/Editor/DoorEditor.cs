using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        Door myDoor = target as Door;

        if (GUILayout.Button("Open"))
        {
            myDoor.Open(true);
        }
        if (GUILayout.Button("Close"))
        {
            myDoor.Close(true);
        }
    }
}
