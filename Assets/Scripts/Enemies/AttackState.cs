using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Performs a single attack.  The state handles the wind‑up and active phases
    /// and then hands control off to <see cref="RecoverState"/>.
    /// </summary>
    public class AttackState : BaseEnemyState
    {
    private AttackData attack;
    private float timer;
    private enum Phase { Windup, Active }
    private Phase phase;

    public AttackState(EnemyController controller, EnemyStateMachine sm) : base(controller, sm) { }

    public override void Enter()
    {
        attack = controller.GetNextAttack();
        if (attack == null)
        {
            stateMachine.ChangeState(controller.ChaseState);
            return;
        }

        if (Time.time < controller.NextAttackTime)
        {
            // still on cooldown, back to chasing
            stateMachine.ChangeState(controller.ChaseState);
            return;
        }

        controller.NextAttackTime = Time.time + attack.cooldown;

        phase = Phase.Windup;
        timer = attack.windUpTime;
        controller.StopMovement();

        if (controller.player != null)
            controller.transform.LookAt(controller.player.position, Vector3.up);
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;

        if (phase == Phase.Windup)
        {
            phase = Phase.Active;
            timer = attack.activeTime;
            controller.OnAttackHit(attack);
        }
        else
        {
            // we finished the active window
            controller.LastAttackRecovery = attack.recoveryTime;
            stateMachine.ChangeState(controller.RecoverState);
        }
    }
}
} // namespace EnemyAI