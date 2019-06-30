using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] int maxHealth;
    [SerializeField] bool isParent = true;
    [SerializeField] Transform[] childInstaPos;

    int health;
    EnemyAI enemyAI;
    Rigidbody2D rbd;
    Room myRoom;
    public override void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        if(isParent)
        {
            for(int i = 0; i < childInstaPos.Length; i++)
            {
                GameObject child = Instantiate(gameObject, childInstaPos[i].position, Quaternion.identity);
                child.GetComponent<Slime>().SetIsParent(false);
                child.GetComponent<Slime>().SetRoomHandler(myRoom);
                child.transform.localScale = new Vector2(0.75f, 0.75f);
            }
        }
        else
        {
            myRoom.NoticeADead();
        }
        gameObject.SetActive(false);
        
    }

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        rbd = GetComponent<Rigidbody2D>();

        health = maxHealth;
        //StartCoroutine(MoveSlimish());
    }
    public override void SetRoomHandler(Room m_Room)
    {
        myRoom = m_Room;
    }
    public override bool Alive()
    {
        return health > 0;
    }

    /*IEnumerator MoveSlimish()
    {
        /*enemyAI.SetCanMove(true);
        yield return new WaitForSeconds(0.4f);

        enemyAI.SetCanMove(false);
        rbd.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveSlimish());
    }*/

    public void SetIsParent(bool newIsParent)
    {
        isParent = newIsParent;
    }
}
