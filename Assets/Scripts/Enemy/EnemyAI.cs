using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] bool canMove;
    [SerializeField] float visionRatio;
    [SerializeField] float closerangeDistance;
    [SerializeField] float movementSpeed;
    [SerializeField] float nextWayPointDistance;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstaclelayer;

    int currentWaypoint = 0;
    bool reachedEnd = false;
    bool isPlayerOnVision = false;
    bool isWallInFront = false;

    Path path;
    Seeker seeker;
    Rigidbody2D rbd;
    Transform target;
    Vector2 targetPos;

    bool activated = false;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rbd = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    public void SetTargetPos(Vector2 pos)
    {
        targetPos = pos;
    }
    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public float GetVisionRatio() { return visionRatio; }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rbd.position, targetPos, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    
    void Update()
    {
        if (target != null)
            targetPos = target.position;
        if (activated)
            ScanForPlayer();
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEnd = true;
            return;
        }
        else
        {
            reachedEnd = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rbd.position).normalized;

        if(isWallInFront)
        {
            direction = -(targetPos - (Vector2)transform.position).normalized;
        }

        Vector2 force = direction * movementSpeed * Time.deltaTime;
        float distance = Vector2.Distance(rbd.position, path.vectorPath[currentWaypoint]);

        if (!isWallInFront && activated)
            rbd.AddForce(force);

        if(distance < nextWayPointDistance)
            currentWaypoint++;
    }

    public bool ScanRadius()
    {
        bool playerDetection = false;
        RaycastHit2D[] circleRay = Physics2D.CircleCastAll(transform.position, visionRatio, Vector2.zero, 10, playerLayer);

        for (int i = 0; i < circleRay.Length; i++)
        {
            if (circleRay[i].collider != null)
            {
                playerDetection = true;
                break;
            }
        }
        return playerDetection;
    }
    void ScanForPlayer()
    {
        bool playerDetection = ScanRadius();
        bool isObstacleBlocking = false;
        isPlayerOnVision = playerDetection;

        Vector2 ray = (targetPos - (Vector2)transform.position).normalized;
        RaycastHit2D visionRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), ray, 1f, obstaclelayer);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), ray, Color.blue);

        if (visionRay.collider != null)
        {
            isObstacleBlocking = true;
        }

        isWallInFront = isObstacleBlocking;
    }

    public void Activate()
    {
        activated = true;
    }
    public void Deactivate()
    {
        activated = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRatio);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closerangeDistance);
    }
}
