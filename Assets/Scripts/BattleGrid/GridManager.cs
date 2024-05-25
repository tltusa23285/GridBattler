using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    public struct GridCell
    {
        public readonly int x, y;
        public GridCell(int _x, int _y)
        {
            x = _x; y = _y;
        }
    }

    public GridRenderer Renderer { get; private set; }
    public OccupancyTracker Occupancy { get; private set; }

    public readonly CombatManager Com;
    public readonly Vector2Int GridSize;
    public readonly GridCell[,] Cells;

    public GridManager(CombatManager cm, int width, int height)
    {
        Com = cm;
        GridSize = new Vector2Int(width, height);
        Cells = new GridCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new GridCell(x, y);
            }
        }
        Renderer = new GridRenderer(this);
        Occupancy = new OccupancyTracker(this);

        Vector3 pos = CoordsToWorldPosition(0, 0);
        pos += CoordsToWorldPosition(width - 1, height - 1);
        pos /= 2;
        cm.CameraController.Position = pos;
        cm.CameraController.Rotation = Quaternion.Euler(45,0,0);
        cm.CameraController.Distance = 5;
    }

    public Vector3 CoordsToWorldPosition(in int x, in int y)
    {
        return Renderer.CoordsToWorldPositon(x, y);
    }

    public bool InBounds(in int x, in int y)
    {
        return x >= 0 && x < GridSize.x && y>= 0 && y < GridSize.y;
    }

    #region Targeting Utility

    public List<Vector2Int> GetRowToEdge(in Vector2Int origin, in int step)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        int x = origin.x;
        int y = origin.y;
        x += step;
        while (InBounds(x, y))
        {
            result.Add(new Vector2Int(x, y));
            x += step;
        }
        return result;
    }

    #endregion
}
