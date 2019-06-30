using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies = new List<Enemy>();
    [SerializeField] List<Door> doors = new List<Door>();
    private void Start()
    {
        foreach(Enemy e in enemies)
        {
            e.SetRoomHandler(this);
            e.gameObject.SetActive(false);
        }
        foreach(Door d in doors)
        {
            d.Open();
        }
    }
    public void EnterRoom()
    {
        if (AlivesInScene())
        {
            foreach (Enemy e in enemies)
                e.gameObject.SetActive(true);
            foreach (Door d in doors)
                d.Close();
        }
    }
    public void NoticeADead()
    {
        if (!AlivesInScene())
        {
            foreach (Door d in doors)
                d.Open();
        }
    }
    public bool AlivesInScene()
    {
        foreach (Enemy e in enemies)
            if (e.Alive()) return true;
        return false;
    }
}
