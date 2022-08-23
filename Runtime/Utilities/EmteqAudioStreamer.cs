using System;
using System.Linq;
using Unity.WebRTC;
using UnityEngine;
using Unity.RenderStreaming;

namespace Emteq.Runtime.Utilities
{
    public class EmteqAudioStreamer : AudioStreamSender
    {
        private MediaStream m_audioStream;
        private AudioListener m_audioListener;
        
        private EmteqStreamListenerTracker _listenerTracker;

        private bool _lookingForNewListener = false;
        
        void Awake()
        {
            AudioListener currentListener = GameObject.FindObjectOfType<AudioListener>();
            
            if (currentListener != null)
            {
                ConfigureForNewListener(currentListener);
            }
            else
            {
                this._lookingForNewListener = true;
            }
        }

        private void Update()
        {
            if (_lookingForNewListener == true)
            {
                AudioListener currentListener = GameObject.FindObjectOfType<AudioListener>();
                
                if (currentListener != null)
                {
                    this._lookingForNewListener = false;

                    ConfigureForNewListener(currentListener);
                }
            }
        }
        
        void OnDisable()
        {
            base.OnDisable();

            if (this._listenerTracker.audioTrack == null)
                return;

            this._listenerTracker.Disable();
        }
        private void OnDestroy()
        {
            Destroy(this._listenerTracker);
            VideoStreamManager.Instance.AudioListenerChanged();
        }

        public void AudioListenerChanged()
        {
            this._lookingForNewListener = true;
        }
        
        public void ConfigureForNewListener(AudioListener newAudioListener)
        {
            Destroy(this._listenerTracker);
            
            this._listenerTracker = newAudioListener.gameObject.AddComponent<EmteqStreamListenerTracker>();
            this.m_audioListener = newAudioListener;
        }

        protected override MediaStreamTrack CreateTrack()
        {
            this._listenerTracker.audioTrack = new AudioStreamTrack();
            this._listenerTracker.audioTrack.Enabled = true;
            return this._listenerTracker.audioTrack;
        }


    }
}
