using Player.UI;
using TMPro;
using UnityEngine;
namespace Player
{
    [UnityEngine.RequireComponent(typeof(ObjectDisplay))]
    public class PlayerShip : Unit
    {
        [SerializeField] TextMeshProUGUI signatureText;
        protected ObjectDisplay display;
        public override Unit CurrentTarget => display.CurrentTarget;
        protected override void Awake()
        {
            base.Awake();
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
    }
}