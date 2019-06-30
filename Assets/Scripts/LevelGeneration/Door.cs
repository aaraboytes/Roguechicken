using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject door;
    public void Open()
    {
        door.SetActive(false);
    }
    public void Close()
    {
        door.SetActive(true);
    }
}
