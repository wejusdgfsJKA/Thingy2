using UnityEditor;
using UnityEngine;
namespace HybridBT
{
    /// <summary>
    /// Editor base class. If a concrete type is declared, the coresponding BT will show debug info in the inspector.
    /// </summary>
    /// <typeparam name="T">Key type.</typeparam>
    public class BTEditor<T> : Editor
    {
        void OnEnable()
        {
            RequiresConstantRepaint();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Draw the default inspector

            var bt = (BT<T>)target;

            if (Application.isPlaying && bt.Root != null)
            {
                GUIStyle leftAlignedStyle = new GUIStyle(GUI.skin.label);
                leftAlignedStyle.alignment = TextAnchor.UpperLeft; // top-left alignment
                leftAlignedStyle.wordWrap = true;

                string info = bt.Root.GetInfo(0);
                float width = EditorGUIUtility.currentViewWidth - 100;
                float height = leftAlignedStyle.CalcHeight(new GUIContent(info), width);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(info, leftAlignedStyle, GUILayout.Width(width), GUILayout.Height(height));
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
