namespace Litkey.AI 
{
    public interface ITransition {
        IState To { get; }
        IPredicate Condition { get; }

        public bool HasExitTime { get; }
        public float ExitTime { get; set; }
    }
}