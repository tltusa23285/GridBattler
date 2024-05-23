using System.Collections;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public enum FACING { Right, Left }
    public CombatManager CM { get; protected set; }

    [field: SerializeField] public FACING Facing { get; protected set; }
    [field: SerializeField] public ActorGraphicController GCon { get; protected set; }

    protected Vector2Int Pos;
    public Vector2Int Position => Pos;

    private Coroutine MoveCo;

    public bool PerformingAction { get; set; }

    public void Spawn(CombatManager cm, in int x, in int y)
    {
        CM = cm;

        if (!CM.Grid.InBounds(x, y) || !CM.Grid.Occupancy.TryOccupyCell(this, x, y))
        {
            Debug.LogError($"Failed to place actor on {x},{y}");
        }
        else
        {
            CM.Grid.Occupancy.TryOccupyCell(this, x, y);
            Pos.x = x; Pos.y = y;
            this.transform.position = CM.Grid.CoordsToWorldPosition(x, y);
        }

        switch (Facing)
        {
            case FACING.Right:
                this.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                break;
            case FACING.Left:
                this.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                break;
            default:
                throw new System.NotImplementedException();
        }

        OnSpawn();
    }

    protected virtual void OnSpawn() { }

    public void Despawn()
    {
        CancelActions();
        CM.Grid.Occupancy.TryUnOccupyCell(this, Pos.x, Pos.y);
        Destroy(this.gameObject);
    }
    protected abstract void CancelActions();

    #region Movement
    public bool TryMoveToCell(Vector2Int direction, float lerpDuration = 0.15f)
    {
        return TryMoveToCell(Position.x + direction.x, Position.y + direction.y, lerpDuration);
    }

    public bool TryMoveToCell(in int x, in int y, float lerpDuration = 0.15f)
    {
        if (!CM.Grid.InBounds(x, y) || CM.Grid.Occupancy.CellHasOccupant(x, y)) return false;
        if (!CM.Grid.Occupancy.TryUnOccupyCell(this, Pos.x, Pos.y)) return false;
        if (!CM.Grid.Occupancy.TryOccupyCell(this, x, y)) return false;

        Pos.x = x; Pos.y = y;

        if (MoveCo != null) StopCoroutine(MoveCo);
        MoveCo = StartCoroutine(LerpMove(CM.Grid.CoordsToWorldPosition(x, y), lerpDuration));

        return true;
    }

    IEnumerator LerpMove(Vector3 target, float lerpDuration)
    {
        float t = 0;
        Vector3 start = this.transform.position;
        while (t < 1)
        {
            t += Time.deltaTime / lerpDuration;
            this.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }
        this.transform.position = target;
        MoveCo = null;
    } 
    #endregion
}
