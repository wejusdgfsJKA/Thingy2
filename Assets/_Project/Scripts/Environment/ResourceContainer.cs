using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Environment
{
    public class ResourceContainer : MonoBehaviour
    {
        [SerializeField] protected ResourceData data;
        public readonly Dictionary<Resource, int> Resources = new();
        protected void OnEnable()
        {
            RegenerateResources();
        }
        protected void RegenerateResources()
        {
            if (data != null)
            {
                if (Resources.Count > 0) Resources.Clear();
                for (int i = 0; i < data.Resources.Count; i++)
                {
                    var res = data.Resources[i];
                    float p = Random.Range(0.0f, 1.0f);
                    if (p < res.Probability)
                    {
                        var value = Random.Range(res.Min, res.Max);
                        Resources[res.Resource] = value;
                    }
                }
            }
        }
        public int Extract(Resource resource, int amount)
        {
            if (Resources.ContainsKey(resource))
            {
                int value = Mathf.Max(Resources[resource], amount);
                Resources[resource] -= value;
                if (Resources[resource] == 0) Resources.Remove(resource);
                if (Resources.Count == 0 && data.ExpirationTime >= 0) StartCoroutine(Deactivate());
                return value;
            }
            return 0;
        }
        protected IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(data.ExpirationTime);
            gameObject.SetActive(false);
        }
    }
}
