using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RPGMovFuncs 
{
    public static Vector2 ClampDir(Vector2 dir)
    {
        Vector2 clamped = Vector2.zero;
        if (dir.x < 0.25f && dir.x > -0.25f)
        {
            if (dir.y > 0)
                return Vector2.up;
            else
                return Vector2.down;
        }
        else if (dir.x > 0.25f && dir.x < 0.75f)
        {
            if (dir.y > 0)
                return Vector2.up + Vector2.right;
            else
                return Vector2.down + Vector2.right;
        }
        else if (dir.x < -0.25f && dir.x > -0.75f)
        {
            if (dir.y > 0)
                return Vector2.up + Vector2.left;
            else
                return Vector2.down + Vector2.left;
        }
        else if (dir.x > 0.75f)
        {
            return Vector2.right;
        }
        else if (dir.x < -0.75f)
            return Vector2.left;
        return clamped;
    }
}
