#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EmteqLabs
{
    public class DevelopmentMenu : MonoBehaviour
    {
        [MenuItem("Emteq/Development Options")]
        public static void ShowWindow()
        {
            DevelopmentOptionsWindow window = (DevelopmentOptionsWindow) EditorWindow.GetWindowWithRect(
                typeof(DevelopmentOptionsWindow), new Rect(0, 0, 260, 200), true, "Emteq Development Options");
        }
    }
}
#endif