using UnityEngine;

public class Archer : EnemyActorController
{
    [Header("Archer")]
    public string AttackActionID = "ArcherShot";
    public string MoveActionID = "StepToPlayerRow1";
    public float TimeBetweenActions;

    private ActorAction AttackAction;
    private ActorAction MoveAction;

    private uint ActionTicks;
    public override void AIStep()
    {
        if (Caller.Position.y != Caller.CM.Player.Position.y)
        {
            MoveAction.Execute(out ActionTicks);
        }
        else
        {
            AttackAction.Execute(out ActionTicks);
        }
        AddFuture(CM.TickManager.TimeToTicks(TimeBetweenActions) + ActionTicks, AIStep);
    }

    protected override bool OnSetup()
    {
        IJsonObjectWrapper<ActorAction> result;
        if (!CM.ActionLibrary.GetItem(AttackActionID, out result)) return false;
        (AttackAction = result.Object).Setup(Caller);
        if (!CM.ActionLibrary.GetItem(MoveActionID, out result)) return false;
        (MoveAction = result.Object).Setup(Caller);

        AddFuture(base.Caller.CM.TickManager.TimeToTicks(TimeBetweenActions), AIStep);

        return true;
    }

    protected override void OnActionsCancelled()
    {
        AttackAction.CancelAction();
        MoveAction.CancelAction();
    }
}