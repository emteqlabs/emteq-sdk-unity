using EmteqLabs.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;


namespace EmteqLabs
{
    public class HeartRateDemo : MonoBehaviour
    {
        [SerializeField] private GameObject _currentHRInstructions;

        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _currentHRText;

        [SerializeField] private GameObject _baselineInstructions;
        [SerializeField] private GameObject _baselinePanels;
        [SerializeField] private Button _calculateBaselineButton;
        [FormerlySerializedAs("_showBaselineButton")][SerializeField] private Button _showResultsButton;
        [SerializeField] private TMP_Text _standardDeviationHRText;
        [SerializeField] private TMP_Text _medianHRText;

        private void Start()
        {
            // Sync framerate to monitors refresh rate
            QualitySettings.vSyncCount = 1;

            EmteqManager.OnDeviceConnect += OnEmteqDeviceConnectionSuccess;
            EmteqManager.OnDeviceDisconnect += OnEmteqDeviceConnectionError;
            EmteqManager.OnHeartRateAverageUpdate += OnHeartRateUpdate;

            if (EmteqManager.IsDeviceConnected() == false)
            {
                _statusText.text = ("<color=#00FFFF>Connecting to Emteq Device</color>");
            }
            else
            {
                OnEmteqDeviceConnectionSuccess();
            }
        }

        private void OnDisable()
        {
            EmteqManager.OnHeartRateAverageUpdate -= OnHeartRateUpdate;
            EmteqManager.OnDeviceConnect -= OnEmteqDeviceConnectionSuccess;
            EmteqManager.OnDeviceDisconnect -= OnEmteqDeviceConnectionError;
        }

        private void OnEmteqDeviceConnectionError()
        {
            _statusText.text = ("<color=#FF4500>Could not connect to Emteq Device</color>");
        }

        private void OnEmteqDeviceConnectionSuccess()
        {
            _statusText.text = ("<color=#00FFFF>Detecting Heart Rate...</color>");
        }

        private void OnHeartRateUpdate(double hr)
        {
            if (hr > 0d)
            {
                _statusText.text = ("<color=#00FF7F>Heart Rate Detected...</color>");
                _currentHRInstructions.SetActive(false);
                _currentHRText.text = hr.ToString("F");
            }
        }

        //called from button in Unity
        public void CalculateBaseline()
        {
            _calculateBaselineButton.gameObject.SetActive(false);
            _showResultsButton.gameObject.SetActive(true);
            _baselineInstructions.SetActive(false);
            _baselinePanels.SetActive(true);
            EmteqManager.StartHeartRateBaselineCalibration();
        }

        //called from a button in Unity
        public void ShowBaselineResult()
        {
            _showResultsButton.gameObject.SetActive(false);
            BaselineHeartRateData baselineHeartRateData = EmteqManager.EndHeartRateBaselineCalibration();
            _standardDeviationHRText.text = baselineHeartRateData.StandardDeviation.ToString("F");
            _medianHRText.text = baselineHeartRateData.Median.ToString("F");
        }
    }
}
