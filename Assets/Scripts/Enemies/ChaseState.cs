namespace EnemyAI
{
    /// <summary>
    /// Enemy moves toward the player.  If the player slips out of detection range
    /// the enemy returns to idle; if the player steps into attack range the state
    /// switches to <see cref="AttackState"/>.
    /// </summary>
    public class ChaseState : BaseEnemyState
    {
        public ChaseState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public override void Tick()
    {
        if (!controller.PlayerInDetectionRange)
        {
            stateMachine.ChangeState(controller.IdleState);
            return;
        }

        if (controller.PlayerInAttackRange)
        {
            stateMachine.ChangeState(controller.AttackState);
            return;
        }

        controller.MoveTowardsPlayer();
    }
}
} // namespace EnemyAI