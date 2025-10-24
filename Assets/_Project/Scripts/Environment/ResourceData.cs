using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ResourceData")]
    public class ResourceData : ScriptableObject
    {
        public float ExpirationTime = 5;
        public List<ResourceValue> Resources = new();
    }
}