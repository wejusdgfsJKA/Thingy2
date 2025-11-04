using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Resources
{
    public class ResourceExtractor : ResourceStorage
    {
        #region Fields
        [SerializeField] protected float extractionRange = 5;
        protected Dictionary<Resource, int> extractionAmounts = new() { { Resource.Metal, 1 },
        { Resource.Fuel,1} };
        [SerializeField] protected float extractionCooldown = 1;
        [field: SerializeField] public bool Extracting { get; protected set; }
        #region Implementation
        LayerMask resourceMask = 1 << 6;
        protected Coroutine coroutine;
        int count;
        static readonly Collider[] nonAllocBuffer = new Collider[5];
        #endregion
        #endregion
        #region Setup
        protected override void OnEnable()
        {
            base.OnEnable();
            Extracting = false;
        }
        protected void OnDisable()
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }
        #endregion
        private void Update()
        {
            if (!Extracting && Physics.CheckSphere(transform.position, extractionRange, resourceMask))
            {
                coroutine = StartCoroutine(ExtractionCoroutine());
            }
        }
        protected IEnumerator ExtractionCoroutine()
        {
            Extracting = true;
            while (Extracting)
            {
                yield return new WaitForSeconds(extractionCooldown);
                count = Physics.OverlapSphereNonAlloc(transform.position, extractionRange,
                    nonAllocBuffer, resourceMask);
                for (int i = 0; i < count; i++)
                {
                    //var c = nonAllocBuffer[i];
                    //if (c == null || !c.gameObject.activeSelf) continue;
                    //var container = ComponentManager<Trackable>.Get(c.transform) as ResourceContainer;
                    //if (container == null || container.Empty) continue;
                    //Extracting = true;
                    //var list = container.Extract(extractionAmounts);
                    //for (int j = 0; j < list.Count; j++)
                    //{
                    //    storage[list[j].resource].Amount += list[j].amount;
                    //    var prevExt = extractionAmounts[list[j].resource];
                    //    extractionAmounts[list[j].resource] = Mathf.Min(prevExt, storage[list[j].resource].CapacityLeft);
                    //}
                }
            }
        }
    }
}