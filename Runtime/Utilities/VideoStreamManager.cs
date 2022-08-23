using System.Threading;
using EmteqLabs;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Signaling;
using UnityEngine;

namespace Emteq.Runtime.Utilities
{
    public class VideoStreamManager : MonoBehaviour
    {
        public static VideoStreamManager Instance { get; private set; }

        [SerializeField, Tooltip("Streaming size should match display aspect ratio. It must be set prior to initialisation.")]
        public Vector2Int streamingResolution = new Vector2Int(640, 360);
        
        [SerializeField, Tooltip("By default the streaming system copies all properties from the existing main camera to align their rendering settings, if this causes problems or you wish to manage the properties manually then you can disable the behaviour with this flag.")]
        public bool autoCopyCameraProperties = true;
        
        public float signalingInterval = 5f;
        public bool enableHwCodec = true;
        
        public bool StreamStatusWidgetVisible
        {
            get => _streamStatusWidgetVisible;
            set
            {
                _streamStatusWidgetVisible = value;
                OnVideoStreamStatusWidgetVisibilityChange?.Invoke(_streamStatusWidgetVisible);
            }
        }
        private bool _streamStatusWidgetVisible = false;

        public enum StreamingStates
        {
            NotInitialised,
            Initialised,
            Streaming,
        }
        public StreamingStates State
        {
            get => _state;
            private set
            {
                _state = value;
                OnDeviceVideoStreamStatusChange?.Invoke(_state);
            }
        }
        private StreamingStates _state;
        
        public delegate void VideoStreamStatusChangeDelegate(StreamingStates newState);
        public event VideoStreamStatusChangeDelegate OnDeviceVideoStreamStatusChange;

        public delegate void VideoStreamWidgetToggleDelegate(bool newSetting);
        public event VideoStreamWidgetToggleDelegate OnVideoStreamStatusWidgetVisibilityChange;
        
        
        private string _targetIpAddress = "";
        private string _targetPort = "";
        private const float _heartbeatInterval = 3f;

        private bool _isConnected = false;
        private float _lastHeartbeatTime = 0f;
        
        private Camera _mainCamera;
        
        private GameObject _webrtcObject;
        private GameObject _streamerObject;
        private EmteqCameraStreamer _cameraStreamer;
        private EmteqAudioStreamer _audioStreamer;
        private RenderStreaming _renderStreaming;
        private Broadcast _streamBroadcast;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            
            this.State = StreamingStates.NotInitialised;

        }

        private void Start()
        {
            EmteqManager.OnVideoStreamConfig += OnVideoStreamConfig;
            EmteqManager.OnVideoStreamStatus += OnVideoStreamReport;
            
            this._mainCamera = Camera.main;
        }

        private void Update()
        {
            if (this._state == StreamingStates.Streaming && Time.time > this._lastHeartbeatTime + VideoStreamManager._heartbeatInterval)
            {
                Debug.Log("Supervision video stream heartbeat timeout (" + VideoStreamManager._heartbeatInterval + " seconds)");
                DeinitialiseSystem();
            }
        }
        
        public void MainCameraChanged()
        {
            if (_cameraStreamer != null)
            {
                _cameraStreamer.MainCameraChanged();
            }
        }
        public void AudioListenerChanged()
        {
            if (_audioStreamer != null)
            {
                _audioStreamer.AudioListenerChanged();
            }
        }
        
        private void OnVideoStreamConfig(string macAddress, string ipAddress, string port)
        {
            // mac address not yet implemented
            this._targetIpAddress = ipAddress;
            this._targetPort = port;
            
            _lastHeartbeatTime = Time.time;
            
            Debug.Log("Received supervision video stream config: " + this._targetIpAddress + ":" + this._targetPort);
            
            switch (this.State)
            {
                case StreamingStates.NotInitialised:
                    InitialiseSystem();
                    AttemptConnection();
                    break;
                case StreamingStates.Initialised:
                    AttemptConnection();
                    break;
                case StreamingStates.Streaming:
                    AttemptConnection();
                    break;
            }
        }
        
        private void OnVideoStreamReport(bool isConnected)
        {
            this._isConnected = isConnected;
            
            switch (this._isConnected)
            {
                case true:
                    _lastHeartbeatTime = Time.time;
                    
                    switch (this.State)
                    {
                        case StreamingStates.NotInitialised:
                            break;
                        case StreamingStates.Initialised:
                            this.State = StreamingStates.Streaming;
                            break;
                        case StreamingStates.Streaming:
                            break;
                    }

                    break;
                case false:
                    switch (this.State)
                    {
                        case StreamingStates.NotInitialised:
                            break;
                        case StreamingStates.Initialised:
                            Debug.Log("Supervision video stream disconnected");
                            DeinitialiseSystem();
                            break;
                        case StreamingStates.Streaming:
                            Debug.Log("Supervision video stream disconnected");
                            DeinitialiseSystem();
                            break;
                    }

                    break;
            }
        }

        private void InitialiseSystem()
        {
            this._webrtcObject = new GameObject("WebRtcRenderStreaming");
            this._webrtcObject.transform.parent = this.transform;
            
            this._webrtcObject.SetActive(false); // required as RenderStreaming defaults to running within its Awake function
            this._renderStreaming = this._webrtcObject.AddComponent<RenderStreaming>();
            this._renderStreaming.runOnAwake = false;
            this._streamBroadcast = this._webrtcObject.AddComponent<Broadcast>();
            this._webrtcObject.SetActive(true);
            
            this._streamerObject = new GameObject("CameraStreamer");
            this._streamerObject.transform.parent = this.transform;
            
            // Only add the streamer script here, it manages the actual camera component
            this._cameraStreamer = this._streamerObject.AddComponent<EmteqCameraStreamer>();

            this._audioStreamer = this._streamerObject.AddComponent<EmteqAudioStreamer>();

            // Registers the streamers with the broadcast system
            this._streamBroadcast.AddComponent(_cameraStreamer);
            this._streamBroadcast.AddComponent(_audioStreamer);
            
            this.State = StreamingStates.Initialised;
        }
        
        private void AttemptConnection()
        {
            ISignaling signalingConfiguration = new WebSocketSignaling($"ws://{_targetIpAddress + ":" + _targetPort}", this.signalingInterval, SynchronizationContext.Current);

            var handlers = new SignalingHandlerBase[1];
            handlers[0] = _streamBroadcast;
                
            _renderStreaming.Run(
                hardwareEncoder: enableHwCodec,
                signaling: signalingConfiguration,
                handlers: handlers);
        }

        private void DeinitialiseSystem()
        {
            this.State = StreamingStates.NotInitialised;

            Destroy(this._audioStreamer);
            Destroy(this._cameraStreamer);

            Destroy(this._renderStreaming);
            Destroy(this._streamerObject);
            Destroy(this._streamBroadcast);
            Destroy(this._webrtcObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                EmteqManager.OnVideoStreamConfig -= OnVideoStreamConfig;
                EmteqManager.OnVideoStreamStatus -= OnVideoStreamReport;
            
                DeinitialiseSystem();
            }
            
        }
    }
}
