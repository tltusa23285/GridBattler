using UnityEngine;

public class PlayerActor : Actor, IDamagable
{
    private ActorAction BasicAttack;

    private ActorAction StepUp;
    private ActorAction StepDown;
    private ActorAction StepLeft;
    private ActorAction StepRight;

    [SerializeField] private ActorCanvas ActorUI;

    private void Awake()
    {
        AwakeDamagable();
    }
    void Start()
    {
        StartDamagable();
    }

    private void Update()
    {
        if (PerformingAction) return;
        if (Input.GetKeyDown(KeyCode.W)) StepUp     .Execute(out _);
        if (Input.GetKeyDown(KeyCode.S)) StepDown   .Execute(out _);
        if (Input.GetKeyDown(KeyCode.A)) StepLeft   .Execute(out _);
        if (Input.GetKeyDown(KeyCode.D)) StepRight  .Execute(out _);

        if (Input.GetKeyDown(KeyCode.J)) BasicAttack.Execute(out _);
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


    #region IDamageable
    [Header("Damageable")]
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
        BasicAttack.CancelAction();
        StepUp.CancelAction();
        StepDown.CancelAction();
        StepLeft.CancelAction();
        StepRight.CancelAction();
    }
    #endregion
}
