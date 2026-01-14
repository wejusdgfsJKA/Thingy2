using HybridBT;
using UnityEditor;
namespace Sample
{
    [CustomEditor(typeof(TestBT))]
    public class TestBTEditor : BTEditor<TestBTKeys>
    {
    }
}