using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] int maxHealth;
    [Header("In game")]
    [SerializeField] int health;
    [SerializeField] int coins;
    [SerializeField] int keys;
    [SerializeField] bool invincible;
    bool shielded = false;
    Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    #region Health
    public void Damage(int damage,Vector2 pos)
    {
        if (shielded)
        {
            player.BreakShield();
            shielded = false;
        }
        health-=damage;
        if (health <= 0)
        {
            GameManager.Instance.GameOver();
        }
        player.KnockBack(pos);
    }
    public void GiveHeal(int heal)
    {
        health += heal;
        if (health > maxHealth)
            health = maxHealth;
    }
    public void MakeInvincible()
    {
        invincible = true;
    }
    public void EndInvincible()
    {
        invincible = false;
    }
    public bool Invincible()
    {
        return invincible;
    }

    public int GetHealth()
    {
        return health;
    }

    #endregion
    #region Money
    public void Coin()
    {
        coins++;
    }
    public bool EnoughCoins(int _coins)
    {
        return coins - _coins >= 0;
    }
    public void SubCoins(int _coins)
    {
        coins -= _coins;
    }
    public int GetCoins()
    {
        return coins;
    }
    #endregion
    #region Keys
    public void GiveKey()
    {
        keys++;
    }
    public bool HaveKey()
    {
        return keys > 0;
    }
    public void UseKey()
    {
        keys--;
    }

    public int GetKeys()
    {
        return keys;
    }
    #endregion
    #region Shield
    public void GiveShield()
    {
        player.GiveShield();
        shielded = true;
    }
    #endregion
    #region Ammo
    public void GiveAmmo(int amount)
    {
        player.GiveAmmo(amount);
    }
    public int GetAmmo()
    {
        return player.GetAmmo();
    }
    #endregion
    #region Items
    public void GiveItem(ItemType item, int amount)
    {
        switch (item)
        {
            case ItemType.ammo:
                GiveAmmo(amount);
                break;
            case ItemType.hp:
                GiveHeal(amount);
                break;
            case ItemType.key:
                GiveKey();
                break;
            case ItemType.shield:
                GiveShield();
                break;
            case ItemType.energyBar:
                //Some shit
                break;
        }
    }
    #endregion
}
