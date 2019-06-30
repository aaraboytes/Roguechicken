using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayGodenEggs : MonoBehaviour
{

    PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshProUGUI>().SetText("X " + playerStats.GetCoins().ToString());
    }
}
