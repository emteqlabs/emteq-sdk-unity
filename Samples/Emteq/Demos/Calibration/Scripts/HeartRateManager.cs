using EmteqLabs.Models;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;


namespace EmteqLabs
{
    public class HeartRateManager : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private GameObject heartRateUI;
        [SerializeField] private TMP_Text statusTextTMP;
        [SerializeField] private TMP_Text currentHRTextTMP;
        [SerializeField] private TMP_Text medianHRTextTMP;
        [SerializeField] private double heartRateMedianBaseline;
        [SerializeField] private double currentAverageHeartRate;
        [SerializeField] private bool baseLineReady = false;
        [SerializeField] private bool averageHRReady = false;
        [SerializeField] private bool enableHeartRateUI = false;
        #endregion

        #region Unity Methods
        private void Start()
        {
            if (heartRateUI != null)
            {
                heartRateUI.SetActive(enableHeartRateUI);
            }
            
            if (EmteqManager.IsDeviceConnected() == false)
            {
                EmteqManager.OnDeviceConnect += OnEmteqDeviceConnectionSuccess;
                EmteqManager.OnDeviceDisconnect += OnEmteqDeviceConnectionError;
                statusTextTMP.text = ("<color=blue>Connecting to Emteq Device</color>");
            }
            else
            {
                OnEmteqDeviceConnectionSuccess();
            }
        }
        #endregion

        #region Public Methods
        public void CalculateBaseline()
        {
            EmteqManager.StartHeartRateBaselineCalibration();
        }

        public void ShowBaselineResult()
        {
            BaselineHeartRateData baselineHeartRateData = EmteqManager.EndHeartRateBaselineCalibration();
            heartRateMedianBaseline = baselineHeartRateData.Median;
            medianHRTextTMP.text = heartRateMedianBaseline.ToString("F");
            baseLineReady = true;
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            EmteqManager.OnHeartRateAverageUpdate -= OnHeartRateUpdate;

            EmteqManager.OnDeviceConnect -= OnEmteqDeviceConnectionSuccess;
            EmteqManager.OnDeviceDisconnect -= OnEmteqDeviceConnectionError;
        }

        private void OnEmteqDeviceConnectionError()
        {
            statusTextTMP.text = ("<color=red>Could not connect to Emteq Device</color>");
        }

        private void OnEmteqDeviceConnectionSuccess()
        {
            statusTextTMP.text = ("<color=blue>Detecting Heart Rate...</color>");
            EmteqManager.OnHeartRateAverageUpdate += OnHeartRateUpdate;
        }

        private void OnHeartRateUpdate(double _heartRate)
        {
            if (_heartRate > 0d)
            {
                statusTextTMP.text = ("<color=green>Heart Rate Detected...</color>");
                currentAverageHeartRate = _heartRate;
                currentHRTextTMP.text = currentAverageHeartRate.ToString("F");
                averageHRReady = true;
            }
        }
        #endregion
    }
}
