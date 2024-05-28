using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : Actor, IDamagable
{
    public enum MOVE_DIR { Up, Down, Left, Right};

    private ActorAction BasicAttack;

    private ActorAction StepUp;
    private ActorAction StepDown;
    private ActorAction StepLeft;
    private ActorAction StepRight;

    public uint InputBuffer;
    private ulong NextFreeTick;
    private ActorAction CurrentAction;
    private ActorAction BufferedAction;
    private List<TickManager.TickCancelToken> CancelTokens = new List<TickManager.TickCancelToken>();
    private bool PerformingAction { get; set; }


    private void Awake()
    {
        AwakeDamagable();
    }
    void Start()
    {
        StartDamagable();
    }

    protected override bool OnSpawn()
    {
        base.OnSpawn();
        IJsonObjectWrapper<ActorAction> result;
        if (Com.ActionLibrary.GetItem("BasicShoot"   , out result)) (BasicAttack = result.Object).Setup(this);
        if (Com.ActionLibrary.GetItem("_pStepUp"     , out result)) (StepUp      = result.Object).Setup(this);
        if (Com.ActionLibrary.GetItem("_pStepDown"   , out result)) (StepDown    = result.Object).Setup(this);
        if (Com.ActionLibrary.GetItem("_pStepLeft"   , out result)) (StepLeft    = result.Object).Setup(this);
        if (Com.ActionLibrary.GetItem("_pStepRight"  , out result)) (StepRight   = result.Object).Setup(this);

        return true;
    }

    private void BufferAction(ActorAction action)
    {
        if (BufferedAction != null) return;
        if (PerformingAction)
        {
            ulong tick_diff = NextFreeTick - Com.TickManager.CurrentTick;
            if (tick_diff <= InputBuffer) // within buffer, queue action
            {
                BufferedAction = action;
                Com.TickManager.RegisterToFutureTick(tick_diff, new ScheduledAction(ExecuteBufferedAction,"Player Buffered"), out TickManager.TickCancelToken token);
                CancelTokens.Add(token);
            } 
            else return;
        }
        else
        {
            PerformingAction = true;
            CurrentAction = action;
            CurrentAction.Execute(out uint ticks);
            NextFreeTick = Com.TickManager.CurrentTick + ticks;
            Com.TickManager.RegisterToFutureTick(ticks, new ScheduledAction(() => PerformingAction = false, $"Player freeing action"), out TickManager.TickCancelToken token);
        }
    }

    private void ExecuteBufferedAction()
    {
        if (BufferedAction == null) 
        {
            Debug.LogError($"Tried to perform null buffered action;");
            return;
        }
        ActorAction action = BufferedAction;
        BufferedAction = null;
        BufferAction(action);
    }

    public void PerformMovement(MOVE_DIR dir)
    {
        switch (dir)
        {
            case MOVE_DIR.Up:   BufferAction(StepUp); break;
            case MOVE_DIR.Down: BufferAction(StepDown) ; break;
            case MOVE_DIR.Left: BufferAction(StepLeft) ; break;
            case MOVE_DIR.Right:BufferAction(StepRight); break;
            default: throw new NotImplementedException();
        }
    }

    public void PerformBasicAttack()
    {
        BufferAction(BasicAttack);
    }

    public void PerformSelectedAttack()
    {
        Debug.Log($"Performing Selected");
    }


    #region IDamageable
    [Header("Damageable")]
    [SerializeField] private ActorCanvas ActorUI;
    private int CurrentHealth;
    public int MaxHealth = 100;

    private void AwakeDamagable()
    {
        CurrentHealth = MaxHealth;
    }
    private void StartDamagable()
    {
        ActorUI.UpdateHealth(CurrentHealth, MaxHealth);
    }

    float IDamagable.CurrentHealth => CurrentHealth;
    float IDamagable.MaxHealth => MaxHealth;
    void IDamagable.Damage(in int dmg)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - dmg, 0, MaxHealth);
        if (CurrentHealth <= 0) { Despawn(); }
        else ActorUI.UpdateHealth(CurrentHealth, MaxHealth);
    }

    void IDamagable.Heal(in int dmg)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + dmg, 0, MaxHealth);
        ActorUI.UpdateHealth(CurrentHealth, MaxHealth);
    }

    protected override void CancelActions()
    {
        PerformingAction = false;
        BufferedAction = null;
        foreach (var item in CancelTokens)
        {
            item.ToCancel.Invoke();
        }
        CurrentAction?.CancelAction();
        //BasicAttack.CancelAction();
        //StepUp.CancelAction();
        //StepDown.CancelAction();
        //StepLeft.CancelAction();
        //StepRight.CancelAction();
    }
    #endregion
}
