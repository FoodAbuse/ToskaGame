using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Temporary stunned state.  After the timer expires the enemy transitions to
    /// <see cref="RecoverState"/>.
    /// </summary>
    public class StaggerState : BaseEnemyState
    {
        private float timer;

    public StaggerState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public void SetDuration(float duration)
    {
        timer = duration;
    }

    public override void Enter()
    {
        controller.StopMovement();
        // could fire stagger animation here
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            stateMachine.ChangeState(controller.RecoverState);
    }
}
} // namespace EnemyAI