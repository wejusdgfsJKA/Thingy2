using TMPro;
using UnityEngine;
namespace Player
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI hullText, shieldText;
        public void SetHullPercentage(float percentage)
        {
            hullText.text = $"{(int)(percentage * 100)}%";
        }
        public void SetShieldPercentage(float percentage)
        {
            shieldText.text = $"{(int)(percentage * 100)}%";
        }
    }
}