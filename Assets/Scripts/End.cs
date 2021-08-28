using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public static Vector2 registeredEndPosition { get; private set; }
    private void Awake()
    {
        RegisterEnd();
    }

    private void RegisterEnd()
    {
        registeredEndPosition = transform.position;
    }

    public static bool IsEnd(Vector2 pos)
    {
        if(registeredEndPosition.x == pos.x && registeredEndPosition.y == pos.y)
        {
            return true;
        }
        return false;
    }
}
