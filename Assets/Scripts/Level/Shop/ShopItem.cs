using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { coin,ammo, shield, hp, key, energyBar }
[System.Serializable]
public class ShopItemParams
{
    public Sprite spr;
    public int prize;
    public ItemType item;
    public int amount;
    public bool stockeable;
}
public class ShopItem : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] Text prizeText;
    [SerializeField] Text stockText;
    [SerializeField] int prize;
    [SerializeField] int amount;
    ItemType item;
    bool buyable = false;
    public void Setup(ShopItemParams itemParams)
    {
        prize = itemParams.prize;
        sprite.sprite = itemParams.spr;
        prizeText.text = prize.ToString();
        item = itemParams.item;
        amount = Random.Range(0, 3);
        if (itemParams.stockeable)
        {
            stockText.gameObject.SetActive(true);
            stockText.text = "X" + amount.ToString();
        }
        else
        {
            stockText.gameObject.SetActive(false);
        }
        buyable = true;
    }
    public void Buy()
    {
        if (!buyable) return;
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats.EnoughCoins(prize))
        {
            buyable = false;
            prizeText.text = "SOLD";
            sprite.color = Color.gray;
            stats.SubCoins(prize);
            stats.GiveItem(item,amount);
            stockText.gameObject.SetActive(false);
        }
        else
        {
            NotEnoughMoney();
        }
    }
    void NotEnoughMoney()
    {
        print("Not enough money");
    }
}
