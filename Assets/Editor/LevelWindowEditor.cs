using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelWindowEditor : Editor {
    
    [MenuItem("Tools/Create New Level")]
    public static void CreateNewLevel()
    {
        GameObject root = new GameObject("Level", typeof(Level));
        GameObject wallsHolder = new GameObject("Walls");
        wallsHolder.transform.parent = root.transform;

        CreateWall(wallsHolder.transform, "OuterWall_Down", new Vector3(-5, 0, 0), new Vector3(10, 0.55f, 1), Vector3.right);
        CreateWall(wallsHolder.transform, "OuterWall_Left", new Vector3(-5, 0, 0), new Vector3(17.8f, 1, 1), Vector3.up);
        CreateWall(wallsHolder.transform, "OuterWall_Right", new Vector3(5, 0, 0), new Vector3(17.8f, 1, 1), Vector3.up);
        CreateWall(wallsHolder.transform, "OuterWall_Up", new Vector3(-5, 17.8f, 0), new Vector3(10, 0.55f, 1), Vector3.right);

        Selection.activeGameObject = root;
    }

    static void CreateWall(Transform root, string wallName, Vector3 pos, Vector3 scale, Vector3 rightDirection)
    {
        GameObject wallBlock = EditorGUIUtility.Load("LevelEditor/Block.prefab") as GameObject;
        GameObject outerWall_Left = Instantiate(wallBlock, root.transform) as GameObject;
        outerWall_Left.name = wallName;
        outerWall_Left.transform.localPosition = pos;
        outerWall_Left.transform.localScale = scale;
        outerWall_Left.transform.right = rightDirection;
    }
}
