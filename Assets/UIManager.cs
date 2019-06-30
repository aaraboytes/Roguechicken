using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite fullSprites;
    [SerializeField] Sprite emptySprites;
    [SerializeField] int lives;

    PlayerStats playerStats;

    // Start is called before the first frame update
    void Start()
    {
        lives = hearts.Length * 2;
        playerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        int currentPlayerHealth = playerStats.GetHealth();
     
        for(int i = 0; i < hearts.Length; i++)
        {
            if(currentPlayerHealth > i * 2)
            {
                hearts[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                hearts[i].color = new Color(1, 1, 1, 0);
            }

            if(currentPlayerHealth == (i+1) * 2 - 1)
            {
                hearts[i].sprite = emptySprites;
            }

        }
    }
}
