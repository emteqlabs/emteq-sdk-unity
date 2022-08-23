using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmteqLabs.Video
{
    public class VideoPlayerSubtitle : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        public void SetText(string text)
        {
            if (_text != null)
            {
                _text.text = text;
            }
        }
    }
}