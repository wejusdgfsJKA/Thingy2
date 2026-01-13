using Player.UI;
using TMPro;
using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(ObjectDisplay))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerShip : Unit
    {
        [SerializeField] TextMeshProUGUI signatureText;
        Rigidbody rb;
        protected ObjectDisplay display;
        public override Unit CurrentTarget => display.CurrentTarget;
        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
            display = GetComponent<ObjectDisplay>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.Player = this;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            GameManager.Player = null;
        }
        protected override void OnTarget(Unit target, DetectionState detectionState = DetectionState.Identified)
        {
            switch (detectionState)
            {
                case DetectionState.Identified:
                    display.AddIdentified(target);
                    break;
                case DetectionState.Tracked:
                    display.AddTracked(target);
                    break;
            }
        }
        protected override float RecalculateSignature()
        {
            var s = base.RecalculateSignature();
            signatureText.text = $"Signature: {(int)s}";
            return s;
        }
    }
}