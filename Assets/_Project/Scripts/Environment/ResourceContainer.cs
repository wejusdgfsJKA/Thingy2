using System.Collections.Generic;
using UnityEngine;
namespace Resources
{
    public class ResourceContainer : Trackable
    {
        [SerializeField] protected ResourceData data;
        public Dictionary<Resource, int> Resources { get; } = new();
        public bool Empty
        {
            get
            {
                return Resources.Count == 0;
            }
        }
        protected override void OnEnable()
        {
            RegenerateResources();
            base.OnEnable();
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
                    if (p <= res.Probability)
                    {
                        var value = Random.Range(res.Min, res.Max);
                        Resources[res.Resource] = value;
                    }
                }
            }
        }
        public List<(Resource resource, int amount)> Extract(
            Dictionary<Resource, int> demand)
        {
            List<(Resource, int)> result = new();
            foreach (var kvp in demand)
            {
                if (Empty) break;
                var res = kvp.Key;
                if (kvp.Value > 0 && Resources.ContainsKey(res))
                {
                    int amount = Mathf.Min(kvp.Value, Resources[res]);
                    Resources[res] -= amount;
                    result.Add((res, amount));
                    if (Resources[res] == 0) Resources.Remove(res);
                }
            }
            UpdateString?.Invoke(this);
            return result;
        }
        public override string ToString()
        {
            string s = "";
            foreach (var a in Resources)
            {
                for (int i = 0; i < a.Value; i++)
                {
                    s += $"<color={GlobalSettings.ResourceColors[a.Key]}>●</color>";
                }
            }
            return s;
        }
    }
}
