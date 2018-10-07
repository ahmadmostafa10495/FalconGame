using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    public virtual ObstacleHit Hit(Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        ObstacleHit oHit = new ObstacleHit();

        return oHit;
    }
}

public struct ObstacleHit
{
    public Vector3 newPosition;
    public Vector3 newDirection;
    public bool freezeBall;
}
