using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{

    [SerializeField] float visionRatio;
    [SerializeField] float closerangeDistance;
    [SerializeField] float movementSpeed;
    [SerializeField] float nextWayPointDistance;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstaclelayer;
    [SerializeField] Transform target;

    int currentWaypoint = 0;
    bool reachedEnd = false;
    bool isPlayerOnVision = false;
    bool isWallInFront = false;

    Path path;
    Seeker seeker;
    Rigidbody2D rbd;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rbd = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone())
            seeker.StartPath(rbd.position, target.position, OnPathComplete);
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
            direction = -(target.position - transform.position).normalized;
        }

        Vector2 force = direction * movementSpeed * Time.deltaTime;
        float distance = Vector2.Distance(rbd.position, path.vectorPath[currentWaypoint]);

        if (isPlayerOnVision && distance > closerangeDistance)
            rbd.AddForce(force);

        if(distance < nextWayPointDistance)
            currentWaypoint++;
    }

    void ScanForPlayer()
    {
        bool playerDetection = false;
        bool isObstacleBlocking = false;

        RaycastHit2D[] circleRay = Physics2D.CircleCastAll(transform.position, visionRatio, Vector2.zero, 10, playerLayer);

        for(int i = 0; i < circleRay.Length; i++)
        {
            if (circleRay[i].collider != null)
            {
                playerDetection = true;
                break;
            }
        }

        isPlayerOnVision = playerDetection;

        Vector2 ray = (target.position - transform.position).normalized;
        RaycastHit2D visionRay = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), ray, 1f, obstaclelayer);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y), ray, Color.blue);

        if (visionRay.collider != null)
        {
            isObstacleBlocking = true;
        }

        isWallInFront = isObstacleBlocking;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRatio);
    }
}
