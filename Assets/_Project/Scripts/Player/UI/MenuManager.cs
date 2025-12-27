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
        public void CloseAllWindows()
        {
            while (windows.Count > 0)
            {
                CloseWindow();
            }
        }
    }
}