using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmteqLabs.Video
{
    public class VideoPlayerControls : MonoBehaviour
    {
        [SerializeField] private SrtVideoPlayer _srtVideoPlayer;
        [SerializeField] private Slider _scrubber;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private TMP_Text _currentTimeText;
        [SerializeField] private TMP_Text _totalTimeText;

        private float _currentPlayheadPosition;

        void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlay);
            _pauseButton.onClick.AddListener(OnPause);
            _scrubber.onValueChanged.AddListener(OnScrubberValueChanged);
        }

        void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlay);
            _pauseButton.onClick.RemoveListener(OnPause);
            _scrubber.onValueChanged.RemoveListener(OnScrubberValueChanged);
        }

        internal void OnPlay()
        {
            _playButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(true);
            _srtVideoPlayer.Play();
        }

        internal void OnPause()
        {
            _playButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(false);
            _srtVideoPlayer.Pause();
        }

        internal void Init(double clipLength)
        {
            SetTotalTimeUI(clipLength);
            SetScrubberValues(clipLength);
        }

        internal void SetPlayheadPosition(double value)
        {
            _currentPlayheadPosition = (float)value;
            _scrubber.value = _currentPlayheadPosition;
            SetCurrentTimeUI(value);
        }

        internal void SetCurrentTimeUI(double time)
        {
            string minutes = Mathf.Floor((float)time / 60).ToString("00");
            string seconds = (time % 60).ToString("00");

            _currentTimeText.text = string.Format("{0}:{1}", minutes, seconds);
        }

        private void SetTotalTimeUI(double clipLength)
        {
            string minutes = Mathf.Floor((float)clipLength / 60).ToString("00");
            string seconds = (clipLength % 60).ToString("00");

            _totalTimeText.text = string.Format("{0}:{1}", minutes, seconds);
        }

        private void OnScrubberValueChanged(float value)
        {
            if (_currentPlayheadPosition != value)
            {
                _srtVideoPlayer.GoToSeconds(value);
            }
        }

        private void SetScrubberValues(double clipLength)
        {
            _scrubber.minValue = 0;
            _scrubber.maxValue = (float)clipLength;
            _scrubber.value = 0;
        }
    }
}