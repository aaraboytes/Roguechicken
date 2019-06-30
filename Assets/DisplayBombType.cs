using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayBombType : MonoBehaviour
{
    [SerializeField] Sprite[] bombsSprites;
    Player player;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var bomb = player.GetBombType();

        switch (bomb)
        {
            case BombType.normal:
                image.sprite = bombsSprites[0];
                break;
            case BombType.chilli:
                image.sprite = bombsSprites[1];
                break;
            case BombType.grenade:
                image.sprite = bombsSprites[2];
                break;
            default:
                break;
        }
    }
}
