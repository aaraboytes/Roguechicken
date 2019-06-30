using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsFunctions : MonoBehaviour
{
    public void SimpleShot(GameObject _bullet, Vector2 pos, Vector2 dir,float speed)
    {
        GameObject bullet = Pool.Instance.Recycle(_bullet, pos, Quaternion.identity);
        bullet.GetComponent<Bullet>().Shoot(dir, BulletType.normal, speed);
    }
    public void TripleShot(GameObject bullet, Vector2 pos,Vector2 dir,float speed,float acc)
    {
        int amount = 3;
        float difference = 0.1f;
        Arc(bullet, pos, dir, speed, acc, amount, difference);
    }
    public void Arc(GameObject bullet,Vector2 pos,Vector2 dir,float speed,float acc,int amount,float difference)
    {
        float angle = Mathf.Atan2(dir.y, dir.x);
        angle -= amount * difference;
        GameObject[] bullets = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            Vector2 nDir = new Vector2(Mathf.Cos(angle + difference * i), Mathf.Sin(angle + difference * i)).normalized;
            bullets[i] = Pool.Instance.Recycle(bullet, pos + nDir * 1.05f, Quaternion.identity);
            bullets[i].GetComponent<Bullet>().Shoot(nDir, BulletType.acc, speed, acc);
        }
    }
    public void Spike(GameObject bullet, Vector2 pos,Vector2 dir,float speed,int amount)
    {
        Vector2 normalDir = new Vector2(dir.y, -dir.x);
        GameObject[] bullets = new GameObject[amount];
        float separation = 0.5f;
        for (int i = 0; i < amount/2; i+=2)
        {
            Vector2 currentPosR = pos + normalDir * i * separation;
            Vector2 currentPosL = pos - normalDir * i * separation;
            bullets[i] = Pool.Instance.Recycle(bullet, currentPosR, Quaternion.identity);
            bullets[i+1] = Pool.Instance.Recycle(bullet, currentPosL, Quaternion.identity);
        }
        StartCoroutine(IEThrowBullets(bullets,0.07f,dir,speed));
    }
    IEnumerator IEThrowBullets(GameObject[] bullets,float cadence,Vector2 dir,float speed)
    {
        for(int i = 0; i < bullets.Length; i += 2)
        {
            bullets[i].GetComponent<Bullet>().Shoot(dir, BulletType.normal, speed);
            bullets[i + 1].GetComponent<Bullet>().Shoot(dir, BulletType.normal, speed);
            yield return new WaitForSeconds(cadence);
        }
    }
    public void Circunference(GameObject bullet,Vector2 pos,float speed, float acc,int vertex,float radius,bool ordered)
    {
        int pointCount = vertex + 1;
        GameObject[] bullets = new GameObject[pointCount];
        
        for(int i = 0; i < pointCount; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / vertex);
            float x = Mathf.Sin(angle) * radius + pos.x;
            float y = Mathf.Cos(angle) * radius + pos.y;
            Vector2 bulletPos = new Vector2(x, y);
            bullets[i] = Pool.Instance.Recycle(bullet, bulletPos, Quaternion.identity);
        }
        StartCoroutine(IEShotCircle(bullets, speed, acc, ordered, 0.1f));
    }
    IEnumerator IEShotCircle(GameObject[] bullets,float speed,float acc,bool ordered,float cadence = 0)
    {
        if (!ordered)
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < bullets.Length; i++)
            {
                float angle = Mathf.Deg2Rad * (i * 360f /(bullets.Length));
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
                bullets[i].GetComponent<Bullet>().Shoot(dir, BulletType.acc, speed, -acc);
            }
        }
        else
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                float angle = Mathf.Deg2Rad * (i * 360f / (bullets.Length));
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
                bullets[i].GetComponent<Bullet>().Shoot(dir, BulletType.acc, speed, -acc);
                yield return new WaitForSeconds(cadence);
            }
        }
    }
}
