using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletSide { player, enemy};
public enum BulletType { normal,acc};
public class Bullet : MonoBehaviour
{
    [SerializeField] BulletSide side;
    [SerializeField] int damage;
    [SerializeField] GameObject particle;
    float speed;
    float acc;
    Rigidbody2D body;
    Vector2 dir;
    BulletType type = BulletType.normal;
    bool alive = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        GetComponent<Collider2D>().enabled = false;
    }
    public void Shoot(Vector2 _dir,BulletType _type,float _speed,float _acc = 0)
    {
        type = _type;
        speed = _speed;
        acc = _acc;
        dir = _dir;
        body.velocity = dir * speed;
        if (type.Equals(BulletType.acc))
        {
            StartCoroutine(IEAccelerate());
        }
        GetComponent<Collider2D>().enabled = true;
    }
    IEnumerator IEAccelerate()
    {
        while (alive)
        {
            speed -= acc * Time.deltaTime;
            body.velocity = dir * speed;
            if (Mathf.Abs(body.velocity.x + body.velocity.y) < 0.5f)
            {
                gameObject.SetActive(false);
            }
            yield return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (side.Equals(BulletSide.player) && !other.CompareTag("Player"))
        {
            //Friendly bullet
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>().Damage(damage);
            }
            gameObject.SetActive(false);
        }
        else if(side.Equals(BulletSide.enemy) && !other.CompareTag("Enemy"))
        {
            //Enemy bullet
            if (other.CompareTag("Player"))
            {
                PlayerStats ps = other.GetComponent<PlayerStats>();
                if (!ps.Invincible())
                {
                    other.GetComponent<PlayerStats>().Damage(damage,transform.position);
                }
            }
            //Pool.Instance.Recycle(particle, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
