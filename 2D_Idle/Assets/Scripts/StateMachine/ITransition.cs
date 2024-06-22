namespace Litkey.AI 
{
    public interface ITransition {
        IState To { get; }
        IPredicate Condition { get; }
    }
}