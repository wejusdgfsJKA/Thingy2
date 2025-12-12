using System;
using System.Collections.Generic;
using System.Linq;

namespace HybridBT
{
    public class UtilitySelector<T> : Composite<T>
    {
        protected int prevChild = -1;
        protected List<UtilityWrapper<T>> children = new();
        public UtilitySelector(string name, Action<Context<T>> onEnter = null, Action onExit = null) : base(name, onEnter, onExit)
        {
            onEnter += (_) => prevChild = -1;
        }
        /// <summary>
        /// Only accepts UtilityWrapper.
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="ArgumentNullException">Thrown if the method was passed a null child.</exception>
        /// <exception cref="ArgumentException">Thrown if the child could not be cast to a UtilityWrapper.</exception>
        public override void AddChild(Node<T> child)
        {
            if (child == null) throw new ArgumentNullException($"{this} was passed a null child!");
            if (child is not UtilityWrapper<T> utilityWrapper) throw new ArgumentException($"{this} received non-UtilityWrapper child {child}.");
            utilityWrapper.Index = children.Count;
            utilityWrapper.Parent = this;
            children.Add(utilityWrapper);
        }
        /// <summary>
        /// Orders children by their utility value, and then executes in order until it 
        /// finds a child which does not fail. Will abort the previous running action if 
        /// a different one is chosen.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(Context<T> context)
        {
            var sortedChildren = children.OrderBy(x => -x.GetScore(context));
            int count = -1;
            foreach (var child in sortedChildren)
            {
                count++;
                child.Evaluate(context);
                if (child.State == NodeState.FAILURE)
                {
                    if (count == children.Count - 1)
                    {
                        State = NodeState.FAILURE;
                        return;
                    }
                    State = NodeState.RUNNING;
                    continue;
                }
                State = child.State;
                {
                    if (prevChild != -1 && prevChild != child.Index) children[prevChild].Abort();
                    prevChild = child.Index;
                }
                return;
            }
        }
        /// <summary>
        /// Same as the one for regular composites.
        /// </summary>
        /// <param name="indentation"></param>
        /// <returns></returns>
        public override string GetInfo(int indentation)
        {
            var s = base.GetInfo(indentation);
            for (int i = 0; i < children.Count; i++)
            {
                s += "\n" + children[i].GetInfo(indentation + 1);
            }
            return s;
        }
    }
    public class UtilitySelectorData<T> : NodeData<T>
    {
        public List<UtilityWrapperData<T>> Children = new();
        protected override Node<T> GetNode(Context<T> context)
        {
            return new UtilitySelector<T>(Name, onEnter, onExit);
        }
        public override Node<T> ObtainNode(Context<T> context)
        {
            var node = (UtilitySelector<T>)GetNode(context);
            foreach (var item in Children) node.AddChild(item.ObtainNode(context));
            return node;
        }
    }
}