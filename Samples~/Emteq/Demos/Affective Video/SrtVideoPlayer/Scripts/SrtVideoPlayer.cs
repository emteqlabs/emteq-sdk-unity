using System;
using UnityEngine;
using UnityEngine.Video;

namespace EmteqLabs.Video
{
    public class SrtVideoPlayer : MonoBehaviour
    {
        // Serialised Properties
        [SerializeField]
        private bool _autoPlay = true;
        [SerializeField]
        private bool _showPlayerControl = true;
        [SerializeField]
        private bool _showSubtitles = true;
        [SerializeField]
        private VideoClip _videoClip;
        [SerializeField]
        private TextAsset _srtFile;
        
        // public Events
        public event Action<string> OnShowSubtitle;
        public event Action<string> OnHideSubtitle;
        public event Action<VideoClip> OnVideoClipFinished;
        
        // Private Properties
        private VideoPlayer _videoPlayer;
        private VideoPlayerControls _playerControls;
        private VideoPlayerSubtitle _subtitlePanel;
        private SRTParser _srtParser;
        private int _currentSubtitleIndex = 0;
        
        private void Awake()
        {
            // Unity VideoPlayer initialisation
            _videoPlayer = GetComponentInChildren<VideoPlayer>();
            if (_videoPlayer == null) return;
            if (_videoClip != null)
            {
                SetVideoClip(_videoClip);
            }

            // Subtitles Initialisation
            _subtitlePanel = GetComponentInChildren<VideoPlayerSubtitle>(true);
            if (_subtitlePanel == null) return;
            _subtitlePanel.gameObject.SetActive(_showSubtitles);
            _subtitlePanel.SetText("");
            if (_srtFile != null)
            {
                SetSubtitles(_srtFile);
            }
            
            // Player Controls Initialisation
            _playerControls = GetComponentInChildren<VideoPlayerControls>(true);
            if (_playerControls == null) return;
            _playerControls.gameObject.SetActive(_showPlayerControl);
            if (_showPlayerControl && _videoClip != null)
            {
                _playerControls.Init(_videoClip.length);
            }
            
            //TODO: test setting up videoclip and srt file dynamically (by code) instead of serialised property
        }

        private void OnEnable()
        {
            if (_videoPlayer)
            {
                _videoPlayer.loopPointReached += VideoPlayerOnloopPointReached;
            }
        }

        private void Update()
        {
            if (_showPlayerControl && _videoPlayer.isPlaying)
            {
                _playerControls.SetPlayheadPosition(_videoPlayer.time);
            }

            if (_srtParser != null)
            {
                CheckSubtitles();
            }
        }

        private void OnDisable()
        {
            if (_videoPlayer)
            {
                _videoPlayer.loopPointReached -= VideoPlayerOnloopPointReached;
            }
        }

        public void Play()
        {
            _videoPlayer.Play();
        }

        public void Pause()
        {
            _videoPlayer.Pause();
        }

        public void Stop()
        {
            _videoPlayer.Stop();
        }
        
        public void TogglePlay()
        {
            if (_videoPlayer.isPlaying)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void GoToSeconds(double seconds)
        {
            _videoPlayer.time = seconds;
        }
        
        public void SetVideoClip(VideoClip videoClip)
        {
            _videoPlayer.clip = _videoClip;
            _videoPlayer.playOnAwake = _autoPlay;
        }

        public void SetVideoClipURL(string url)
        {
            _videoPlayer.url = url;
        }
        
        public void SetSubtitles(TextAsset subtitlesAsset)
        {
            _srtFile = subtitlesAsset;
            _srtParser = new SRTParser(_srtFile);
        }

        private void VideoPlayerOnloopPointReached(VideoPlayer source)
        {
            SubtitleBlock subtitleBlock = _srtParser.GetForTime((float) _videoPlayer.time);
            if (subtitleBlock.Index != _currentSubtitleIndex)
            {
                ShowSubtitle(subtitleBlock);
                _currentSubtitleIndex = subtitleBlock.Index;
            }
            OnVideoClipFinished?.Invoke(source.clip);
        }

        private void CheckSubtitles()
        {
            SubtitleBlock subtitleBlock = _srtParser.GetForTime((float) _videoPlayer.time);

            if (subtitleBlock.Index != _currentSubtitleIndex)
            {
                ShowSubtitle(subtitleBlock);
                _currentSubtitleIndex = subtitleBlock.Index;
            }
        }

        private void ShowSubtitle(SubtitleBlock subtitleBlock)
        {
            // if(_currentSubtitleIndex > 0)
            // {
            //     OnHideSubtitle?.Invoke(_srtParser._subtitles[_currentSubtitleIndex - 1].Text);
            // }
            if(subtitleBlock != SubtitleBlock.Blank)
            {
                OnShowSubtitle?.Invoke(subtitleBlock.Text);
            }
            else if(_currentSubtitleIndex > 0)
            {
                OnHideSubtitle?.Invoke(_srtParser._subtitles[_currentSubtitleIndex - 1].Text);
            }
            if (_showSubtitles)
            {
                _subtitlePanel.SetText(subtitleBlock.Text);
            }
        }
    }
}