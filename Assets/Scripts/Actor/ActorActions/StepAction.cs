using UnityEngine;

public class StepAction : ActorAction
{
    public Vector2Int Direction;
    public float Duration = 0.15f;
    public float Buffer = .05f;

    public override void Execute(out uint ticksToResolve)
    {
        ticksToResolve = CM.TickManager.TimeToTicks(Duration - Buffer);
        Caller.PerformingAction = true;
        AddNext(() => Caller.TryMoveToCell(Direction, Duration));
        AddFuture(ticksToResolve, ()=> Caller.PerformingAction = false);
    }
}
