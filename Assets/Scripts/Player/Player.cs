using System.Collections;
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
    [SerializeField] float knockBackForce;
    [SerializeField] float knockBackTime;
    bool whiteKb = false;

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
    Transform animatorChild;
    Animator anim;
    AudioSource audio;
    float timer = 0;

    PlayerStats stats;
    

    private void Start()
    {
        animatorChild = transform.GetChild(0);
        anim = animatorChild.gameObject.GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();

        audio = GetComponent<AudioSource>();
        storedDir = dir;
        storedMovement = movement;
        stats = GetComponent<PlayerStats>();
        bullets = maxBullets;

        mousePlay = true;
    }
   
    void Update()
    {
        anim.SetFloat("velocity", Mathf.Abs(body.velocity.x + body.velocity.y));
        if (body.velocity.x > 0)
        {
            animatorChild.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            animatorChild.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Mathf.Abs(Input.GetAxis("RHorizontal") + Input.GetAxis("RVertical")) > 0)
        {
            mousePlay = false;
        }
        else
        {
            mousePlay = true;
        }
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
        if(interactuable)
            body.velocity = movement;
    }
    #region Health
    public void KnockBack(Vector2 pos)
    {
        interactuable = false;
        stats.MakeInvincible();
        body.AddForce(((Vector2)transform.position - pos).normalized, ForceMode2D.Impulse);
        StartCoroutine(IEKnockBack());
    }
    IEnumerator IEKnockBack()
    {
        float time = 0;
        float middleTime = 0;
        while (time < knockBackTime)
        {
            time += Time.deltaTime;
            middleTime += Time.deltaTime;
            if (middleTime > 0.01f)
            {
                whiteKb = !whiteKb;
                middleTime = 0;
                if (whiteKb)
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
                }
                else
                {
                    transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            yield return null;
        }
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        stats.EndInvincible();
        interactuable = true;
    }
    #endregion
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
        Pool.Instance.Recycle(bombToInst, (Vector2)transform.position + RPGMovFuncs.ClampDir(storedDir), Quaternion.identity);
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
        GameObject b = Pool.Instance.Recycle(bullet, (Vector2)transform.position + storedDir *1.05f, Quaternion.identity);
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
            anim.SetTrigger("dash");
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
            print("Dashing");
            timer += Time.deltaTime;
            yield return null;
        }
        timer = cadence;
        stats.EndInvincible();
        interactuable = true;
        dashed = false;
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
