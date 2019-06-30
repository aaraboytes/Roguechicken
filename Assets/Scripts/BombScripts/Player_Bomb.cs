using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Bomb : MonoBehaviour
{
    public float vel =5f;
    Rigidbody2D rigi;

    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
       float h = Input.GetAxis("Horizontal");
       float v = Input.GetAxis("Vertical");

       Vector2 mov = new Vector2 (h,v);
       mov *= vel;
       rigi.velocity = mov;

    }
}
