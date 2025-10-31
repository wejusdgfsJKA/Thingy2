using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Resources
{
    public class ResourceExtractor : ResourceStorage
    {
        #region Fields
        [SerializeField] protected float range = 5;
        protected Dictionary<Resource, int> extraction = new() { { Resource.Metal, 1 },
        { Resource.Fuel,1} };
        readonly HashSet<Collider> colliders = new();
        [SerializeField] protected float extractionCooldown = 1;
        protected Coroutine coroutine;
        public bool Extracting { get; protected set; }
        SphereCollider coll;
        #endregion        
        private void Awake()
        {
            coll = GetComponent<SphereCollider>();
            coll.isTrigger = true;
            coll.radius = range;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Extracting = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            colliders.Add(other);
            if (!Extracting)
            {
                coroutine = StartCoroutine(ExtractionCoroutine());
            }
        }
        private void OnTriggerExit(Collider other)
        {
            colliders.Remove(other);
        }
        protected void OnDisable()
        {
            colliders.Clear();
            if (coroutine != null) StopCoroutine(coroutine);
        }
        protected IEnumerator ExtractionCoroutine()
        {
            Extracting = true;
            while (Extracting)
            {
                yield return new WaitForSeconds(extractionCooldown);
                if (colliders.Count > 0)
                {
                    foreach (var c in colliders)
                    {
                        if (c == null) continue;
                        var container = ComponentManager<Trackable>.Get(c.transform) as ResourceContainer;
                        if (container == null || container.Empty) continue;
                        Extracting = true;
                        var list = container.Extract(extraction);
                        for (int j = 0; j < list.Count; j++)
                        {
                            storage[list[j].resource].Amount += list[j].amount;
                            var prevExt = extraction[list[j].resource];
                            extraction[list[j].resource] = Mathf.Min(prevExt, storage[list[j].resource].CapacityLeft);
                        }
                    }
                }
            }
        }
    }
}