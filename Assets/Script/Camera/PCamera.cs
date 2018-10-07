using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCamera : MonoBehaviour {
    public static PCamera Instance;

    public float YDifference = 6.7f;

    [SerializeField] private Transform target;
    [SerializeField] private float switchingDuration = 1;
    [SerializeField] private AnimationCurve transitionCurve;

    private Vector3 settlingPosition;
    private IEnumerator shakingEnum;

    void Awake ()
    {
        Instance = this;
	}
    public void PlaceCameraOnTarget(Transform t, bool instant = false)
    {
        target = t;

        if (instant)
        {
            transform.position = new Vector3(0, target.position.y + YDifference, transform.position.z);
            settlingPosition = transform.position;
        }
        else
        {
            StopCoroutine("GoToTarget");
            StartCoroutine("GoToTarget");
        }
    }
    IEnumerator GoToTarget()
    {
        float startTime = Time.time;

        Vector3 startPos = transform.position;
        while (Time.time < startTime + switchingDuration)
        {
            float t = (Time.time - startTime) / switchingDuration;
            t = transitionCurve.Evaluate(t);
            transform.position = Vector3.LerpUnclamped(startPos, new Vector3(transform.position.x, target.position.y + YDifference, transform.position.z), t);
            settlingPosition = transform.position;

            yield return null;
        }
        transform.position = new Vector3(transform.position.x, target.position.y + YDifference, transform.position.z);
        settlingPosition = transform.position;
    }

    public void Shake(float power, float duration)
    {
        if (shakingEnum != null)
            StopCoroutine(shakingEnum);

        shakingEnum = Shaking(power, duration);
        StartCoroutine(shakingEnum);
    }
    IEnumerator Shaking(float power, float duration)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            transform.position = settlingPosition + new Vector3(Random.Range(-power, power), Random.Range(-power, power));
            yield return null;
        }
        PlaceCameraOnTarget(LevelManager.Instance.CurrentCP.transform, true);
    }
}
