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
    protected CombatManager CM => Caller.CM;
    protected GridManager Grid => Caller.CM.Grid;

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
}