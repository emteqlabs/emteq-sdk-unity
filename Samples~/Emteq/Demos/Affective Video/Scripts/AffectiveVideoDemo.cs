using EmteqLabs.Models;
using EmteqLabs.Video;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace EmteqLabs
{
    public class AffectiveVideoDemo : MonoBehaviour
    {
        [SerializeField] private CustomCalibration _customCalibration;
        [SerializeField] private GameObject _customCalibrationParent;
        [SerializeField] private SrtVideoPlayer _videoPlayer;

        private void Awake()
        {
            _customCalibration.OnExpressionCalibrationComplete += OnExpressionCalibrationComplete;
        }

        private void Start()
        {
            // Sync framerate to monitors refresh rate
            QualitySettings.vSyncCount = 1;
            _customCalibrationParent.gameObject.SetActive(true);
            _videoPlayer.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _videoPlayer.OnShowSubtitle -= OnShowSubtitle;
        }

        private void OnExpressionCalibrationComplete(EmgCalibrationData expressionCalibrationData)
        {
            _customCalibration.OnExpressionCalibrationComplete -= OnExpressionCalibrationComplete;
            _customCalibrationParent.gameObject.SetActive(false);
            _videoPlayer.gameObject.SetActive(true);
            PlayVideo();
        }

        private void PlayVideo()
        {
            _videoPlayer.OnShowSubtitle += OnShowSubtitle;
            _videoPlayer.OnHideSubtitle += OnHideSubtitle;
            _videoPlayer.OnVideoClipFinished += OnVideoClipFinished;
            _videoPlayer.Play();
        }
        
        private void OnShowSubtitle(string subtitle)
        {
            //Debug.Log(subtitle+" Show");
            EmteqManager.StartDataSection(subtitle);
        }

        private void OnHideSubtitle(string subtitle)
        {
            //Debug.Log(subtitle+" Hide");
            EmteqManager.EndDataSection(subtitle);
        }

        private void OnVideoClipFinished(VideoClip videoClip)
        {
            _videoPlayer.OnShowSubtitle -= OnShowSubtitle;
            _videoPlayer.OnHideSubtitle -= OnHideSubtitle;
            _videoPlayer.OnVideoClipFinished -= OnVideoClipFinished;
        }
    }
}
