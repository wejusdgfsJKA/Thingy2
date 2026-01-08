using System.Collections.Generic;
using UnityEngine;
namespace Player.UI
{
    public class MenuPDA : MonoBehaviour
    {
        protected Stack<Window> windows = new();
        public void OpenWindow(Window window)
        {
            if (window == null) return;
            if (windows.Count > 0)
            {
                windows.Peek().Deactivate();
            }
            windows.Push(window);
            window.Activate();
        }
        public virtual void CloseWindow()
        {
            if (windows.Count == 0) return;
            var window = windows.Pop();
            window.Close();
            if (windows.Count > 0)
            {
                var previousWindow = windows.Peek();
                previousWindow.Activate();
            }
        }
        public virtual void CloseAllWindows()
        {
            while (windows.Count > 0)
            {
                var window = windows.Pop();
                window.Close();
            }
        }
    }
}