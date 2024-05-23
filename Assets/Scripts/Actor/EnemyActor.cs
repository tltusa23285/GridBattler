using UnityEngine;

public class EnemyActor : Actor, IDamagable
{
    [SerializeField] private ActorCanvas ActorUI;

    public EnemyActorController Controller { get; private set; }

    private void Awake()
    {
        AwakeDamagable();
    }
    void Start()
    {
        StartDamagable();
    }

    private ActorAction Attack;

    public void SetController(EnemyActorController con)
    {
        Controller = con;
        Controller.Setup(this);
    }
    protected override void OnSpawn()
    {
        Attack = new ShootAction();
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
        Controller.CancelActions();
    }
    #endregion
}
