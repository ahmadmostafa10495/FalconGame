using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public List<CheckPoint> AllCPs = new List<CheckPoint>();

    [HideInInspector] public float LevelHeight = 16.85f;
}
