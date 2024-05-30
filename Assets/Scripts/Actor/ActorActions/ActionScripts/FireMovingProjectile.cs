using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FireMovingProjectile : ActorAction
{
    public int Damage = 20;
    public uint TicksToTravel = 20;

    private GameObject Projectile;

    private List<Vector2Int> Targets;

    public override void Execute(out uint ticksToResolve)
    {
        ticksToResolve = 1;
        Targets = Grid.GetRowToEdge(Caller.Position, Caller.Facing == Actor.FACING.Right ? 1 : -1);
        AddNext(() => SpawnProjectile(Caller.Position));
        foreach (Vector2Int target in Targets)
        {
            ticksToResolve += TicksToTravel;
            AddFuture(ticksToResolve, () => MoveProjectile(target));
            AddFuture(ticksToResolve, () => Grid.Renderer.SetTargetFlash(target.x, target.y, true));
            AddFuture(ticksToResolve + TicksToTravel, () => Grid.Renderer.SetTargetFlash(target.x, target.y, false));
            AddFuture(ticksToResolve, () => AttackTile(target));
        }
        AddFuture(ticksToResolve + TicksToTravel, () => DestroyProjectile());
    }

    protected override void ActionCancelled()
    {
        foreach (var item in Targets)
        {
            Grid.Renderer.SetTargetFlash(item.x, item.y, false);
        }
        DestroyProjectile();
    }

    private void SpawnProjectile(Vector2Int target)
    {
        GameObject go = Addressables.LoadAssetAsync<GameObject>("ArrowPrefab").WaitForCompletion();
        Projectile = GameObject.Instantiate(go);
        Projectile.transform.position = Grid.CoordsToWorldPosition(target.x, target.y);
    }

    private void MoveProjectile( Vector2Int target)
    {
        Projectile.transform.position = Grid.CoordsToWorldPosition(target.x, target.y);
    }

    private void DestroyProjectile()
    {
        GameObject.Destroy(Projectile);
        Projectile = null;
    }

    private void AttackTile(Vector2Int target)
    {
        if (Grid.Occupancy.GetOccupantInCell(target.x, target.y, out Actor a))
        {
            if (a is IDamagable)
            {
                ((IDamagable)a).Damage(20);
                this.CancelAction();
            }
        }
    }
}