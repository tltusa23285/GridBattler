using GBGame.Interfaces;

namespace GBGame.Actors.Actions
{
    public class WSlashAction : ActorAction
    {
        public int Dmg = 30;
        public uint Initial = 30;
        public override void Execute(out uint ticksToResolve)
        {
            ticksToResolve = 15;
            AddNext(() => Caller.GraphicControl.TryPlayAnim("ShootAction", 5));
            AddNext(() => Caller.TryMoveToCell(Caller.Position.x + 2, Caller.Position.y, 5));
            AddFuture(5, () => PerformAttack());
            AddFuture(5, () => Caller.TryMoveToCell(Caller.Position.x - 2, Caller.Position.y, 5));
        }
            
        private void PerformAttack()
        {
            int x = Caller.Position.x;
            int y = Caller.Position.y + 1;

            switch (Caller.Facing)
            {
                case Actor.FACING.Right: x++; break;
                case Actor.FACING.Left: x--; break;
                default: throw new System.NotImplementedException();
            }

            for (int i = 0; i < 3; i++)
            {
                if (!(Com.Grid.InBounds(x, y - i))) continue;
                if (Com.Grid.Occupancy.GetOccupantInCell(x, y - i, out Actor a))
                {
                    if (a is IDamagable)
                    {
                        ((IDamagable)a).Damage(Dmg);
                    }
                }
            }
        }
    }

}