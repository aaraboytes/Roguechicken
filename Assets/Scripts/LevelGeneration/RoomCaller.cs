using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCaller : MonoBehaviour
{
    [SerializeField]Room myRoom;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        myRoom.EnterRoom();
    }
}
