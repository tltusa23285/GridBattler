using UnityEngine;
using UnityEngine.AddressableAssets;

public class GridRenderer
{
    private GridManager Grid;

    private CellGraphicController[,] Tiles;

    public GridRenderer(GridManager grid)
    {
        Grid = grid;
        Tiles = new CellGraphicController[grid.GridSize.x, grid.GridSize.y];
        GameObject prefab = Addressables.LoadAssetAsync<GameObject>("TilePrefab").WaitForCompletion();
        GameObject go;
        foreach (var item in Grid.Cells)
        {
            go = GameObject.Instantiate(prefab, Grid.CM.transform);
            Tiles[item.x, item.y] = go.GetComponent<CellGraphicController>();
            Tiles[item.x, item.y].Position = CoordsToWorldPositon(item.x, item.y);
        }
    }

    public Vector3 CoordsToWorldPositon(in int x, in int y)
    {
        return new Vector3(x, 0, y);
    }

    public void SetTargetFlash(in int x, in int y, bool on) => Tiles[x, y].SetTargetFlash(on);
}
