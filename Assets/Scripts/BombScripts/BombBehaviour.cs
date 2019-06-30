using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
   public bool boton;
   Rigidbody2D rigi ;
    public float Timer = 3f;
   
    
     public float vel =5f;

    public GameObject ExplosionPrefab;
    // Start is called before the first frame update

    public void Start() {
        rigi = GetComponent<Rigidbody2D>();
        rigi.isKinematic=true;
    }
   
    private void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer<= 0)
        {
            Explode();
        }

    }
    public void Shoot(Vector2 dir){
        rigi.isKinematic=false;
        rigi.velocity = dir*vel;
        print(rigi.velocity);

    }    
    void Explode()
    {
        GameObject go = (GameObject)Instantiate(ExplosionPrefab, transform.position , transform.rotation);
        PlayerBombs.currentBombs--;
        Destroy(gameObject);
    }
    
    
}
