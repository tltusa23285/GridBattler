using System;
using System.Collections.Generic;

[System.Serializable]
public abstract class ActorAction : IJsonObject
{
    public const string LIB_FOLDER_NAME = "ActorActions";
    public const string LIB_FILE_NAME = "ActionLibrary";

    public string ActionID;

    string IJsonObject.JsonID => ActionID;

    protected Actor Caller { get; private set; }
    protected CombatManager Com => Caller.Com;
    protected GridManager Grid => Caller.Com.Grid;

    public void Setup(Actor actor) 
    { 
        Caller = actor;
        OnSetup();
    }

    public void SetDown(Actor actor)
    {
        CancelAction();
        OnSetdown();
    }

    protected virtual void OnSetup() { }
    protected virtual void OnSetdown() { }

    /// <summary>
    /// Executes this action
    /// </summary>
    /// <param name="caller"></param>
    public void Execute() => Execute(out _);
    /// <summary>
    /// Executes this action, returning how long the action should take to resolve in full
    /// </summary>
    public abstract void Execute(out uint ticksToResolve);


    private HashSet<TickManager.TickCancelToken> CancelTokens = new HashSet<TickManager.TickCancelToken>();

    private ScheduledAction GenAction(Action action, string msg = default)
    {
        return new ScheduledAction(
            action,
            $"[{Caller.ActorId}] Performing [{this.ActionID}] {msg}"
            );
    }

    protected void AddNext(Action action, string msg = default)
    {
        Com.TickManager.RegisterToNextTick(GenAction(action, msg), out TickManager.TickCancelToken token);
        CancelTokens.Add(token);
    }

    protected void AddFuture(in uint ticks, Action action, string msg = default)
    {
        Com.TickManager.RegisterToFutureTick(ticks, GenAction(action, msg), out TickManager.TickCancelToken token);
        CancelTokens.Add(token);
    }

    public void CancelAction()
    {
        foreach (var item in CancelTokens) item.ToCancel.Invoke();
        CancelTokens.Clear();
        ActionCancelled();
    }

    protected virtual void ActionCancelled() { }
}
