using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdBombBehaviour : MonoBehaviour
{
    public float Timer2 = 2f;
    

    public GameObject ExplosionPrefab;
    // Start is called before the first frame update

    // Update is called once per frame
    private void Update()
    {
        Timer2 -= Time.deltaTime;
        if(Timer2<= 0)
        {
            Explode();

        }
    }

    void Explode()
    {
        GameObject go = (GameObject)Instantiate(ExplosionPrefab, transform.position , transform.rotation);
        PlayerBombs.currentBombs--;
        Destroy(gameObject);
    }
}
