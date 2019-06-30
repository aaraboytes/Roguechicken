using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{

     public enum BombType
    {
        Normal,
        Cruz,
        Granada
    }
    public BombType bombType= BombType.Normal;
    public int conteo = 1;
    public float maxT = 1f;
    public GameObject[] prefab_bomb;
    
    public float vel = 2f;
     Rigidbody2D rb2D;

   
    

    Vector2 mov = new Vector2 (0,0);
 
    [SerializeField]
    bool boton = false;
    
    private void Start() {
        
        mov *= vel;
        rb2D = GetComponent<Rigidbody2D>();

    }
    // Start is called before the first frame update
    private RaycastHit2D CheckRaycast (Vector2 direction)
    {
        Vector2 startingPosition= new Vector2 (transform.position.x + direction.x,transform.position.y+direction.y);

        Debug.DrawRay(startingPosition,direction,Color.red);
        
        return Physics2D.Raycast(startingPosition,direction,3f);

    }

    private bool RaycastCheckUpdate()
    {
        if(boton)
        {
            Vector2 directionright = new Vector2(1,0);
            Vector2 directionleft = new Vector2(-1,0);
            Vector2 directionup = new Vector2(0,1);
            Vector2 directiondown = new Vector2(0,-1);
            Vector2 directionrightup = new Vector2(1,1);
            Vector2 directionleftup = new Vector2(-1,1);
            Vector2 directionrightdown = new Vector2(1,-1);
            Vector2 directionleftdown = new Vector2(-1,-1);

            RaycastHit2D hitup =CheckRaycast(directionup);
            RaycastHit2D hitdown =CheckRaycast(directiondown);
            RaycastHit2D hitleft =CheckRaycast(directionleft);
            RaycastHit2D hitright =CheckRaycast(directionright);
            RaycastHit2D hitupright =CheckRaycast(directionrightup);
            RaycastHit2D hitdownright =CheckRaycast(directionrightdown);
            RaycastHit2D hitleftup =CheckRaycast(directionleftup);
            RaycastHit2D hitleftdown =CheckRaycast(directionleftdown);
            
       

            Hit(hitup,Vector2.up);
            Hit(hitdown,new Vector2(0,-1));
            Hit(hitright,Vector2.right);
            Hit(hitleft,new Vector2(-1,0));
            Hit(hitupright,new Vector2(1,1));
            Hit(hitdownright,new Vector2(1,-1));
            Hit(hitleftup, new Vector2 (-1,1));
            Hit(hitleftdown, new Vector2(-1,-1));

            return true;
        }
        else
        {
            return false;
        }
    }
    void Hit(RaycastHit2D hit,Vector3 dir){
        if(hit.collider != null && hit.collider.gameObject.CompareTag("bomba"))
        {
            Debug.Log("hit " + hit.collider.tag);
            print(dir);
            hit.collider.gameObject.GetComponent<BombBehaviour>().Shoot(dir);
            Debug.DrawRay(transform.position,hit.point,Color.red ,1f);
        }
    }
    private void Update() {
       
        //RaycastCheckUpdate();
        

        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(PlayerBombs.currentBombs <PlayerBombs.MaxBombs)
            {
                SpawnBomb();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            boton = true;
            Debug.Log ("F");
            if(RaycastCheckUpdate())
            {
                boton = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(conteo <= 2)
            {
                if(conteo == 0)
                {
                    bombType = BombType.Normal;
                    Debug.Log("normal");
                }
                else if(conteo == 1)
                {
                    bombType = BombType.Cruz;
                    Debug.Log("cruz");
                }
                else if(conteo == 2)
                {
                    bombType = BombType.Granada;
                    Debug.Log("granada");
                   
                }
                //bomb = prefab_bomb[conteo].GetComponent<BombBehaviour>();
            }
            else
            {
                conteo = -1;
            }
            conteo++;
        }
        
    }
   
  
    void SpawnBomb()
    {
        GameObject go = null;
        switch(bombType)
        {
            case BombType.Normal:   go = prefab_bomb[0];
                                     
            break;

            case BombType.Cruz:   go = prefab_bomb[1];
            break;

            case BombType.Granada:   go = prefab_bomb[2];

            break;
        }
        GameObject goInst = Instantiate(go,transform.position, transform.rotation);
        PlayerBombs.currentBombs ++;
    }

  
}

