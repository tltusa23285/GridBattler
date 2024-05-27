using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShootAction : ActorAction
{
    public int Damage = 10;
    public uint Startup = 30;
    public uint LockTime = 60;

    public override void Execute(out uint ticksToResolve)
    {
        AddNext(() => Caller.GraphicControl.TryPlayAnim("ShootAction", LockTime));

        List<Vector2Int> targets = Grid.GetRowToEdge(Caller.Position, Caller.Facing == Actor.FACING.Right ? 1 : -1);

        AddFuture((uint)Mathf.Min(10, Startup), () => HighlightTiles(Caller, targets, true));
        AddFuture(Startup+1, () => HighlightTiles(Caller, targets, false));

        AddFuture(Startup, () => PerformAttack(Caller,targets));

        Caller.PerformingAction = true;
        AddFuture(LockTime, () => Caller.PerformingAction = false);

        ticksToResolve = LockTime;
    }

    private void HighlightTiles(Actor caller, List<Vector2Int> targets, bool on)
    {
        foreach (var item in targets)
        {
            caller.Com.Grid.Renderer.SetTargetFlash(item.x, item.y, on);
        }
    }

    private void PerformAttack(Actor caller, List<Vector2Int> targets)
    {
        foreach (var item in targets)
        {
            if (caller.Com.Grid.Occupancy.GetOccupantInCell(item.x,item.y, out Actor a))
            {
                if (a is IDamagable)
                {
                    ((IDamagable)a).Damage(Damage);
                }
                break;
            }
        }
    }

}
