using UnityEngine;
using UnityEngine.Tilemaps;

public class Wall : MonoBehaviour
{
    private Tilemap tilemap = null;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        RegisterWallPos();
    }

    void RegisterWallPos()
    {
        for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; i++)
        {
            for (int j = tilemap.cellBounds.yMin; j < tilemap.cellBounds.yMax; j++)
            {
                Vector3Int gridPos = new Vector3Int(i, j, RegisterPosition.registeredZPos);
                if (tilemap.HasTile(gridPos))
                {
                    Vector3 worldPosAltered = tilemap.CellToWorld(gridPos);
                    worldPosAltered = new Vector3(worldPosAltered.x + RegisterPosition.registeredPosOffset, worldPosAltered.y + RegisterPosition.registeredPosOffset, RegisterPosition.registeredZPos);
                    if (!RegisterPosition.registeredWallPositions.Contains(worldPosAltered))
                    {
                        RegisterPosition.registeredWallPositions.Add(worldPosAltered);
                    }
                }
            }
        }
    }
}
