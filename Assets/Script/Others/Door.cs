using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    [Header("Sides")]
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;

    [Header("Points")]
    [SerializeField] private Transform leftClosedPoint;
    [SerializeField] private Transform leftOpenedPoint;
    [SerializeField] private Transform rightClosedPoint;
    [SerializeField] private Transform rightOpenedPoint;

    [Header("Settings")]
    [SerializeField] private float switchingDuration = 1;
    [SerializeField] private AnimationCurve switchingCurve;

    private void Update()
    {
        if (Input.GetKeyDown("o"))
            Open();
        if (Input.GetKeyDown("c"))
            Close();
    }

    public void Open(bool instant = false)
    {
        if (!instant)
        {
            StopCoroutine("Switching");
            StartCoroutine("Switching", true);
        }
        else
        {
            left.position = leftOpenedPoint.position;
            right.position = rightOpenedPoint.position;
        }
    }
    public void Close(bool instant = false)
    {
        if (!instant)
        {
            StopCoroutine("Switching");
            StartCoroutine("Switching", false);    
        }
        else
        {
            left.position = leftClosedPoint.position;
            right.position = rightClosedPoint.position;
        }
    }

    IEnumerator Switching(bool open)
    {
        float startTime = Time.time;

        Vector3 lStart = left.position;
        Vector3 rStart = right.position;
        Vector3 lEnd = open ? leftOpenedPoint.position : leftClosedPoint.position;
        Vector3 rEnd = open ? rightOpenedPoint.position : rightClosedPoint.position;

        while (Time.time < startTime + switchingDuration)
        {
            float t = (Time.time - startTime) / switchingDuration;

            t = switchingCurve.Evaluate(t);
            
            left.position = Vector3.Lerp(lStart, lEnd, t);
            right.position = Vector3.Lerp(rStart, rEnd, t);

            yield return null;
        }
        left.position = lEnd;
        right.position = rEnd;
    }
}
