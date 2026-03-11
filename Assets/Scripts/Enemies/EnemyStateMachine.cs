/// <summary>
/// Simple wrapper that keeps track of the current <see cref="BaseEnemyState"/>
/// and handles the transition plumbing.
/// </summary>
namespace EnemyAI
{
    public class EnemyStateMachine
    {
        public BaseEnemyState CurrentState { get; private set; }

    public void Initialize(BaseEnemyState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(BaseEnemyState newState)
    {
        if (CurrentState == newState) return;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
} // namespace EnemyAI