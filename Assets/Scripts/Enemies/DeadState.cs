using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Final state, the enemy ceases all behaviour and is usually removed from the
    /// scene.
    /// </summary>
    public class DeadState : BaseEnemyState
    {
    public DeadState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public override void Enter()
    {
        controller.StopMovement();
        if (controller.agent != null)
            controller.agent.enabled = false;
        controller.OnDeath();
    }

    public override void Tick() { /* nothing */ }
}
} // namespace EnemyAI