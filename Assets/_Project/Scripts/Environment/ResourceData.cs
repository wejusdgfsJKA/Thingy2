using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(menuName = "ScriptableObjects/ResourceData")]
    public class ResourceData : ScriptableObject
    {
        public List<ResourceGen> Resources = new();
    }
}