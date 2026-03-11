namespace EnemyAI
{
    /// <summary>
    /// Enemy is standing still, watching for the player.  Transition to chase when
    /// the player is detected.
    /// </summary>
    public class IdleState : BaseEnemyState
    {
        public IdleState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public override void Enter()
    {
        controller.StopMovement();
    }

    public override void Tick()
    {
        if (controller.PlayerInDetectionRange)
            stateMachine.ChangeState(controller.ChaseState);
    }
}
} // namespace EnemyAI