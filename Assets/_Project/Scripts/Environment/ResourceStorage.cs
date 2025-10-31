using System;
using System.Collections.Generic;
using UnityEngine;
namespace Resources
{
    public class ResourceStorage : MonoBehaviour
    {
        protected readonly Dictionary<Resource, StorageCell> storage = new();
        protected virtual void OnEnable()
        {
            storage.Clear();
            foreach (var resType in (Resource[])Enum.GetValues(typeof(Resource)))
            {
                storage.Add(resType, new(10));
            }
        }
    }
}