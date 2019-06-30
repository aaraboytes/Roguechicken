using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milk : Enemy
{
    [SerializeField] EnemyAI brain;
    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] Vector2 idleTimes;
    [SerializeField] Vector2 walkTimes;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletForce;
    float selectedIdleTime;
    float selectedWalkTime;
    float timeSelected = 0;
    [SerializeField]EnemyState state = EnemyState.Idle;
    float timer = 0;
    float visionRatio;
    Transform player;

    //Setting up brain
    bool setUpBrain = false;
    bool playerWatched = false;

    //Shot
    [SerializeField]BulletsFunctions shotFuncs;

    private void Start()
    {
        shotFuncs = GetComponent<BulletsFunctions>();
        brain = GetComponent<EnemyAI>();
        visionRatio = brain.GetVisionRatio();
        selectedIdleTime = Random.Range(idleTimes.x, idleTimes.y);
        selectedWalkTime = Random.Range(walkTimes.x, walkTimes.y);
        player = FindObjectOfType<Player>().transform;
    }
    private void Update()
    {
        if (!playerWatched)
        {
            if (brain.ScanRadius())
            {
                playerWatched = true;
                brain.SetTarget(player);
                timer = 0;
            }
        }

        timer += Time.deltaTime;
        if (timer > timeSelected)
        {
            ChangeState();
            timer = 0;
        }

        if(state == EnemyState.Move) {
            if (!setUpBrain)
            {
                Vector2 randomTarget = (Vector2)transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * 10;
                brain.SetTargetPos(randomTarget);
                setUpBrain = true;
            }
            brain.Activate();
        }
        else if(state == EnemyState.Shoot)
        {
            Shoot();
            ChangeState();
        }
        else if(state == EnemyState.Idle)
        {
            
        }
    }
    void Shoot()
    {
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        shotFuncs.SimpleShot(bullet, (Vector2)transform.position + dir*1.05f, dir, bulletForce);
    }
    public override void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
    public override void Die()
    {
        Destroy(gameObject);
    }
    void ChangeState()
    {
        if (playerWatched)
        {
            if(state == EnemyState.Idle)
            {
                state = Random.Range(0, 2) > 0.5f ? EnemyState.Move : EnemyState.Shoot;
                if(state == EnemyState.Move)
                {
                    timeSelected = Random.Range(walkTimes.x, walkTimes.y);
                }
            }else if(state == EnemyState.Move || state == EnemyState.Shoot)
            {
                state = EnemyState.Idle;
                brain.Deactivate();
                timeSelected = Random.Range(idleTimes.x, idleTimes.y);
            }
        }
        else
        {
            if (state == EnemyState.Move)
            {
                state = EnemyState.Idle;
                timeSelected = Random.Range(idleTimes.x, idleTimes.y);
                setUpBrain = false;
                brain.Deactivate();
            }
            else
            {
                state = EnemyState.Move;
                timeSelected = Random.Range(walkTimes.x, walkTimes.y);
            }
        }
    }
}
