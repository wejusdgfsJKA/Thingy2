using UnityEngine;

namespace HybridBT
{
    /// <summary>
    /// Base behaviour tree class. Contains a blackboard and root node.
    /// </summary>
    /// <typeparam name="T">Key data type.</typeparam>
    public abstract class BT<T> : MonoBehaviour
    {
        protected Context<T> blackboard;
        [SerializeField] protected NodeData<T> rootData;
        public Node<T> Root { get; protected set; }
        protected virtual void Awake()
        {
            blackboard = new(transform.GetComponent<Ship>());
            SetupBlackboard();
            SetupTree();
        }
        protected abstract void SetupBlackboard();
        /// <summary>
        /// In the base version, sets up the root node from the root node data.
        /// </summary>
        protected virtual void SetupTree()
        {
            Root = rootData.ObtainNode(blackboard);
        }
        public void Tick()
        {
            Root?.Evaluate(blackboard);
        }
        public void SetValue(T key, object value) => blackboard.SetData(key, value);
    }
}