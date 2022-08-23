using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmteqLabs
{
    public class DemoMenu : MonoBehaviour
    {
        void Start()
        {
            // Sync framerate to monitors refresh rate
            QualitySettings.vSyncCount = 1;
        }

        public void GoToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void ClickTest()
        {}
        
    }
}
