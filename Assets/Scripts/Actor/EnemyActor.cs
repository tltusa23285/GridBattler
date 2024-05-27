using UnityEngine;

public class EnemyActor : Actor, IDamagable
{
    public string EnemyController;
    [SerializeField] private ActorCanvas ActorUI;

    public EnemyActorController Controller { get; private set; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    void Start()
    {
        ActorUI.UpdateHealth(CurrentHealth, MaxHealth);
    }

    public void SetController(EnemyActorController con)
    {
        Controller = con;
        Controller.Setup(this);
    }
    protected override bool OnSpawn()
    {
        if (!Com.EnemyLibrary.GetItem(EnemyController, out var result)) return false;
        SetController(result.Object);
        return true;
    }

    #region IDamageable
    [Header("Damageable")]
    private int CurrentHealth;
    public int MaxHealth = 100;

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
