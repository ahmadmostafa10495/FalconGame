using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public Door DoorAbove, DoorBelow;

    [SerializeField] private Collider2D myCollider;

    public void Deactivate()
    {
        myCollider.enabled = false;
    }
    public void Activate()
    {
        myCollider.enabled = true;
    }
}
