using UnityEngine;

namespace EmteqLabs.EyeTracking
{
    public class EyeTrackingDemo : MonoBehaviour
    {
        void Start()
        {
            EyeTrackingManager.OnEnterGaze += OnEnterGaze;
            EyeTrackingManager.OnExitGaze += OnExitGaze;
        }

        private void OnDestroy()
        {
            EyeTrackingManager.OnEnterGaze -= OnEnterGaze;
            EyeTrackingManager.OnExitGaze -= OnExitGaze;
        }

        private void OnEnterGaze(TrackedObject trackedobject)
        {
            var cube = trackedobject.gameObject.GetComponent<InteractiveCube>();
            if (cube != null)
            {
                cube.Animate();
            }
        }
        
        private void OnExitGaze(TrackedObject trackedobject)
        {
            var cube = trackedobject.gameObject.GetComponent<InteractiveCube>();
            if (cube != null)
            {
                cube.ResetAnimation();
            }
        }
    }
}
