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
    protected CombatManager CM => Caller.CM;
    protected GridManager Grid => Caller.CM.Grid;

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

    protected void AddNext(Action action)
    {
        CM.TickManager.RegisterToNextTick(action, out TickManager.TickCancelToken token);
        CancelTokens.Add(token);
    }

    protected void AddFuture(in uint ticks, Action action)
    {
        CM.TickManager.RegisterToFutureTick(ticks, action, out TickManager.TickCancelToken token);
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
