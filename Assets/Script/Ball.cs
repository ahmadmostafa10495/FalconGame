using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField] private float speed = 10;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private LayerMask lm;
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float maxPathLength = 10;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve moveToCP;

    [Header("Effects")]
    [SerializeField] private GameObject collisionPS;
    [SerializeField] private GameObject deathPS;

    private bool controllable = true;
    private bool aiming;
    private bool traveling;
    private Collider2D ignoredCollider;
    private Vector3 direction;
    private Vector3 lastPos;
    private Vector3 lastHitPos;
    private int stuckFrames;

    void Update ()
    {
        if (aiming)
            Aiming();

        if (traveling)
            Traveling();
	}
    void OnMouseDown()
    {
        if (!controllable) return;

        aiming = true;
        pathLine.enabled = true;
    }

    void Aiming()
    {
        //Get mouse point in world.
        Vector3 mousePos = Input.mousePosition;
        Vector3 curPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 15));
        Vector3 dir = (transform.position - curPos).normalized;

        //End shooting if dragging ended.
        if (Input.GetMouseButtonUp(0))
        {
            aiming = false;
            pathLine.enabled = false;
            Shoot(dir);
        }

        PathDrawing(dir);
    }
    void PathDrawing(Vector3 initialDirection)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        Collider2D lastCollider = null;

        Vector3 curDirection = initialDirection;
        Vector3 curOriginPos = transform.position;
        float remainingDistance = maxPathLength;

        int iterations = 0;
        while (remainingDistance > 0)
        {
            pathPoints.Add(curOriginPos);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(curOriginPos, radius, curDirection, remainingDistance, lm);

            RaycastHit2D hit = new RaycastHit2D();
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider != lastCollider)
                {
                    hit = hits[j];
                    break;
                }
            }

            Collider2D hitCollider = hit.collider;
            //If this ray hit an object then save the new info to calculate the next line segment.
            if (hitCollider != null && hitCollider != lastCollider)
            {
                ObstacleHit oHit = GetLineObstacleHit(hitCollider, hit.centroid, curDirection, hit.normal);

                if (oHit.freezeBall == true)
                {
                    curOriginPos = hit.point;
                    pathPoints.Add(curOriginPos);
                    break;
                }

                curOriginPos = oHit.newPosition;
                curDirection = oHit.newDirection;

                lastCollider = hit.collider;

                remainingDistance -= hit.distance;
            }
            //If this ray didn't hit an object, then end the line here.
            else
            {
                //Add the last point (in air, since they ray didn't hit anything).
                pathPoints.Add(curOriginPos + curDirection * remainingDistance);
                remainingDistance = 0;
                break;
            }

            iterations++;
            if (iterations > 20)
            {
                Debug.Log("Too many iterations");
                break;
            }
        }

        //Draw the path
        int pointsCount = pathPoints.Count;
        pathLine.positionCount = pointsCount;
        pathLine.SetPositions(pathPoints.ToArray());
    }

    void Shoot(Vector3 dir)
    {
        if (dir.magnitude < 0.75f) return;

        traveling = true;
        controllable = false;
        direction = dir;
        LevelManager.Instance.ActivateCP(LevelManager.Instance.CurrentCP);
    }

    void Traveling()
    {
        lastPos = transform.position;

        transform.Translate(direction * speed * Time.deltaTime);

        //Debug.DrawLine(lastPos, transform.position);

        RaycastHit2D[] hits = Physics2D.CircleCastAll(lastPos, radius, direction, Vector3.Distance(lastPos, transform.position), lm);

        RaycastHit2D hit = new RaycastHit2D();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != ignoredCollider)
            {
                hit = hits[i];
                break;
            }
        }

        Collider2D hitCollider = hit.collider;
        if (hitCollider != null && hitCollider != ignoredCollider)
        {
            CheckPoint cp = hitCollider.GetComponent<CheckPoint>();

            if (cp != null)
            {
                PlaceOnCP(cp);
              
                //A check point was hit.
                LevelManager.Instance.CheckPointReached(cp);

            }
            else
            {
                ObstacleHit oHit = GetBallObstacleHit(hitCollider, hit.centroid, direction, hit.normal);

                if (!oHit.freezeBall)
                {
                    bool stuck = StuckCheck(oHit.newPosition);
                    lastHitPos = oHit.newPosition;

                    if (!stuck)
                    {
                        transform.position = oHit.newPosition;
                        Shoot(oHit.newDirection);
                    }

                    ignoredCollider = hitCollider;

                    PCamera.Instance.Shake(0.025f, 0.05f);
                    GameObject cPS = Instantiate(collisionPS);
                    cPS.transform.position = hit.point;
                    cPS.transform.forward = hit.normal;
                }
                else
                {
                    traveling = false;
                }
            }
        }
    }

    bool StuckCheck(Vector3 curHitPos)
    {
        if (Vector3.Distance(lastHitPos, curHitPos) < 0.1f)
        {
            stuckFrames++;

            if (stuckFrames > 10)
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(lastPos, radius, direction, 0, lm);

                Vector3 outputNormal = new Vector3();

                for (int i = 0; i < hits.Length; i++)
                {
                    outputNormal += (Vector3)hits[i].normal;
                }

                outputNormal.Normalize();

                transform.position += outputNormal * 0.2f;
                Shoot(outputNormal);

                return true;
            }
            else
                return false;
        }
        else
        {
            stuckFrames = 0;
            return false;
        }
    }

    /// <summary>
    /// Get the hit data when the ball collides with any object.
    /// </summary>
    /// <param name="collidedObstacle"></param>
    /// <param name="hitPoint"></param>
    /// <param name="direction"></param>
    /// <param name="hitNormal"></param>
    /// <returns></returns>
    ObstacleHit GetBallObstacleHit(Collider2D collidedObstacle, Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        Obstacle o = collidedObstacle.GetComponent<Obstacle>();

        if (o)
            return o.Hit(hitPoint, direction, hitNormal);
        else
            return DefaultObstacleHit(hitPoint, direction, hitNormal);
    }
    /// <summary>
    /// Get the hit data when the line collides with any objects.
    /// </summary>
    /// <param name="collidedObstacle"></param>
    /// <param name="hitPoint"></param>
    /// <param name="direction"></param>
    /// <param name="hitNormal"></param>
    /// <returns></returns>
    ObstacleHit GetLineObstacleHit(Collider2D collidedObstacle, Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        Obstacle o = collidedObstacle.GetComponent<Obstacle>();

        if (o)
        {
            if (o.GetType() == typeof(OKiller) || o.GetType()  == typeof(OPortal))
            {
                ObstacleHit oHit = new ObstacleHit();
                oHit.freezeBall = true;
                return oHit;
            }
            return o.Hit(hitPoint, direction, hitNormal);
        }
        else
            return DefaultObstacleHit(hitPoint, direction, hitNormal);
    }

    /// <summary>
    /// The default function that will be called if the object the ball collided with doesn't have an obstacle script.
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    ObstacleHit DefaultObstacleHit(Vector3 hitPoint, Vector3 direction, Vector3 hitNormal)
    {
        ObstacleHit oHit = new ObstacleHit();
        oHit.newDirection = Vector3.Reflect(direction, hitNormal);
        oHit.newPosition = hitPoint;
        return oHit;
    }

    public void PlaceOnCP(CheckPoint cp, bool instant = false)
    {
        StopCoroutine("PlacingOnCP");

        traveling = false;
        controllable = true;
        ignoredCollider = cp.GetComponent<Collider2D>();

        if (instant)
            transform.position = cp.transform.position;
        else
            StartCoroutine("PlacingOnCP",cp.transform.position);

    }
    IEnumerator PlacingOnCP(Vector3 cpPos)
    {
        float startTime = Time.time;
        float duration = 0.5f;
        Vector3 startPos = transform.position;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            t = moveToCP.Evaluate(t);

            transform.position = Vector3.LerpUnclamped(startPos, cpPos, t);

            yield return null;
        }
        transform.position = cpPos;
    }

    public void Die()
    {
        Instantiate(deathPS, transform.position, Quaternion.identity);
    }
}
