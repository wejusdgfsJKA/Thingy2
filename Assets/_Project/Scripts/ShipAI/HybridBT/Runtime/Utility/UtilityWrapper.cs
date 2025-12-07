using System;

namespace HybridBT
{
    /// <summary>
    /// Contains one Node and a Consideration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UtilityWrapper<T> : Node<T>
    {
        public int Index { get; set; }
        protected readonly Consideration<T> consideration;
        protected readonly Node<T> child;
        public float Score { get; protected set; }
        public UtilityWrapper(string name, Node<T> child, Consideration<T> consideration,
            Action<Context<T>> onEnter, Action onExit) : base(name, onEnter, onExit)
        {
            this.child = child;
            child.Parent = this;
            this.consideration = consideration;
        }
        public float GetScore(Context<T> context)
        {
            //Score exists only for debugging
            Score = consideration.Evaluate(context);
            return Score;
        }
        protected override void Execute(Context<T> context)
        {
            child.Evaluate(context);
            State = child.State;
        }
        /// <summary>
        /// Same as the one for node, but also returns the utility score and the child on a separate line below.
        /// </summary>
        /// <param name="indentation"></param>
        /// <returns></returns>
        public override string GetInfo(int indentation)
        {
            return base.GetInfo(indentation) + $"(Utility: {Score})\n{child.GetInfo(indentation + 1)}";
        }
    }
    public class UtilityWrapperData<T> : NodeData<T>
    {
        public Consideration<T> Consideration;
        public NodeData<T> Child;
        protected override Node<T> GetNode(Context<T> context)
        {
            return new UtilityWrapper<T>(Name, Child.ObtainNode(context), Consideration, onEnter, onExit);
        }
    }
}