using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
namespace Player.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour
    {
        [SerializeField] GameObject initialSelectedButton;
        [SerializeField] CanvasGroup canvasGroup;
        public bool Active { get; private set; }
        public void Activate()
        {
            Assert.IsNotNull(initialSelectedButton);
            canvasGroup.interactable = true;
            Active = true;
            gameObject.SetActive(true);
            var current = EventSystem.current;
            if (current) current.SetSelectedGameObject(initialSelectedButton);
        }
        public void Close()
        {
            gameObject.SetActive(false);
            Deactivate();
        }
        public void Deactivate()
        {
            canvasGroup.interactable = false;
            Active = false;
            var current = EventSystem.current;
            if (current) current.SetSelectedGameObject(null);
        }
    }
}