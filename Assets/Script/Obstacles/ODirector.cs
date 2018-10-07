using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODirector : Obstacle {

    public override ObstacleHit Hit(Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        ObstacleHit oHit = new ObstacleHit();
        oHit.newDirection = hitNormal;
        oHit.newPosition = hitPoint;
        return oHit;
    }
}
