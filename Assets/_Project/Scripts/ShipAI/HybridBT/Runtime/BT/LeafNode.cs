using System;

namespace HybridBT
{

    public class LeafNode<T> : Node<T>
    {
        protected Func<Context<T>, NodeState> onEvaluate;
        public LeafNode(string name, Func<Context<T>, NodeState> onEvaluate, Action<Context<T>> onEnter = null, Action onExit = null) : base(name, onEnter, onExit)
        {
            this.onEvaluate = onEvaluate;
        }
        protected override void Execute(Context<T> context)
        {
            State = onEvaluate(context);
        }
    }
    public abstract class LeafNodeData<T> : NodeData<T>
    {
        protected abstract Func<Context<T>, NodeState> onEvaluate { get; }
        protected override Node<T> GetNode(Context<T> context)
        {
            return new LeafNode<T>(Name, onEvaluate, onEnter, onExit);
        }
    }
}