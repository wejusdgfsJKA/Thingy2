using UnityEditor;
namespace HybridBT.Template
{
    /// <summary>
    /// Without this, you won't get debug info for the hybrid BT in the inspector.
    /// </summary>
    [CustomEditor(typeof(TemplateBT))]
    public class TemplateEditor : BTEditor<BTKeys>
    {

    }
}