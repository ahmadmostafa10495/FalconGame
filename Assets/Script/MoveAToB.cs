using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAToB : MonoBehaviour {
    [SerializeField]
    private Transform pointA, pointB;
    [SerializeField]
    private float speed;
    private bool AToB = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (AToB)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed);
            if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
                AToB = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pointA.position, speed);
            if (Vector3.Distance(transform.position, pointA.position) < 0.1f)
                AToB = true;
        }
    }
}
