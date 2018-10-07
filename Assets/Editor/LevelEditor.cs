using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {

    private enum Elements { Blocker, KillerZone, CheckPoint, Door, Director, Portal }

    private float levelHeight {
        get {
            Level l = target as Level;
            return l.LevelHeight;
        }
        set
        {
            Level l = target as Level;
            l.LevelHeight = value;
        }
    }
    private GameObject wallBlock;

    private bool creatingElement;
    private Elements elementBeingCreated;
    private int elementBeingCreatedState;
    private GameObject elementBeingCreatedReference;
    private Vector3 elementBeingCreatedStartPosition;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        Level level = target as Level;

        Undo.RecordObject(level, "Level Change");
        levelHeight = EditorGUILayout.FloatField("Level Height", levelHeight);
        EditorGUILayout.Space();

        UpdateSelectedLevelHeight();

        #region Elements Creation
        EditorGUILayout.LabelField("Create Elements:", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        #region Blocker
        if (creatingElement && elementBeingCreated == Elements.Blocker)
            GUI.backgroundColor = Color.red;

        if (GUILayout.Button("Blocker", GUILayout.Width(60), GUILayout.Height(30)))
        {
            if (!(creatingElement && elementBeingCreated == Elements.Blocker))
            {
                creatingElement = true;
                elementBeingCreated = Elements.Blocker;
                elementBeingCreatedState = 0;
            }
            else
            {
                creatingElement = false;
            }
        }
        GUI.backgroundColor = Color.white;
        #endregion

        #region Killer
        if (creatingElement && elementBeingCreated == Elements.KillerZone)
            GUI.backgroundColor = Color.red;

        if (GUILayout.Button("Killer", GUILayout.Width(60), GUILayout.Height(30)))
        {
            if (!(creatingElement && elementBeingCreated == Elements.KillerZone))
            {
                creatingElement = true;
                elementBeingCreated = Elements.KillerZone;
                elementBeingCreatedState = 0;
            }
            else
            {
                creatingElement = false;
            }
        }
        GUI.backgroundColor = Color.white;
        #endregion

        #region Check Point
        if (creatingElement && elementBeingCreated == Elements.CheckPoint)
            GUI.backgroundColor = Color.red;

        if (GUILayout.Button("CP", GUILayout.Width(60), GUILayout.Height(30)))
        {
            if (!(creatingElement && elementBeingCreated == Elements.CheckPoint))
            {
                creatingElement = true;
                elementBeingCreated = Elements.CheckPoint;
                elementBeingCreatedState = 0;
            }
            else
            {
                creatingElement = false;
            }
        }
        GUI.backgroundColor = Color.white;
        #endregion

        GUILayout.EndHorizontal();

        #endregion

        serializedObject.ApplyModifiedProperties();
    }
    private void OnSceneGUI()
    {
        if (creatingElement)
        {
            Level level = target as Level;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            //Cancel controls.
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                creatingElement = false;
                Repaint();
            }

            switch (elementBeingCreated)
            {
                case Elements.Blocker:
                    DrawStartToEndElement(level, "Block.prefab", "Blockers", "Block");
                    break;
                case Elements.KillerZone:
                    DrawStartToEndElement(level, "KillZone.prefab", "KillZones", "KillZone");
                    break; 
                case Elements.CheckPoint:
                    if (elementBeingCreatedState == 0)
                    {
                        //Waiting user to pick the blocker's start point...
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            Transform cpHolder = level.transform.Find("CheckPoints");
                            if (!cpHolder)
                            {
                                cpHolder = (new GameObject("CheckPoints")).transform;
                                cpHolder.parent = level.transform;
                            }

                            GameObject cp = EditorGUIUtility.Load("LevelEditor/CheckPoint.prefab") as GameObject;
                            elementBeingCreatedReference = Instantiate(cp) as GameObject;
                            elementBeingCreatedReference.name = "CheckPoint";
                            elementBeingCreatedReference.transform.parent = cpHolder;
                            elementBeingCreatedReference.transform.position = GetMouseWorldPos();

                            CheckPoint cpBehaviour = elementBeingCreatedReference.GetComponent<CheckPoint>();
                            level.AllCPs.Add(cpBehaviour);

                            //Finish creation process once the user clicks.
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                                elementBeingCreatedState = 0;
                        }
                    }
                    break;
            }
        }
    }

    void UpdateSelectedLevelHeight()
    {
        Transform selectedLevel = ((Level)target).transform;

        Transform wall_Left = selectedLevel.Find("Walls").Find("OuterWall_Left");
        Transform wall_Right = selectedLevel.Find("Walls").Find("OuterWall_Right");
        Transform wall_Up = selectedLevel.Find("Walls").Find("OuterWall_Up");

        wall_Left.localPosition = new Vector3(-5, 0, 0);
        wall_Left.localScale = new Vector3(levelHeight, 1, 1);
        wall_Right.transform.localPosition = new Vector3(5, 0, 0);
        wall_Right.localScale = new Vector3(levelHeight, 1, 1);
        wall_Up.transform.localPosition = new Vector3(-5, levelHeight, 0);
    }
    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Event.current.mousePosition;
        return Camera.current.ScreenToWorldPoint(new Vector3(mousePos.x, Screen.height - mousePos.y - 37, -Camera.current.transform.position.z));
    }

    void DrawStartToEndElement(Level level, string ePrefabName, string eHolder, string eName)
    {
        if (elementBeingCreatedState == 0)
        {
            //Waiting user to pick the blocker's start point...
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Transform blockersHolder = level.transform.Find(eHolder);
                if (!blockersHolder)
                {
                    blockersHolder = (new GameObject(eHolder)).transform;
                    blockersHolder.parent = level.transform;
                }

                GameObject block = EditorGUIUtility.Load("LevelEditor/" + ePrefabName) as GameObject;
                elementBeingCreatedReference = Instantiate(block) as GameObject;
                elementBeingCreatedReference.name = eName;
                elementBeingCreatedReference.transform.parent = blockersHolder;

                elementBeingCreatedStartPosition = GetMouseWorldPos();
                elementBeingCreatedReference.transform.position = elementBeingCreatedStartPosition;

                elementBeingCreatedState++;
            }
        }
        else if (elementBeingCreatedState == 1)
        {
            //Waiting user to pick the blocker's end point...
            Vector3 endPos = GetMouseWorldPos();
            float length = Vector3.Distance(endPos, elementBeingCreatedStartPosition);

            elementBeingCreatedReference.transform.localScale = new Vector3(length, elementBeingCreatedReference.transform.localScale.y, 1);
            elementBeingCreatedReference.transform.right = endPos - elementBeingCreatedStartPosition;

            elementBeingCreatedReference.transform.eulerAngles = new Vector3(0, 0, elementBeingCreatedReference.transform.eulerAngles.z - (elementBeingCreatedReference.transform.eulerAngles.z % 15));

            //Finish creation process once the user clicks.
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                elementBeingCreatedState = 0;
                elementBeingCreatedReference.transform.eulerAngles = new Vector3(0, 0, elementBeingCreatedReference.transform.eulerAngles.z - (elementBeingCreatedReference.transform.eulerAngles.z % 15));
            }
        }
    }

    //void Hash()
    //{
    //    if (elementBeingCreatedState == 0)
    //    {
    //        //Waiting user to pick the blocker's start point...
    //        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
    //        {
    //            Transform blockersHolder = level.transform.Find("Blockers");
    //            if (!blockersHolder)
    //            {
    //                blockersHolder = (new GameObject("Blockers")).transform;
    //                blockersHolder.parent = level.transform;
    //            }

    //            GameObject block = EditorGUIUtility.Load("LevelEditor/Block.prefab") as GameObject;
    //            elementBeingCreatedReference = Instantiate(block) as GameObject;
    //            elementBeingCreatedReference.name = "Blocker";
    //            elementBeingCreatedReference.transform.parent = blockersHolder;

    //            elementBeingCreatedStartPosition = GetMouseWorldPos();
    //            elementBeingCreatedReference.transform.position = elementBeingCreatedStartPosition;

    //            elementBeingCreatedState++;
    //        }
    //    }
    //    else if (elementBeingCreatedState == 1)
    //    {
    //        //Waiting user to pick the blocker's end point...
    //        Vector3 endPos = GetMouseWorldPos();
    //        float length = Vector3.Distance(endPos, elementBeingCreatedStartPosition);

    //        elementBeingCreatedReference.transform.localScale = new Vector3(length, 0.5f, 1);
    //        elementBeingCreatedReference.transform.right = endPos - elementBeingCreatedStartPosition;

    //        elementBeingCreatedReference.transform.eulerAngles = new Vector3(0, 0, elementBeingCreatedReference.transform.eulerAngles.z - (elementBeingCreatedReference.transform.eulerAngles.z % 15));

    //        //Finish creation process once the user clicks.
    //        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
    //        {
    //            elementBeingCreatedState = 0;
    //            elementBeingCreatedReference.transform.eulerAngles = new Vector3(0, 0, elementBeingCreatedReference.transform.eulerAngles.z - (elementBeingCreatedReference.transform.eulerAngles.z % 15));
    //        }
    //    }
    //}
}

