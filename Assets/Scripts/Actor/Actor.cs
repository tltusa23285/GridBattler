using System.Collections;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public enum FACING { Right, Left }
    public CombatManager Com { get; protected set; }

    [field: SerializeField] public FACING Facing { get; protected set; }
    [field: SerializeField] public ActorGraphicController GCon { get; protected set; }

    protected Vector2Int Pos;
    public Vector2Int Position => Pos;
    private Coroutine MoveCo;

    public bool PerformingAction { get; set; }
    public virtual string ActorId { get => this.gameObject.name; protected set => this.gameObject.name = value; }

    public void Spawn(CombatManager cm, in int x, in int y)
    {
        Com = cm;

        if (!Com.Grid.InBounds(x, y) || !Com.Grid.Occupancy.TryOccupyCell(this, x, y))
        {
            Debug.LogError($"Failed to place actor on {x},{y}");
        }
        else
        {
            Com.Grid.Occupancy.TryOccupyCell(this, x, y);
            Pos.x = x; Pos.y = y;
            this.transform.position = Com.Grid.CoordsToWorldPosition(x, y);
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
        Com.Grid.Occupancy.TryUnOccupyCell(this, Pos.x, Pos.y);
        Destroy(this.gameObject);
    }
    protected abstract void CancelActions();

    #region Movement
    public bool TryMoveToCell(Vector2Int direction, uint tickDuration)
    {
        return TryMoveToCell(Position.x + direction.x, Position.y + direction.y, tickDuration);
    }

    public bool TryMoveToCell(in int x, in int y, uint tickDuration)
    {
        if (!Com.Grid.InBounds(x, y) || Com.Grid.Occupancy.CellHasOccupant(x, y)) return false;
        if (!Com.Grid.Occupancy.TryUnOccupyCell(this, Pos.x, Pos.y)) return false;
        if (!Com.Grid.Occupancy.TryOccupyCell(this, x, y)) return false;

        Pos.x = x; Pos.y = y;

        if (MoveCo != null) StopCoroutine(MoveCo);
        MoveCo = StartCoroutine(LerpMove(Com.Grid.CoordsToWorldPosition(x, y), Com.TickManager.TicksToTime(tickDuration)));

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
