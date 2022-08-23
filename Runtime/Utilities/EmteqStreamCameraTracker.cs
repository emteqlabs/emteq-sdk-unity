using UnityEngine;

namespace Emteq.Runtime.Utilities
{
    public class EmteqStreamCameraTracker : MonoBehaviour
    {
        private void OnDestroy()
        {
            VideoStreamManager.Instance.MainCameraChanged();
        }
    }
}
