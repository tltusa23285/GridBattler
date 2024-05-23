public class WSlashAction : ActorAction
{
    public int Dmg = 30;
    public uint Initial = 30;
    public override void Execute(out uint ticksToResolve)
    {
        ticksToResolve = 15;
        AddNext(() => Caller.GCon.TryPlayAnim("ShootAction", Caller.CM.TickManager.TicksToTime(5)));
        AddNext(() => Caller.TryMoveToCell(Caller.Position.x + 2, Caller.Position.y, Caller.CM.TickManager.TicksToTime(5)));
        AddFuture(5, () => PerformAttack());
        AddFuture(5, () => Caller.TryMoveToCell(Caller.Position.x-2, Caller.Position.y, Caller.CM.TickManager.TicksToTime(5)));
        Caller.PerformingAction = true;
        AddFuture(15, () => Caller.PerformingAction = false);
    }

    private void PerformAttack()
    {
        int x = Caller.Position.x;
        int y = Caller.Position.y+1;

        switch (Caller.Facing)
        {
            case Actor.FACING.Right: x++; break;
            case Actor.FACING.Left: x--; break;
            default: throw new System.NotImplementedException();
        }

        for (int i = 0; i < 3; i++)
        {
            if (!(CM.Grid.InBounds(x, y - i))) continue;
            if (CM.Grid.Occupancy.GetOccupantInCell(x, y - i, out Actor a))
            {
                if (a is IDamagable)
                {
                    ((IDamagable)a).Damage(Dmg);
                }
            }
        }
    }
}
