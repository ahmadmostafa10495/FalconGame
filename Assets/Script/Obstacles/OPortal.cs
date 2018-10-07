using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OPortal : Obstacle {

    [SerializeField] private Transform pairedPortal;


    public override ObstacleHit Hit(Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        ObstacleHit oHit = new ObstacleHit();
        oHit.newDirection = direction;
        Vector3 localHit = transform.InverseTransformPoint(hitPoint);
        oHit.newPosition = pairedPortal.TransformPoint(localHit) + oHit.newDirection * 0.1f;
        return oHit;
    }
}
