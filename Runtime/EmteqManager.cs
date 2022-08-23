using System.Collections.Generic;
using UnityEngine;
using EmteqLabs.Faceplate;
using EmteqLabs.MaskProtocol;
using EmteqLabs.Models;
using UnityEditor;

namespace EmteqLabs
{
    public class EmteqManager : MonoBehaviour
    {
        public static EmteqManager Instance { get; private set; }
        public static bool ShowContactPrompt = true;
        public static bool DataRecordingOn = false;

        #region  Serialisable properties
        [SerializeField]
        private bool _autoStartRecordingData = true;

        [SerializeField]
        private bool _showContactPrompt = true;

        public bool ShowLogMessages = true;
        #endregion


        #region EmteqDevice Public Events
        public static event EmteqPlugin.DeviceConnectDelegate OnDeviceConnect
        {
            add
            {
                _onDeviceConnectDelegate += value;
                EmteqPlugin.Instance.OnDeviceConnect += OnPluginDeviceConnectHandler;
            }
            remove
            {
                _onDeviceConnectDelegate -= value;
                EmteqPlugin.Instance.OnDeviceConnect -= OnPluginDeviceConnectHandler;
            }
        }

        private static EmteqPlugin.DeviceConnectDelegate _onDeviceConnectDelegate;
        private static void OnPluginDeviceConnectHandler()
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onDeviceConnectDelegate?.Invoke();
            });
        }

        public static event EmteqPlugin.DeviceDisconnectDelegate OnDeviceDisconnect
        {
            add
            {
                _onDeviceDisconnectDelegate += value;
                EmteqPlugin.Instance.OnDeviceDisconnect += OnPluginDeviceDisconnectHandler;
            }
            remove
            {
                _onDeviceDisconnectDelegate -= value;
                EmteqPlugin.Instance.OnDeviceDisconnect -= OnPluginDeviceDisconnectHandler;
            }
        }

        private static EmteqPlugin.DeviceDisconnectDelegate _onDeviceDisconnectDelegate;
        private static void OnPluginDeviceDisconnectHandler()
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onDeviceDisconnectDelegate?.Invoke();
            });
        }

        public static event EmteqPlugin.LogDelegate OnDeviceLog;
        private static void OnLogMessageReceivedPluginHandler(string logMessage, LogType logType)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                Logger.LogMessage(logMessage, logType);
                OnDeviceLog?.Invoke(logMessage, logType);
            });
        }

        public event EmteqPlugin.DeviceFitStateChangeDelegate OnDeviceFitStateChange
        {
            add
            {
                _onDeviceFitStateChange += value;
                EmteqPlugin.Instance.OnDeviceFitStateChange += OnPluginDeviceFitStateChangeHandler;
            }
            remove
            {
                _onDeviceFitStateChange -= value;
                EmteqPlugin.Instance.OnDeviceFitStateChange -= OnPluginDeviceFitStateChangeHandler;
            }
        }

        private static EmteqPlugin.DeviceFitStateChangeDelegate _onDeviceFitStateChange;
        private static void OnPluginDeviceFitStateChangeHandler(FitState fitState)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onDeviceFitStateChange?.Invoke(fitState);
            });
        }

        public static event EmteqPlugin.SensorContactStateChangeDelegate OnSensorContactStateChange
        {
            add
            {
                _onSensorContactStateChange += value;
                EmteqPlugin.Instance.OnSensorContactStateChange += SensorContactStateChangeHandler;
            }
            remove
            {
                _onSensorContactStateChange -= value;
                EmteqPlugin.Instance.OnSensorContactStateChange -= SensorContactStateChangeHandler;
            }
        }

        private static event EmteqPlugin.SensorContactStateChangeDelegate _onSensorContactStateChange;
        private static void SensorContactStateChangeHandler(Dictionary<MuscleMapping, ContactState> sensorContactState)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onSensorContactStateChange?.Invoke(sensorContactState);
            });
        }

        public static event EmteqPlugin.HeartRateAverageUpdateDelegate OnHeartRateAverageUpdate // once per second
        {
            add
            {
                _onHeartRateAverageUpdate += value;
                EmteqPlugin.Instance.OnHeartRateAverageUpdate += OnPluginHeartRateAverageUpdateHandler;
            }
            remove
            {
                _onHeartRateAverageUpdate -= value;
                EmteqPlugin.Instance.OnHeartRateAverageUpdate -= OnPluginHeartRateAverageUpdateHandler;
            }
        }

        private static event EmteqPlugin.HeartRateAverageUpdateDelegate _onHeartRateAverageUpdate;
        private static void OnPluginHeartRateAverageUpdateHandler(double bpm)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onHeartRateAverageUpdate?.Invoke(bpm);
            });
        }

        public static event EmteqPlugin.ValenceUpdateDelegate OnValenceUpdate
        {
            add
            {
                _onValenceUpdate += value;
                EmteqPlugin.Instance.OnValenceUpdate += OnPluginValenceUpdateHandler;
            }
            remove
            {
                _onValenceUpdate -= value;
                EmteqPlugin.Instance.OnValenceUpdate -= value;
            }
        }

        private static event EmteqPlugin.ValenceUpdateDelegate _onValenceUpdate;
        private static void OnPluginValenceUpdateHandler(float normalisedValence)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onValenceUpdate?.Invoke(normalisedValence);
            });
        }

        public static event EmteqPlugin.VideoStreamConfigDelegate OnVideoStreamConfig
        {
            add
            {
                _onVideoStreamConfigDelegate += value;
                EmteqPlugin.Instance.OnVideoStreamConfig -= OnVideoStreamConfigHandler;
                EmteqPlugin.Instance.OnVideoStreamConfig += OnVideoStreamConfigHandler;
            }
            remove
            {
                _onVideoStreamConfigDelegate -= value;
                EmteqPlugin.Instance.OnVideoStreamConfig -= OnVideoStreamConfigHandler;
            }
        }

        private static EmteqPlugin.VideoStreamConfigDelegate _onVideoStreamConfigDelegate;
        private static void OnVideoStreamConfigHandler(string macAddress, string ipAddress, string port)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onVideoStreamConfigDelegate?.Invoke(macAddress, ipAddress, port);
            });
        }

        public static event EmteqPlugin.VideoStreamStatusDelegate OnVideoStreamStatus
        {
            add
            {
                _onVideoStreamStatusDelegate += value;
                EmteqPlugin.Instance.OnVideoStreamStatus -= OnVideoStreamStatusHandler;
                EmteqPlugin.Instance.OnVideoStreamStatus += OnVideoStreamStatusHandler;
            }
            remove
            {
                _onVideoStreamStatusDelegate -= value;
                EmteqPlugin.Instance.OnVideoStreamStatus -= OnVideoStreamStatusHandler;
            }
        }
        private static EmteqPlugin.VideoStreamStatusDelegate _onVideoStreamStatusDelegate;
        private static void OnVideoStreamStatusHandler(bool isConnected)
        {
            UnityThreadRelay.Instance.Invoke(() =>
            {
                _onVideoStreamStatusDelegate?.Invoke(isConnected);
            });
        }
        #endregion

        #region Unity Life Cycle
        void Awake()
        {
            Application.logMessageReceived += ApplicationOnlogMessageReceived;
            if (FindObjectOfType<EmteqManager>() != null && Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            ShowContactPrompt = _showContactPrompt;
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }

        void Start()
        {
            if (Instance == this)
            {
                EmteqPlugin.Instance.OnLogMessageReceived += OnLogMessageReceivedPluginHandler;
            }
        }

        void OnEnable()
        {
            if (Instance == this)
            {
                EmteqPlugin.Instance.OnDeviceConnect += OnDeviceConnectHandler;
                EmteqPlugin.Instance.Enable();
            }
        }

        private void OnDeviceConnectHandler()
        {
            if (_autoStartRecordingData)
            {
                StartRecordingData();
            }
        }

        void OnDisable()
        {
            if (Instance == this)
            {
                StopRecordingData();
                EmteqPlugin.Instance.OnDeviceConnect -= OnDeviceConnectHandler;
                EmteqPlugin.Instance.Disable();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Application.logMessageReceived -= ApplicationOnlogMessageReceived;
                EmteqPlugin.Instance.OnLogMessageReceived -= OnLogMessageReceivedPluginHandler;
                ShowContactPrompt = true;
            }
        }
        #endregion
        
        #region Public API Methods
        public static bool IsDeviceConnected()
        {
            return EmteqPlugin.Instance.IsDeviceConnected();
        }

        public static Dictionary<MuscleMapping, ushort> GetEmgAmplitudeRms()
        {
            return EmteqPlugin.Instance.GetEmgAmplitudeRms();
        }

        public static PpgRawSignal GetRawPpgSignal()
        {
            return EmteqPlugin.Instance.GetRawPpgSignal();
        }

        public static void StartRecordingData()
        {
            EmteqPlugin.Instance.StartRecordingData();
            DataRecordingOn = true;
        }

        public static void StopRecordingData()
        {
            EmteqPlugin.Instance.StopRecordingData();
            DataRecordingOn = false;
        }

        public static void SetParticipantID(string id)
        {
            EmteqPlugin.Instance.SetParticipantID(id);
            ShowContactPrompt = true;
        }

        public static void SetDataPoint(string label)
        {
            EmteqPlugin.Instance.SetDataPoint(label);
        }

        public static void SetDataPoint<T>(string label, T metadata)
        {
            EmteqPlugin.Instance.SetDataPoint<T>(label, metadata);
        }

        public static void StartDataSection(string label)
        {
            EmteqPlugin.Instance.StartDataSection(label);
        }

        public static void StartDataSection<T>(string label, T metadata)
        {
            EmteqPlugin.Instance.StartDataSection<T>(label, metadata);
        }

        public static void EndDataSection(string label)
        {
            EmteqPlugin.Instance.EndDataSection(label);
        }

        public static void EndDataSection<T>(string label, T metadata)
        {
            EmteqPlugin.Instance.EndDataSection<T>(label, metadata);
        }
        #endregion


        #region Calibration
        public static void StartExpressionCalibration(ExpressionType expressionType, FaceSide faceSide = FaceSide.Left)
        {
            EmteqPlugin.Instance.StartExpressionCalibration(expressionType);
        }

        public static EmgCalibrationData EndExpressionCalibration()
        {
            return EmteqPlugin.Instance.EndExpressionCalibration();
        }

        public static void ResetExpressionCalibrationValues()
        {
            EmteqPlugin.Instance.ResetExpressionCalibrationValues();
        }

        public static void StartHeartRateBaselineCalibration()
        {
            EmteqPlugin.Instance.StartBaselineHeartRateCalibration();
        }

        public static BaselineHeartRateData EndHeartRateBaselineCalibration()
        {
            return EmteqPlugin.Instance.EndBaselineHeartRateCalibration();
        }

        public static void ResetBaselineHeartRateCalibration()
        {
            EmteqPlugin.Instance.ResetBaselineHeartRateCalibration();
        }

        public static Dictionary<MuscleMapping, float> GetNormalisedEmgRms()
        {
            return EmteqPlugin.Instance.GetNormalisedEmgRms();
        }

        public static Dictionary<MuscleMapping, float> GetSustainedNormalisedEmgRms()
        {
            return EmteqPlugin.Instance.GetSustainedNormalisedEmgRms();
        }
        #endregion

        private void ApplicationOnlogMessageReceived(string condition, string stacktrace, UnityEngine.LogType type)
        {
            if (type == UnityEngine.LogType.Exception)
            {
                Logger.LogMessage(string.Format("condition: {0} | stacktrace: {1}", condition, stacktrace), LogType.Exception);
            }
        }
    }
}
