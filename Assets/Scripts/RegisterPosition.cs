using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RegisterPosition
{
    public static List<Vector2> registeredWallPositions { get; private set; } = new List<Vector2>();
    public static List<Vector2> registeredPlayerPositions { get; private set; } = new List<Vector2>();
    public static int registeredZPos { get => 0; }
    public static float registeredPosOffset { get => 0.5f; }
    public static void Reset()
    {
        registeredWallPositions = new List<Vector2>();
        registeredPlayerPositions = new List<Vector2>();
    }
}
