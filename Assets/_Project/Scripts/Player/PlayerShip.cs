using EventBus;
using Player.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using Weapons;
namespace Player
{
    [RequireComponent(typeof(ObjectDisplay))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerShip : Unit
    {
        [SerializeField] TextMeshProUGUI signatureText, holdFireText;
        [SerializeField] TextMeshProUGUI missionOverText;
        Rigidbody rb;
        protected ObjectDisplay display;
        public override Unit CurrentTarget => display.CurrentTarget;
        protected override void Awake()
        {
            base.Awake();
            missionOverText.enabled = false;
            EventBus<AllEnemiesEliminated>.AddActions(0, null, Victory);
            rb = GetComponent<Rigidbody>();
            display = GetComponent<ObjectDisplay>();
        }
        public void ToggleHoldFire()
        {
            holdFireText.enabled = !holdFireText.enabled;
            foreach (var turret in Turrets)
            {
                var b = turret.Weapon as BeamWeapon;
                if (b != null) turret.ToggleHoldFire();
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            holdFireText.enabled = false;
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
        public void Victory()
        {
            EventBus<AllEnemiesEliminated>.RemoveActions(0, null, Victory);
            missionOverText.enabled = true;
            missionOverText.text = "Victory!";
            GameManager.EndMission();
            StartCoroutine(WaitForTime(3, () => GameManager.ChangeSceneOnMissionEnd()));
        }
        public void OnDeath()
        {
            missionOverText.enabled = true;
            missionOverText.text = "Systems failure";
            GameManager.Player = null;
            GameManager.EndMission();
            StartCoroutine(WaitForTime(3, () => GameManager.ChangeSceneOnMissionEnd()));
        }
        IEnumerator WaitForTime(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}