using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class Actor : MonoBehaviour
{
    #region Static
    /// <summary>
    /// Instantiates an Actor Instance
    /// Handles loading and instantion of an object
    /// </summary>
    /// <returns></returns>
    public static bool Instantiate(CombatManager cm, string toSpawn, in int x, in int y, out Actor result)
    {
        result = null;
        GameObject go = Addressables.LoadAssetAsync<GameObject>(toSpawn).WaitForCompletion();
        if (go == null)
        {
            Debug.LogError($"No asset of id {toSpawn}");
            return false;
        }

        go = Instantiate(go);
        if (!go.TryGetComponent(out result))
        {
            Debug.LogError($"Asset is missing Actor component {toSpawn}");
            return false;
        }
        if (!result.Spawn(cm, x, y))
        {
            Debug.LogError($"Failed to spawn {toSpawn} at [{x},{y}]");
            return false;
        }
        return true;
    } 
    #endregion

    public enum FACING { Right, Left }
    public CombatManager Com { get; protected set; }

    [field: SerializeField] public FACING Facing { get; protected set; }
    [field: SerializeField] public ActorGraphicController GraphicControl { get; protected set; }

    protected Vector2Int Pos;
    public Vector2Int Position => Pos;
    private Vector3 WorldPosition;
    private Coroutine MoveCo;
    public virtual string ActorId { get => this.gameObject.name; protected set => this.gameObject.name = value; }

    /// <summary>
    /// Initializes the actor
    /// </summary>
    /// <param name="cm"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private bool Spawn(CombatManager cm, in int x, in int y)
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
            WorldPosition = Com.Grid.CoordsToWorldPosition(x, y);
            this.transform.position = WorldPosition;
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

        return OnSpawn();
    }

    protected virtual bool OnSpawn() { return true; }

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
        MoveCo = StartCoroutine(LerpMove(Com.Grid.CoordsToWorldPosition(x, y), tickDuration));

        return true;
    }

    IEnumerator LerpMove(Vector3 target, uint duration)
    {
        Vector3 start = WorldPosition;

        ulong start_time = Com.TickManager.CurrentTick;
        ulong end_time = start_time + duration;

        while (true)
        {
            if (Com.TickManager.CurrentTick == end_time) break;
            WorldPosition = Vector3.Lerp(start, target, 
                Mathf.InverseLerp(start_time,end_time, Com.TickManager.CurrentTick));
            yield return null;
        }

        WorldPosition = target;
        MoveCo = null;
    }
    #endregion

    private void LateUpdate()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, WorldPosition, Time.deltaTime / Com.TickManager.TickRate);
    }
}
