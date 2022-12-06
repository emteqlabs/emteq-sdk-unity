using System.Collections.Generic;
using UnityEngine;
using EmteqLabs.Faceplate;
using EmteqLabs.MaskProtocol;
using EmteqLabs.Models;
using UnityEditor;
using System;

namespace EmteqLabs
{
    public class EmteqManager : MonoBehaviour
    {
        private static EmteqManager instance_ = null;

        public static EmteqManager Instance
        {
            get
            {
                if (instance_ == null)
                    Debug.LogAssertion("EmteqManager.Instance is NULL");

                return instance_;
            }
        }

        public static bool ShowContactPrompt = true;
        public static bool DataRecordingOn = false;
        public static bool LslBroadcastEnabled = false;

        #region  Serialisable properties
        [SerializeField]
        private bool _autoStartRecordingData = true;

        [SerializeField]
        [Tooltip("Warning: May cause instability on Pico")]
        private bool _autoStartLabStreamingLayerBroadcast = true;

        [SerializeField]
        private bool _showContactPrompt = true;

        public bool ShowLogMessages = true;
        #endregion

        private UnityThreadRelay unityThreadRelay_ = new UnityThreadRelay();

        #region EmteqDevice Public Events
        public static event EmteqPlugin.DeviceConnectDelegate OnDeviceConnect
        {
            add
            {
                Instance._onDeviceConnectDelegate -= value; // prevents duplication
                Instance._onDeviceConnectDelegate += value;
            }
            remove
            {
                Instance._onDeviceConnectDelegate -= value;
            }
        }

        private EmteqPlugin.DeviceConnectDelegate _onDeviceConnectDelegate;
        private void OnPluginDeviceConnectHandler()
        {
            if (_autoStartRecordingData)
            {
                StartRecordingData();
            }

            unityThreadRelay_.Invoke(() =>
            {
                _onDeviceConnectDelegate?.Invoke();
            });
        }

        public static event EmteqPlugin.DeviceDisconnectDelegate OnDeviceDisconnect
        {
            add
            {
                Instance._onDeviceDisconnectDelegate -= value; // prevents duplication
                Instance._onDeviceDisconnectDelegate += value;
            }
            remove
            {
                Instance._onDeviceDisconnectDelegate -= value;
            }
        }

        private EmteqPlugin.DeviceDisconnectDelegate _onDeviceDisconnectDelegate;
        private void OnPluginDeviceDisconnectHandler()
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onDeviceDisconnectDelegate?.Invoke();
            });
        }

        public event EmteqPlugin.LogDelegate OnDeviceLog;
        private void OnLogMessageReceivedPluginHandler(string logMessage, LogType logType)
        {
            unityThreadRelay_.Invoke(() =>
            {
                Logger.LogMessage(logMessage, logType);
                OnDeviceLog?.Invoke(logMessage, logType);
            });
        }

        public static event EmteqPlugin.DeviceFitStateChangeDelegate OnDeviceFitStateChange
        {
            add
            {
                Instance._onDeviceFitStateChange -= value; // prevents duplication
                Instance._onDeviceFitStateChange += value;
            }
            remove
            {
                Instance._onDeviceFitStateChange -= value;
            }
        }

        private EmteqPlugin.DeviceFitStateChangeDelegate _onDeviceFitStateChange;
        private void OnPluginDeviceFitStateChangeHandler(FitState fitState)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onDeviceFitStateChange?.Invoke(fitState);
            });
        }

        public static event EmteqPlugin.SensorContactStateChangeDelegate OnSensorContactStateChange
        {
            add
            {
                Instance._onSensorContactStateChange -= value; // prevents duplication
                Instance._onSensorContactStateChange += value;
            }
            remove
            {
                Instance._onSensorContactStateChange -= value;
            }
        }

        private event EmteqPlugin.SensorContactStateChangeDelegate _onSensorContactStateChange;
        private void OnSensorContactStateChangeHandler(Dictionary<MuscleMapping, ContactState> sensorContactState)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onSensorContactStateChange?.Invoke(sensorContactState);
            });
        }

        public static event EmteqPlugin.HeartRateAverageUpdateDelegate OnHeartRateAverageUpdate // once per second
        {
            add
            {
                Instance._onHeartRateAverageUpdate -= value; // prevents duplication
                Instance._onHeartRateAverageUpdate += value;
            }
            remove
            {
                Instance._onHeartRateAverageUpdate -= value;
            }
        }

        private event EmteqPlugin.HeartRateAverageUpdateDelegate _onHeartRateAverageUpdate;
        private void OnPluginHeartRateAverageUpdateHandler(double bpm)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onHeartRateAverageUpdate?.Invoke(bpm);
            });
        }

        public static event EmteqPlugin.ValenceUpdateDelegate OnValenceUpdate
        {
            add
            {
                Instance._onValenceUpdate -= value; // prevents duplication
                Instance._onValenceUpdate += value;
            }
            remove
            {
                Instance._onValenceUpdate -= value;
            }
        }

        private event EmteqPlugin.ValenceUpdateDelegate _onValenceUpdate;
        private void OnPluginValenceUpdateHandler(float normalisedValence)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onValenceUpdate?.Invoke(normalisedValence);
            });
        }

        public static event EmteqPlugin.VideoStreamConfigDelegate OnVideoStreamConfig
        {
            add
            {
                Instance._onVideoStreamConfigDelegate -= value; // prevents duplication
                Instance._onVideoStreamConfigDelegate += value;
            }
            remove
            {
                Instance._onVideoStreamConfigDelegate -= value;
            }
        }

        private EmteqPlugin.VideoStreamConfigDelegate _onVideoStreamConfigDelegate;
        private void OnVideoStreamConfigHandler(string macAddress, string ipAddress, string port)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onVideoStreamConfigDelegate?.Invoke(macAddress, ipAddress, port);
            });
        }

        public static event EmteqPlugin.VideoStreamStatusDelegate OnVideoStreamStatus
        {
            add
            {
                Instance._onVideoStreamStatusDelegate -= value; // prevents duplication
                Instance._onVideoStreamStatusDelegate += value;
            }
            remove
            {
                Instance._onVideoStreamStatusDelegate -= value;
            }
        }
        private EmteqPlugin.VideoStreamStatusDelegate _onVideoStreamStatusDelegate;
        private void OnVideoStreamStatusHandler(bool isConnected)
        {
            unityThreadRelay_.Invoke(() =>
            {
                _onVideoStreamStatusDelegate?.Invoke(isConnected);
            });
        }
        #endregion

        #region Unity Life Cycle

        private void Update()
        {
            unityThreadRelay_.Update();
        }

        void Awake()
        {
            // Don't create secondary instance when Emteqmanager is instantiated in multiple scenes
            if (instance_ != null)
            {
                Destroy(gameObject);
                return;
            }

            instance_ = this;
            Application.logMessageReceived += ApplicationOnlogMessageReceived;
            ShowContactPrompt = _showContactPrompt;
            DontDestroyOnLoad(gameObject); //< Don't unload on scene change
            Debug.Log("EmteqManager Instance Created");
        }

        void Start()
        {
            if (instance_ != this)
                Debug.LogAssertion("EmteqManager.Start() on NON-Instance!");

            EmteqPlugin.Instance.OnDeviceConnect += OnPluginDeviceConnectHandler;
            EmteqPlugin.Instance.OnDeviceDisconnect += OnPluginDeviceDisconnectHandler;
            EmteqPlugin.Instance.OnDeviceFitStateChange += OnPluginDeviceFitStateChangeHandler;
            EmteqPlugin.Instance.OnSensorContactStateChange += OnSensorContactStateChangeHandler;
            EmteqPlugin.Instance.OnHeartRateAverageUpdate += OnPluginHeartRateAverageUpdateHandler;
            EmteqPlugin.Instance.OnLogMessageReceived += OnLogMessageReceivedPluginHandler;
            EmteqPlugin.Instance.OnValenceUpdate += OnPluginValenceUpdateHandler;
            EmteqPlugin.Instance.OnVideoStreamConfig += OnVideoStreamConfigHandler;
            EmteqPlugin.Instance.OnVideoStreamStatus += OnVideoStreamStatusHandler;

            EmteqPlugin.Instance.Enable();

            if (_autoStartLabStreamingLayerBroadcast == true)
            {
                EnableLabStreamingLayerBroadcast();
            }
        }

        void OnDisable()
        {
            if (instance_ != this)
            {
                //Debug.LogAssertion("EmteqManager.OnDisable() on NON-Instance!");
                return; //< Unity 2021.3.10f1 will stil call `OnDisable` from `Destroy` inside Awake()
            }

            StopRecordingData();
            EmteqPlugin.Instance.Disable();

            EmteqPlugin.Instance.OnDeviceConnect -= OnPluginDeviceConnectHandler;
            EmteqPlugin.Instance.OnDeviceDisconnect -= OnPluginDeviceDisconnectHandler;
            EmteqPlugin.Instance.OnDeviceFitStateChange -= OnPluginDeviceFitStateChangeHandler;
            EmteqPlugin.Instance.OnSensorContactStateChange -= OnSensorContactStateChangeHandler;
            EmteqPlugin.Instance.OnHeartRateAverageUpdate -= OnPluginHeartRateAverageUpdateHandler;
            EmteqPlugin.Instance.OnLogMessageReceived -= OnLogMessageReceivedPluginHandler;
            EmteqPlugin.Instance.OnValenceUpdate -= OnPluginValenceUpdateHandler;
            EmteqPlugin.Instance.OnVideoStreamConfig -= OnVideoStreamConfigHandler;
            EmteqPlugin.Instance.OnVideoStreamStatus -= OnVideoStreamStatusHandler;
        }

        private void OnDestroy()
        {
            if (instance_ == this)
            {
                ShowContactPrompt = true;                
                instance_ = null;
            }
        }
        #endregion

        #region Public API Methods
        public static bool IsDeviceConnected()
        {
            return EmteqPlugin.Instance.IsDeviceOpen;
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

        //Warning: LSL may cause instability on Pico
        public static void EnableLabStreamingLayerBroadcast()
        {
            EmteqPlugin.Instance.EnableLabStreamingLayerBroadcast();
            LslBroadcastEnabled = true;
        }

        public static void DisableLabStreamingLayerBroadcast()
        {
            EmteqPlugin.Instance.DisableLabStreamingLayerBroadcast();
            LslBroadcastEnabled = false;
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
