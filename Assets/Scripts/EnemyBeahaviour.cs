using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeahaviour : MonoBehaviour
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float movementSpeed;
    [SerializeField] float visionRatio;

    bool isMoving = false;



    void Start()
    {
        
    }

    void Update()
    {
        ScanForPlayer();
    }

    void ScanForPlayer()
    {
        bool playerDetection = false;
        RaycastHit2D circleRay = Physics2D.CircleCast(transform.position, visionRatio, Vector2.right, 0, playerLayer);

        if (circleRay.collider != null)
        {
            if (circleRay.collider.gameObject.GetComponent<Player>())
            {
                playerDetection = true;
            }
        }

        isMoving = playerDetection;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRatio);
    }
}

public enum EnemyType
{
    Leche,
    RoboCow,
    Udder,
    CowBot,
    JhonnyBull
}
