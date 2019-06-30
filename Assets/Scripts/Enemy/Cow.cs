﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Enemy
{
    [SerializeField] EnemyAI brain;
    [SerializeField] int health;
    [SerializeField] float speed;
    [SerializeField] Vector2 idleTimes;
    [SerializeField] Vector2 walkTimes;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletForce;
    [SerializeField] float bulletAcc;
    float selectedIdleTime;
    float selectedWalkTime;
    float timeSelected = 0;
    [SerializeField] EnemyState state = EnemyState.Idle;
    float timer = 0;
    float visionRatio;
    Transform player;
    Room myRoom;
    Transform animatorChild;
    Animator anim;
    Rigidbody2D body;
    [SerializeField] GameObject coin;

    //Setting up brain
    bool setUpBrain = false;
    bool playerWatched = false;
    
    //Shot
    [SerializeField] BulletsFunctions shotFuncs;
    [SerializeField] float cadence;
    bool canShootAgain = true;

    private void Start()
    {
        animatorChild = transform.GetChild(0);
        anim = animatorChild.gameObject.GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();

        shotFuncs = GetComponent<BulletsFunctions>();
        brain = GetComponent<EnemyAI>();
        visionRatio = brain.GetVisionRatio();
        selectedIdleTime = Random.Range(idleTimes.x, idleTimes.y);
        selectedWalkTime = Random.Range(walkTimes.x, walkTimes.y);
        player = FindObjectOfType<Player>().transform;
    }
    private void Update()
    {
        anim.SetFloat("velocity", Mathf.Abs(body.velocity.x + body.velocity.y));
        if (body.velocity.x > 0)
        {
            animatorChild.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            animatorChild.transform.localScale = new Vector3(1, 1, 1);
        }
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

        if (state == EnemyState.Move)
        {
            if (!setUpBrain)
            {
                Vector2 randomTarget = (Vector2)transform.position + new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * 10;
                brain.SetTargetPos(randomTarget);
                setUpBrain = true;
            }
            brain.Activate();
        }
        else if (state == EnemyState.Shoot)
        {
            Shoot();
            ChangeState();
        }
        else if (state == EnemyState.Idle)
        {

        }
    }
    void Shoot()
    {
        if (canShootAgain)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            if (Random.Range(0, 2) > 1)
            {
                shotFuncs.TripleShot(bullet, (Vector2)transform.position + dir * 1.05f, dir, bulletForce, -bulletAcc);
            }
            else
            {
                shotFuncs.Arc(bullet, (Vector2)transform.position + dir * 1.05f, dir, bulletForce, -bulletAcc, 5, 0.1f);
            }
            canShootAgain = false;
            StartCoroutine(IECanShootAgain());
        }
    }
    public override void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();
    }
    public override void Die()
    {
        myRoom.NoticeADead();
        int insideCoins = Random.Range(0, 5);
        for (int i = 0; i < insideCoins; i++)
        {
            Vector2 dir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
            GameObject currentCoin = Instantiate(coin, (Vector2)transform.position + dir, Quaternion.identity);
            currentCoin.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
        }
        gameObject.SetActive(false);
    }
    public override bool Alive()
    {
        return health > 0;
    }
    public override void SetRoomHandler(Room m_Room)
    {
        myRoom = m_Room;
    }
    void ChangeState()
    {
        if (playerWatched)
        {
            if (state == EnemyState.Idle)
            {
                state = Random.Range(0, 2) > 0.5f ? EnemyState.Move : EnemyState.Shoot;
                if (state == EnemyState.Move)
                {
                    timeSelected = Random.Range(walkTimes.x, walkTimes.y);
                }
            }
            else if (state == EnemyState.Move || state == EnemyState.Shoot)
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
    IEnumerator IECanShootAgain()
    {
        yield return new WaitForSeconds(cadence);
        canShootAgain = true;
    }
}
