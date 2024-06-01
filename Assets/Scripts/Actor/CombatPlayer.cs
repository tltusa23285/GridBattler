using GBGame.Actors.ActorTypes;
using UnityEngine;

namespace GBGame
{
    public class CombatPlayer
    {
        private readonly PlayerActor Actor;
        private CombatManager Com => Actor.Com;
        public CombatPlayer(PlayerActor actor)
        {
            Actor = actor;
        }

        public void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.W)) Actor.PerformMovement(PlayerActor.MOVE_DIR.Up);
            if (Input.GetKeyDown(KeyCode.S)) Actor.PerformMovement(PlayerActor.MOVE_DIR.Down);
            if (Input.GetKeyDown(KeyCode.A)) Actor.PerformMovement(PlayerActor.MOVE_DIR.Left);
            if (Input.GetKeyDown(KeyCode.D)) Actor.PerformMovement(PlayerActor.MOVE_DIR.Right);

            if (Input.GetKeyDown(KeyCode.J)) Actor.PerformBasicAttack();
            if (Input.GetKeyDown(KeyCode.K)) Actor.PerformSelectedAttack();
        }
    } 
}
