using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class EnemyActorController : IJsonObject
{
    public const string LIB_FOLDER_NAME = "EnemyControllers";
    public const string LIB_FILE_NAME = "EnemyLibrary";

    public string EnemyID;
    public string DisplayName => EnemyID;

    [Header("Stats")]
    public int Health = 100;

    [Header("Graphics")]
    public string Prefab;

    string IJsonObject.JsonID => EnemyID;

    protected Actor Caller { get; private set; }
    protected CombatManager Com => Caller.Com;
    protected GridManager Grid => Caller.Com.Grid;

    public abstract void AIStep();
    public bool Setup(EnemyActor caller)
    {
        Caller = caller;
        return OnSetup();
    }
    protected abstract bool OnSetup();

    private HashSet<TickManager.TickCancelToken> CancelTokens = new HashSet<TickManager.TickCancelToken>();

    public void CancelActions()
    {
        foreach (var item in CancelTokens) item.ToCancel.Invoke();
        CancelTokens.Clear();
        OnActionsCancelled();
    }
    protected virtual void OnActionsCancelled() { }

    private ScheduledAction GenAction(Action action, string msg = default)
    {
        return new ScheduledAction(
            action,
            $"[{Caller.ActorId}] Performing [{this.EnemyID}] {msg}"
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
}