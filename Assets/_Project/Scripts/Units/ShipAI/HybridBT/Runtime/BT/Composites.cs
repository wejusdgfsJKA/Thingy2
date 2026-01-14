using System;
using System.Collections.Generic;

namespace HybridBT
{
    public abstract class Composite<T> : Node<T>
    {
        public Composite(string name, Action<Context<T>> onEnter = null,
            Action<Context<T>> onExit = null) : base(name, onEnter, onExit) { }
        public abstract void AddChild(Node<T> child);
    }
    public abstract class RegularComposite<T> : Composite<T>
    {
        protected List<Node<T>> children = new();
        public RegularComposite(string name, Action<Context<T>> onEnter = null,
            Action<Context<T>> onExit = null) : base(name, onEnter, onExit)
        {

        }
        /// <summary>
        /// Add the child to a list.
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="ArgumentNullException">Thrown if the method was passed a null child.</exception>
        public override void AddChild(Node<T> child)
        {
            if (child == null) throw new ArgumentNullException($"{this} was passed a null child!");
            children.Add(child);
            child.Parent = this;
        }
        /// <summary>
        /// Return data about this composite. Same as the one for Node, but also returns the children 
        /// below, each on a separate line.
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
    public abstract class RegularCompositeData<T> : NodeData<T>
    {
        public List<NodeData<T>> Children;
        public override Node<T> ObtainNode(Context<T> context)
        {
            var node = (Composite<T>)base.ObtainNode(context);
            foreach (var item in Children) node.AddChild(item.ObtainNode(context));
            return node;
        }
    }
    public class Sequence<T> : RegularComposite<T>
    {
        protected int currentChild = 0;
        public Sequence(string name, Action<Context<T>> onEnter = null, Action<Context<T>> onExit = null) : base(name, onEnter, onExit)
        {
            onEnter += (_) => currentChild = 0;
        }
        /// <summary>
        /// Execute all children in sequence. Abort on child FAILURE.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(Context<T> context)
        {
            children[currentChild].Evaluate(context);
            switch (children[currentChild].State)
            {
                case NodeState.FAILURE:
                    Abort(context);
                    break;
                case NodeState.RUNNING:
                    SetState(NodeState.RUNNING, context);
                    break;
                case NodeState.SUCCESS:
                    var newState = currentChild == children.Count - 1 ? NodeState.SUCCESS : NodeState.RUNNING;
                    SetState(newState, context);
                    currentChild = Math.Min(currentChild + 1, children.Count - 1);
                    break;
            }
        }
    }
    public class SequenceData<T> : RegularCompositeData<T>
    {
        protected override Node<T> GetNode(Context<T> context)
        {
            return new Sequence<T>(Name, onEnter, onExit);
        }
    }
    public class Selector<T> : RegularComposite<T>
    {
        protected int prevChild = -1;
        public Selector(string name, Action<Context<T>> onEnter = null, Action<Context<T>> onExit = null) : base(name, onEnter, onExit)
        {
            onEnter += (_) => prevChild = -1;
        }
        /// <summary>
        /// Executes the first child which does not fail. If previously had a lower priority child, 
        /// it will Abort it.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(Context<T> context)
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Evaluate(context);
                if (children[i].State == NodeState.FAILURE)
                {
                    if (i == children.Count - 1)
                    {
                        state = NodeState.FAILURE;
                        return;
                    }
                    SetState(NodeState.RUNNING, context);
                    continue;
                }
                if (prevChild != -1 && i > prevChild) children[prevChild].Abort(context);
                prevChild = i;
                SetState(children[i].State, context);
                return;
            }
        }
    }
    public class SelectorData<T> : RegularCompositeData<T>
    {
        protected override Node<T> GetNode(Context<T> context)
        {
            return new Selector<T>(Name, onEnter, onExit);
        }
    }
    public class ParallelNode<T> : RegularComposite<T>
    {
        public ParallelNode(string name, Node<T> leftChild, Node<T> rightChild, Action<Context<T>> onEnter = null,
            Action<Context<T>> onExit = null) : base(name, onEnter, onExit)
        {
            AddChild(leftChild);
            AddChild(rightChild);
        }
        /// <summary>
        /// Executes the first child. If not FAILURE, will evaluate the second, otherwise if 
        /// the second is RUNNING it will Abort it.
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(Context<T> context)
        {
            children[0].Evaluate(context);
            SetState(children[0].State, context);
            if (State != NodeState.FAILURE) children[1].Evaluate(context);
            else if (children[1].State == NodeState.RUNNING) children[1].Abort(context);
        }
    }
    public class ParallelNodeData<T> : NodeData<T>
    {
        public NodeData<T> LeftChild, RightChild;
        protected override Node<T> GetNode(Context<T> context)
        {
            return new ParallelNode<T>(Name, LeftChild.ObtainNode(context), RightChild.ObtainNode(context), onEnter, onExit);
        }
    }
}