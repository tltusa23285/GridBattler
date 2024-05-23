public class OccupancyTracker
{
    private readonly GridManager Grid;

    private readonly Actor[,] Occupants;

    public OccupancyTracker(GridManager grid)
    {
        Grid = grid;
        Occupants = new Actor[Grid.GridSize.x, Grid.GridSize.y];
    }

    public bool CellHasOccupant(in int x, in int y)
    {
        return Occupants[x,y] != null;
    }

    public bool TryOccupyCell(in Actor a, in int x, in int y)
    {
        if(CellHasOccupant(x,y)) return false;
        Occupants[x, y] = a;
        return true;
    }

    public bool TryUnOccupyCell(in Actor a, in int x, in int y)
    {
        if (Occupants[x,y] != a) return false;
        Occupants[x, y] = null;
        return true;
    }

    public bool GetOccupantInCell(in int x, in int y, out Actor a)
    {
        a = Occupants[x, y];
        return a != null;
    }
}
