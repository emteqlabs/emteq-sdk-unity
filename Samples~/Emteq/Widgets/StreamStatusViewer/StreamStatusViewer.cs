using System;
using System.Collections.Generic;
using EmteqLabs.Faceplate;
using EmteqLabs.MaskProtocol;
using Emteq.Runtime.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmteqLabs
{
    public class StreamStatusViewer : MonoBehaviour
    {
        public TMP_Text statusText;

        private void Start()
        {
            VideoStreamManager.Instance.OnDeviceVideoStreamStatusChange += OnVideoStreamStatusChange;
            VideoStreamManager.Instance.OnVideoStreamStatusWidgetVisibilityChange += OnVideoStreamWidgetToggleChange;
            
            statusText.text = VideoStreamManager.Instance.State.ToString();
            
            this.gameObject.SetActive(VideoStreamManager.Instance.StreamStatusWidgetVisible);
        }

        private void OnDestroy()
        {
            VideoStreamManager.Instance.OnDeviceVideoStreamStatusChange -= OnVideoStreamStatusChange;
            VideoStreamManager.Instance.OnVideoStreamStatusWidgetVisibilityChange -= OnVideoStreamWidgetToggleChange;
        }
        
        private void OnVideoStreamStatusChange(VideoStreamManager.StreamingStates newState)
        {
            statusText.text = newState.ToString();
        }
        
        private void OnVideoStreamWidgetToggleChange(bool newSetting)
        {
            this.gameObject.SetActive(newSetting);
        }
        
    }
}
