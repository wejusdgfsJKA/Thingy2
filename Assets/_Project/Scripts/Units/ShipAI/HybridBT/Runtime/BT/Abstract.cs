using System;
using System.Collections.Generic;
using UnityEngine;

namespace HybridBT
{
    public enum NodeState : byte
    {
        SUCCESS,
        RUNNING,
        FAILURE
    }
    public abstract class BaseElement
    {
        public readonly string Name;
        public BaseElement(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Get the name of this object, indented a certain nr of times.
        /// </summary>
        /// <param name="indentation">How many times we should apply an indent.</param>
        /// <returns>The name of the object.</returns>
        public virtual string GetInfo(int indentation)
        {
            return new string('\t', indentation) + Name;
        }
    }
    public abstract class BaseElementData : ScriptableObject
    {
        [SerializeField] protected string myName;
        public string Name
        {
            get => myName.Length > 0 ? myName : name;
        }
    }
    public abstract class Node<T> : BaseElement
    {
        /// <summary>
        /// HasTarget wrapper. Stores the last result and a name, together with the actual function.
        /// </summary>
        protected class Condition : BaseElement
        {
            readonly Func<Context<T>, bool> function;
            public bool Result { get; protected set; }
            public Condition(string name, Func<Context<T>, bool> function) : base(name)
            {
                this.function = function;
                Result = false;
            }
            public bool Evaluate(Context<T> context)
            {
                Result = function(context);
                return Result;
            }
        }
        protected NodeState state = NodeState.FAILURE;
        /// <summary>
        /// Current state of the node. Initially set to FAILURE.
        /// </summary>
        public NodeState State { get => state; }
        /// <summary>
        /// If the new value is RUNNING, will connect Abort to parent's Abort.
        /// Otherwise if the previous state was RUNNING, will disconnect Abort to parent's Abort and 
        /// invoke onExit.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="context"></param>
        public void SetState(NodeState newValue, Context<T> context)
        {
            if (state != newValue)
            {
                if (newValue == NodeState.RUNNING)
                {
                    if (Parent != null) Parent.Abort += Abort;
                }
                else if (state == NodeState.RUNNING)
                {
                    if (Parent != null) Parent.Abort -= Abort;
                    onExit?.Invoke(context);
                }
                state = newValue;
            }
        }
        /// <summary>
        /// Gets invoked when the node was previously RUNNING and either has blocking decorators or its parent aborted.
        /// </summary>
        public Action<Context<T>> Abort { get; set; }
        protected Node<T> parent;
        public Node<T> Parent
        {
            get => parent;
            set
            {
                parent ??= value;
            }
        }
        /// <summary>
        /// Fires when the node exits RUNNING status.
        /// </summary>
        protected Action<Context<T>> onExit;
        /// <summary>
        /// Fires when the node is evaluated and not RUNNING.
        /// </summary>
        protected Action<Context<T>> onEnter;
        protected readonly List<Condition> conditions = new();
        protected readonly List<Action<Context<T>>> services = new();
        public Node(string name, Action<Context<T>> onEnter, Action<Context<T>> onExit) : base(name)
        {
            this.onEnter = onEnter;
            this.onExit = onExit;
            Abort += (ctx) => SetState(NodeState.FAILURE, ctx);
        }
        /// <summary>
        /// Evaluates all conditions. If one fails, invokes Abort and stops execution.
        /// Otherwise will execute all services, invoke onEnter if the previous state of the node was not
        /// RUNNING, and then will fire Execute.
        /// </summary>
        /// <param name="context">The context of the agent.</param>
        public virtual void Evaluate(Context<T> context)
        {
            for (var i = 0; i < conditions.Count; i++)
            {
                if (!conditions[i].Evaluate(context))
                {
                    if (State == NodeState.RUNNING) Abort(context);
                    return;
                }
            }
            for (var i = 0; i < services.Count; i++) services[i](context);
            if (State != NodeState.RUNNING)
            {
                onEnter?.Invoke(context);
                SetState(NodeState.RUNNING, context);
            }
            Execute(context);
        }
        /// <summary>
        /// Contains the action the node should execute if possible.
        /// </summary>
        /// <param name="context">The context of the agent.</param>
        protected abstract void Execute(Context<T> context);
        public void AddService(Action<Context<T>> service)
        {
            if (service == null) throw new ArgumentNullException($"{this} was passed a null service!");
            services.Add(service);
        }
        public void AddCondition(ConditionData<T> data) => conditions.Add(new(data.Name, data.Function));
        /// <summary>
        /// Get this node's name, state, and status of conditions.
        /// </summary>
        /// <param name="indentation">How many times indentation should be applied.</param>
        /// <returns>A string containing info about the node.</returns>
        public override string GetInfo(int indentation)
        {
            var s = base.GetInfo(indentation) + $"[{state}]";
            if (conditions.Count > 0)
            {
                s += "[";
                foreach (var condition in conditions)
                {
                    s += $"{condition.Name} {condition.Result};";
                }
                s += "]";
            }
            return s;
        }
    }
    public abstract class NodeData<T> : BaseElementData
    {
        protected virtual Action<Context<T>> onEnter { get => null; }
        protected virtual Action<Context<T>> onExit { get => null; }
        public List<ConditionData<T>> Conditions = new();
        public List<ServiceData<T>> Services = new();
        protected abstract Node<T> GetNode(Context<T> context);
        public virtual Node<T> ObtainNode(Context<T> context)
        {
            var node = GetNode(context);
            foreach (var item in Conditions) node.AddCondition(item);
            foreach (var item in Services) node.AddService(item.Action);
            return node;
        }
    }
    public abstract class ConditionData<DataKey> : BaseElementData
    {
        public abstract Func<Context<DataKey>, bool> Function { get; }
    }
    public abstract class ServiceData<DataKey> : BaseElementData
    {
        public abstract Action<Context<DataKey>> Action { get; }
    }
}