﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType { normal,chilli,grenade};
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    bool interactuable = true;
    [SerializeField] bool mousePlay;
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float dashForce;
    [SerializeField] float dashTime;
    bool dashed = false;

    [Header("Shooting")]
    [SerializeField] Transform indicator;
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletForce;
    [SerializeField] float cadence;
    [SerializeField] int maxBullets;
    [SerializeField] int bullets;
    [SerializeField] int ammo;
    bool reloading = false;

    [Header("Bomb")]
    [SerializeField] GameObject normalBomb;
    [SerializeField] GameObject chilliBomb;
    [SerializeField] GameObject grenadeBomb;
    [SerializeField] BombType bType = BombType.normal;
    [SerializeField] float bombForce;
    [SerializeField] float bombTime;
    float bombTimer;

    [Header("Shield")]
    [SerializeField] GameObject shield;
    bool shielded;

    Vector2 dir = Vector2.right;
    Vector2 storedDir;

    Vector2 movement = Vector2.right;
    Vector2 storedMovement;
    Rigidbody2D body;
    AudioSource audio;
    float timer = 0;

    PlayerStats stats;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        storedDir = dir;
        storedMovement = movement;
        stats = GetComponent<PlayerStats>();
        bullets = maxBullets;

        if (Input.GetJoystickNames().Length > 1)
            mousePlay = false;
        else
            mousePlay = true;
    }
   
    void Update()
    {
        if (interactuable)
        {
            #region Movement
            movement = Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up;
            if (movement != Vector2.zero)
                storedMovement = movement.normalized;
            movement *= speed;
            if (mousePlay)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dir = mousePos - (Vector2)transform.position;
            }
            else
            {
                dir = Input.GetAxis("RHorizontal") * Vector2.right + Input.GetAxis("RVertical") * Vector2.down;
            }
            dir.Normalize();
            
            if (dir != Vector2.zero)
                storedDir = dir;
            indicator.transform.localPosition = storedDir;

            if (Input.GetAxisRaw("Dash") > 0 || Input.GetButtonDown("Dash"))
                Dash();
            else
            {
                if (dashed)
                    dashed = false;
            }
            #endregion
            #region Inputs
            if (Input.GetButtonDown("ChangeBomb"))
            {
                ChangeBomb();
            }
            if (Input.GetButtonDown("Interaction"))
            {
                Debug.DrawLine((Vector2)transform.position + storedMovement, (Vector2)transform.position + storedMovement*2, Color.cyan);
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + storedMovement, storedDir,0.5f);
                if(hit.collider!=null){
                    print(hit.collider.gameObject);
                    if (hit.collider.GetComponent<ShopItem>())
                    {
                        hit.collider.GetComponent<ShopItem>().Buy();
                    }else if (hit.collider.GetComponent<Chest>())
                    {
                        hit.collider.GetComponent<Chest>().CheckChest();
                    }
                }
            }
            #endregion
            #region Attack
            timer += Time.deltaTime;
            if (Input.GetAxisRaw("Shoot") > 0 || Input.GetButtonDown("Shoot"))
                if (timer > cadence && !reloading)
                {
                    if (bullets > 0)
                    {
                        Shoot();
                        bullets--;
                    }
                    else
                    {
                        Reload();
                    }
                    timer = 0;
                }
            bombTimer += Time.deltaTime;
            if (Input.GetButtonDown("Bomb") || Input.GetButtonDown("Bomb"))
            {
                if (bombTimer > bombTime)
                {
                    Bomb();
                    bombTimer = 0;
                }
            }
            #endregion
        }
    }
    private void FixedUpdate()
    {
        body.velocity = movement;
    }
    #region Bomb
    void Bomb()
    {
        print("Bomb");
        GameObject bombToInst = null;
        switch (bType)
        {
            case BombType.normal:
                bombToInst = normalBomb;
                break;
            case BombType.chilli:
                bombToInst = chilliBomb;
                break;
            case BombType.grenade:
                bombToInst = grenadeBomb;
                break;
        }
        Instantiate(bombToInst, (Vector2)transform.position + RPGMovFuncs.ClampDir(storedDir), Quaternion.identity);
    }
    void ChangeBomb()
    {
        if (bType != BombType.grenade)
            bType++;
        else
            bType = BombType.normal;
    }
    #endregion
    #region Shoot
    void Shoot()
    {
        print("Shoot");
        GameObject b = Instantiate(bullet, (Vector2)transform.position + storedDir *1.05f, Quaternion.identity);
        b.GetComponent<Bullet>().Shoot(storedDir, BulletType.normal, bulletForce);
    }
    void Reload()
    {
        if (ammo > 0)
        {
            reloading = true;
            ammo--;
            bullets = maxBullets;
        }
        StartCoroutine(IEWaitForReload());
    }
    IEnumerator IEWaitForReload()
    {
        yield return new WaitForSeconds(2);
        reloading = false;
    }
    #endregion
    #region Dash
    void Dash()
    {
        if (!dashed)
        {
            print("Dash");
            dashed = true;
            StartCoroutine(IEDash());
        }
    }
    IEnumerator IEDash()
    {
        interactuable = false;
        stats.MakeInvincible();
        timer = 0;
        body.velocity = Vector2.zero;
        while (timer < dashTime)
        {
            body.velocity = storedMovement * dashForce;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = cadence;
        stats.EndInvincible();
        interactuable = true;
    }
    #endregion
    #region Shield
    public void GiveShield()
    {
        shield.SetActive(true);
    }
    public void BreakShield()
    {
        shield.SetActive(false);
    }
    #endregion
    #region Givers
    public void GiveAmmo(int amount)
    {
        ammo += amount;
    }
    #endregion
}
