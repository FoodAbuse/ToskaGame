using UnityEngine;

namespace EnemyAI
{
    /// <summary>
    /// Abstract base class for all states.  Each concrete state gets a reference to
    /// the owning <see cref="EnemyController"/> and the shared state machine.
    /// </summary>
    public abstract class BaseEnemyState
    {
        protected EnemyController controller;
        protected EnemyStateMachine stateMachine;

    public BaseEnemyState(EnemyController controller, EnemyStateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Tick() { }
    public virtual void OnTriggerEnter(Collider other) { }
}
} // namespace EnemyAI