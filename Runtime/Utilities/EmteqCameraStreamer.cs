using System.Collections.Generic;
using Unity.RenderStreaming;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Animations;

namespace Emteq.Runtime.Utilities
{
    public class EmteqCameraStreamer : VideoStreamSender
    {
        public override Texture SendTexture => _camera.targetTexture;

        public int depth = 24;
        public int antiAliasing = 2;
        public bool enableMipMaps = true;
        private bool _lookingForNewCamera = false;

        private Camera _camera;
        private ParentConstraint _cameraConstraint;
        private EmteqStreamCameraTracker _cameraTracker;
        private RenderTexture _streamingTexture;

        protected virtual void Awake()
        {
            this._camera = this.gameObject.AddComponent<Camera>();
            this._cameraConstraint = this.gameObject.AddComponent<ParentConstraint>();
            this._cameraConstraint.constraintActive = true;

            RenderTextureFormat format = WebRTC.GetSupportedRenderTextureFormat(SystemInfo.graphicsDeviceType);
            this._streamingTexture = new RenderTexture(VideoStreamManager.Instance.streamingResolution.x, VideoStreamManager.Instance.streamingResolution.y, depth, format)
            {
                antiAliasing = this.antiAliasing,
                useMipMap = this.enableMipMaps
            };
            this._streamingTexture.Create();

            Camera currentMain = Camera.main;

            if (currentMain != null)
            {
                ConfigureForNewCamera(currentMain);
            }
            else
            {
                this._lookingForNewCamera = true;
            }
        }

        private void Update()
        {
            if (_lookingForNewCamera == true)
            {
                Camera currentMain = Camera.main;

                if (currentMain != null)
                {
                    this._lookingForNewCamera = false;

                    ConfigureForNewCamera(currentMain);
                }
            }
        }

        public void MainCameraChanged()
        {
            this._lookingForNewCamera = true;
        }

        public void ConfigureForNewCamera(Camera newSourceCamera)
        {
            this._cameraTracker = newSourceCamera.gameObject.AddComponent<EmteqStreamCameraTracker>();

            if (VideoStreamManager.Instance.autoCopyCameraProperties == true)
            {
                this._camera.CopyFrom(newSourceCamera);
            }

            _camera.targetTexture = this._streamingTexture; // Copying from the new one will have overwritten our render texture setting

            List<ConstraintSource> newSourceList = new List<ConstraintSource>();

            ConstraintSource newConstraint = new ConstraintSource();
            newConstraint.sourceTransform = newSourceCamera.transform;
            newConstraint.weight = 1;

            newSourceList.Add(newConstraint);

            this._cameraConstraint.SetSources(newSourceList);

        }

        protected override MediaStreamTrack CreateTrack()
        {
            return new VideoStreamTrack(this._streamingTexture);
        }

        private void OnDestroy()
        {
            Destroy(this._cameraTracker);
        }
    }
}
