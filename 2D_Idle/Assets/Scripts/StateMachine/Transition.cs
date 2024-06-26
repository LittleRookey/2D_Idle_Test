namespace Litkey.AI
{
    public class Transition : ITransition {
        public IState To { get; }
        public IPredicate Condition { get; }
        public bool HasExitTime { get; }
        public float ExitTime { get; set; } = 0;  // Default to zero for no exit time

        public Transition(IState to, IPredicate condition, bool hasExitTime = false, float exitTime = 0.0f) {
            To = to;
            Condition = condition;
            HasExitTime = hasExitTime;
            ExitTime = exitTime;
        }
    }
}