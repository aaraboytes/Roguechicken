using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject[] itemSlots;
    [SerializeField] ShopItemParams[] shopItems;
    private void Start()
    {
        GenerateItems();
    }
    public void GenerateItems()
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            ShopItemParams item = shopItems[Random.Range(0, shopItems.Length)];
            itemSlots[i].GetComponent<ShopItem>().Setup(item);
        }
    }
}
