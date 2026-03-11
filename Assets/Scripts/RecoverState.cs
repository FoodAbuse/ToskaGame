using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Brief pause after an attack before the enemy can make another decision.
    /// </summary>
    public class RecoverState : BaseEnemyState
    {
        private float timer;

    public RecoverState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public override void Enter()
    {
        timer = controller.LastAttackRecovery;
        controller.StopMovement();
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        if (controller.PlayerInAttackRange && controller.PlayerInDetectionRange)
            stateMachine.ChangeState(controller.AttackState);
        else if (controller.PlayerInDetectionRange)
            stateMachine.ChangeState(controller.ChaseState);
        else
            stateMachine.ChangeState(controller.IdleState);
    }
}
} // namespace EnemyAI