#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace EmteqLabs
{
    public class DevelopmentOptionsWindow : EditorWindow
    {
        public static string dabFilePath = "";
        public static string jsonFilePath = "";
        public static bool loopDataReplay = false;
        public static bool dabDataFileSelected = false;
        public static bool jsonDataFileSelected = false;
        
        private bool advSettingsEnabled;
        private string dabButtonText;
        private string jsonButtonText;
        private string advSettingsOptionLabel = "Advanced Settings";
        private string dabButtonLabel = "Mask Data file (.dab)";
        private string jsonButtonLabel = "Event Data file (.json)";
        private string dabDataFileSelectedText = ".dab file selected!";
        private string jsonDataFileSelectedText = ".json file selected!";
        private string dabDataFileNotSelectedText = "Select .dab file";
        private string jsonDataFileNotSelectedText = "Select .json file";
        private string loopText = "Loop Data Replay";
        
        void OnGUI()
        {
            advSettingsEnabled = EditorGUILayout.BeginToggleGroup(advSettingsOptionLabel, advSettingsEnabled);
            
            GUILayout.BeginHorizontal();
        
            GUILayout.Label(dabButtonLabel);

            if (GUILayout.Button(dabButtonText, GUILayout.Width(120)))
            {
                dabFilePath = EditorUtility.OpenFilePanel(dabDataFileNotSelectedText, "", "dab");
            }
            
            if (dabFilePath.Length != 0)
            {
                //todo: Wire up to dll
                var dabFileContent = File.ReadAllBytes(dabFilePath);
                dabDataFileSelected = true;
                dabButtonText = dabDataFileSelectedText;
            }
            else
            {
                dabButtonText = dabDataFileNotSelectedText;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(jsonButtonLabel);
            
            if (GUILayout.Button(jsonButtonText,GUILayout.Width(120)))
            {
                jsonFilePath = EditorUtility.OpenFilePanel(jsonDataFileNotSelectedText, "", "json");
            }
            
            if (jsonFilePath.Length != 0)
            {
                //todo: Wire up to dll
                var jsonFileContent = File.ReadAllBytes(jsonFilePath);
                jsonDataFileSelected = true;
                jsonButtonText = jsonDataFileSelectedText;
            }
            else
            {
                jsonButtonText = jsonDataFileNotSelectedText;
            }
            
            GUILayout.EndHorizontal();
            
            loopDataReplay = EditorGUILayout.Toggle(loopText, loopDataReplay);
            
            EditorGUILayout.EndToggleGroup();
        }
    }
}
#endif
