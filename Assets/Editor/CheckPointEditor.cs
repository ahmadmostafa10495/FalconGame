using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckPoint))]
public class CheckPointEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        CheckPoint myCP = target as CheckPoint;
        PCamera myCam = FindObjectOfType<PCamera>();

        if (GUILayout.Button("Move Camera Here"))
        {
            myCam.transform.position = new Vector3(myCam.transform.position.x, myCP.transform.position.y + myCam.YDifference, myCam.transform.position.z);
        }
    }
}
