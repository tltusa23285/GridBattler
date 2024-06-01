namespace GBGame.Interfaces
{
    public interface IDamagable
    {
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public void Damage(in int dmg);
        public void Heal(in int dmg);
    } 
}
