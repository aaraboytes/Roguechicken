using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle, Move, Shoot};
public abstract class Enemy : MonoBehaviour
{
    public abstract void Damage(int damage);
    public abstract void Die();
    public abstract bool Alive();
    public abstract void SetRoomHandler(Room m_Room);
}
