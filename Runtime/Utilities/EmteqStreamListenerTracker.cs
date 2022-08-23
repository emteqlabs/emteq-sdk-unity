using UnityEngine;
using Unity.WebRTC;

namespace Emteq.Runtime.Utilities
{
    [RequireComponent(typeof(AudioListener))]
    public class EmteqStreamListenerTracker : MonoBehaviour
    {
        public AudioStreamTrack audioTrack;
        const int sampleRate = 48000;

        public bool TrackState()
        {
            bool enabled = false;
            // if the stream has been disposed of then reading Enabled will fail
            try
            {
                enabled = audioTrack.Enabled;
            }
            catch (System.Exception)
            {
                enabled = false;
            }
            return enabled;
        }
        public void Disable()
        {

            if (TrackState() == true)
            {
                audioTrack.Stop();
                audioTrack.Enabled = false;
                audioTrack.Dispose();
                VideoStreamManager.Instance.AudioListenerChanged();
            }
        }
        private void OnDestroy()
        {
            Disable();
        }
        // This method is called on the audio thread.
        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (audioTrack != null &&
                VideoStreamManager.Instance.State == VideoStreamManager.StreamingStates.Streaming)
            {
                audioTrack.SetData(data, channels, sampleRate);
            }
        }
    }
}
