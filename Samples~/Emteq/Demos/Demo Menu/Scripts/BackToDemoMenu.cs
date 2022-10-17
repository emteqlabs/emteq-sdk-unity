using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmteqLabs
{
    public class BackToDemoMenu : MonoBehaviour
    {

        [SerializeField]
        private string _mainMenuName = "DemoMenu";
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas.worldCamera == null && Camera.main != null)
            {
                canvas.worldCamera = Camera.main;
            }
        }

        public void BackToMenuScene()
        {
            SceneManager.LoadScene(_mainMenuName);
        }
    }
}
