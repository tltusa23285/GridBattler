using UnityEngine;

namespace GBGame.Actors.Actions
{
    public class StepTowardsPlayer : ActorAction
    {
        public enum AxisType { Vertical, Horizontal }

        public float MaxTravelDistance = 1;
        public uint TicksToTravel = 20;
        public AxisType Axis;
        public override void Execute(out uint ticksToResolve)
        {
            ticksToResolve = 0;
            Vector2Int caller_pos = Caller.Position;
            Vector2Int player_pos = Caller.Com.PlayerActor.Position;
            switch (Axis)
            {
                case AxisType.Vertical:
                    if (caller_pos.y == player_pos.y) break;
                    ticksToResolve = TicksToTravel;
                    AddNext(
                        () => Caller.TryMoveToCell(
                            caller_pos.x,
                            caller_pos.y + (int)Mathf.Clamp((player_pos.y - caller_pos.y), -MaxTravelDistance, MaxTravelDistance),
                            TicksToTravel));
                    break;
                case AxisType.Horizontal:
                    if (caller_pos.x == player_pos.x) break;
                    ticksToResolve = TicksToTravel;
                    AddNext(
                        () => Caller.TryMoveToCell(
                            caller_pos.x + (int)Mathf.Clamp(player_pos.x - caller_pos.x, -MaxTravelDistance, MaxTravelDistance),
                            caller_pos.y,
                            TicksToTravel));
                    break;
                default: throw new System.NotImplementedException();
            }
        }
    }
}