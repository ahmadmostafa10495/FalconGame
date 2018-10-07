using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OKiller : Obstacle {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public override ObstacleHit Hit(Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        ObstacleHit oHit = new ObstacleHit();
        oHit.freezeBall = true;
        LevelManager.Instance.KillBall();
        return oHit;
    }
}
