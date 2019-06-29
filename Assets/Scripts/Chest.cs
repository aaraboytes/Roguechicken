using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemGameObjectRelation
{
    public ItemType item;
    public GameObject go;
}
public class Chest : MonoBehaviour
{
    [SerializeField] GameObject coin;
    [SerializeField] ItemGameObjectRelation[] items;
    ItemType[] inside = new ItemType[0];
    int insideCoins;
    bool opened;
    private void Start()
    {
        GenerateChest();
    }
    void GenerateChest()
    {
        if (true)
        {
            inside = new ItemType[(int)Random.Range(1, 3)];
            for(int i = 0; i < inside.Length; i++)
            {
                inside[i] = SelectRandItem();
            }
        }
        insideCoins = Random.Range(3,8);
    }
    ItemType SelectRandItem()
    {
        ItemType item;
        item = ItemType.ammo;
        item += Random.Range(0, 4);
        return item;
    }
    public void CheckChest()
    {
        if (opened)
            return;
        PlayerStats ps = FindObjectOfType<PlayerStats>();
        if (ps.HaveKey())
        {
            ps.UseKey();
            Open();
        }
    }
    void ImpulseItem(GameObject go,Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
    }
    Vector2 RandomDir()
    {
        return new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
    }
    public void Open()
    {
        opened = true;
        for(int i = 0; i < insideCoins; i++)
        {
            Vector2 dir = RandomDir();
            GameObject currentCoin = Instantiate(coin, (Vector2)transform.position + dir,Quaternion.identity);
            ImpulseItem(currentCoin,dir);
        }
        for(int i = 0; i < inside.Length; i++)
        {
            foreach(ItemGameObjectRelation obj in items)
            {
                if (obj.item.Equals(inside[i]))
                {
                    Vector2 dir = RandomDir();
                    GameObject currentItem = Instantiate(obj.go, (Vector2)transform.position + dir, Quaternion.identity);
                    ImpulseItem(currentItem,dir);
                    break;
                }

            }
        }
    }
}
