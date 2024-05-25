using UnityEngine;

public class StepAction : ActorAction
{
    public Vector2Int Direction;
    public uint Duration = 10;
    public uint InputBuffer = 5;

    public override void Execute(out uint ticksToResolve)
    {
        ticksToResolve = Duration;
        Caller.PerformingAction = true;
        AddNext(() => Caller.TryMoveToCell(Direction, Duration));
        AddFuture(Duration - InputBuffer, ()=> Caller.PerformingAction = false);
    }
}
