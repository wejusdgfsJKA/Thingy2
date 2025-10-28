using Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ResourceExtractor : MonoBehaviour
{
    protected class StorageCell
    {
        public readonly int MaxAmount;
        public int Amount { get; set; }
        public int CapacityLeft
        {
            get => MaxAmount - Amount;
        }
        public StorageCell(int maxAmount, int amount = 0)
        {
            MaxAmount = maxAmount;
            Amount = amount;
        }
    }
    [SerializeField] protected float range;
    LayerMask layerMask = 1 << 6;
    protected Dictionary<Resource, StorageCell> storage;
    protected Dictionary<Resource, int> extraction = new() { { Resource.Metal, 1 },
        { Resource.Fuel,1} };
    protected static Collider[] nonAllocBuffer = new Collider[5];
    protected int count;
    [SerializeField] protected float extractionCooldown = 1;
    protected Coroutine coroutine;
    public bool extracting;
    protected void OnEnable()
    {
        storage = new();
        foreach (var resType in (Resource[])Enum.GetValues(typeof(Resource)))
        {
            storage.Add(resType, new(10));
        }
    }
    protected void Update()
    {
        Extract();
    }
    protected void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }
    protected void Extract()
    {
        if (!extracting && Physics.CheckSphere(transform.position, range, layerMask))
        {
            coroutine = StartCoroutine(ExtractionCoroutine());
        }
    }
    protected IEnumerator ExtractionCoroutine()
    {
        extracting = true;
        while (extracting)
        {
            yield return new WaitForSeconds(extractionCooldown);
            count = Physics.OverlapSphereNonAlloc(transform.position, range,
                nonAllocBuffer, layerMask);
            extracting = false;
            for (int i = 0; i < count; i++)
            {
                var c = nonAllocBuffer[i];
                if (c == null) continue;
                var container = ComponentManager<Trackable>.Get(c.transform) as ResourceContainer;
                if (container == null || container.Empty) continue;
                extracting = true;
                var list = container.Extract(extraction);
                for (int j = 0; j < list.Count; j++)
                {
                    storage[list[j].resource].Amount += list[j].amount;
                    var prevExt = extraction[list[j].resource];
                    extraction[list[j].resource] = Mathf.Min(prevExt, storage[list[j].resource].CapacityLeft);
                }
            }
            if (extracting)
            {
                string s = "";
                foreach (var kvp in storage)
                {
                    s += $"{kvp.Key} {kvp.Value.Amount}/{kvp.Value.MaxAmount}\n";
                }
                Debug.Log(s);
            }
        }
    }
}
