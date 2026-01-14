using HybridBT;
using TMPro;
using UnityEngine;
namespace Sample
{
    public class TestBT : BT<TestBTKeys>
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] Transform goober;
        [SerializeField] float meleeRange = 1, hipfireRange = 4;
        protected override void SetupBlackboard()
        {
            SetValue(TestBTKeys.Goober, goober);
            SetValue(TestBTKeys.Self, transform);
            SetValue(TestBTKeys.MeleeRange, meleeRange);
            SetValue(TestBTKeys.HipfireRange, hipfireRange);
        }
        private void Update()
        {
            if (text) text.text = Root.GetInfo(0);
        }
    }
}