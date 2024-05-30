using UnityEngine;

public class StepAction : ActorAction
{
    public Vector2Int Direction;
    public uint Duration = 10;

    public override void Execute(out uint ticksToResolve)
    {
        ticksToResolve = Duration;
        AddNext(() => Caller.TryMoveToCell(Direction, Duration));
    }
}
