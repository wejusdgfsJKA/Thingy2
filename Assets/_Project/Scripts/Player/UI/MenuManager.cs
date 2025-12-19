using System.Collections.Generic;
using UnityEngine;
namespace Player.UI
{
    public class MenuManager : MonoBehaviour
    {
        protected Stack<GameObject> windows = new();
        public void OpenWindow(GameObject window)
        {
            windows.Push(window);
            window.SetActive(true);
        }
        public void CloseWindow()
        {
            if (windows.Count == 0) return;
            var window = windows.Pop();
            window.SetActive(false);
        }
        public void CloseWindow(GameObject window)
        {
            if (windows.Count == 0) return;
            while (windows.Count > 0)
            {
                var currentWindow = windows.Pop();
                currentWindow.SetActive(false);
                if (currentWindow == window) break;
            }
        }
        public void CloseAllWindows()
        {
            while (windows.Count > 0)
            {
                var window = windows.Pop();
                window.SetActive(false);
            }
        }
    }
}