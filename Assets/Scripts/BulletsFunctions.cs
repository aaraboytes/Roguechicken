using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsFunctions : MonoBehaviour
{
    public void SimpleShot(GameObject _bullet, Vector2 pos, Vector2 dir,float speed)
    {
        GameObject bullet = Instantiate(_bullet, pos, Quaternion.identity);
        bullet.GetComponent<Bullet>().Shoot(dir, BulletType.normal, speed);
    }
    public void TripleShot(GameObject bullet, Vector2 pos,Vector2 dir,float speed,float acc)
    {
        GameObject[] bullets = new GameObject[3];
        for(int i = 0; i < 3; i++)
        {
            bullets[i] = Instantiate(bullet, pos, Quaternion.identity);
        }
        bullets[0].GetComponent<Bullet>().Shoot(dir, BulletType.acc, speed, -acc);
        float angle = Mathf.Atan2(dir.y, dir.x);
        float plus = angle + 15f;
        float minus = angle - 15f;
        Vector2 rDir = new Vector2(Mathf.Cos(plus * Mathf.Rad2Deg), Mathf.Sin(plus * Mathf.Rad2Deg));
        Vector2 lDir = new Vector2(Mathf.Cos(minus * Mathf.Rad2Deg), Mathf.Sin(minus * Mathf.Rad2Deg));
        bullets[1].GetComponent<Bullet>().Shoot(rDir, BulletType.acc, speed-2, -acc);
        bullets[2].GetComponent<Bullet>().Shoot(lDir, BulletType.acc, speed-2, -acc);
    }
}
