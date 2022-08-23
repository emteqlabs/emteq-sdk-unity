#if EMTEQ_EYE
using System.Runtime.InteropServices;
using EmteqLabs.EyeTracking;
using ViveSR.anipal.Eye;
using EyeData_v2 = ViveSR.anipal.Eye.EyeData_v2;
using GazeIndex = ViveSR.anipal.Eye.GazeIndex;
#endif
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace EmteqLabs
{
    public class EyeTrackingManager : MonoBehaviour
    {
        public delegate void GazeDelegate(TrackedObject trackedObject);
        public static event GazeDelegate OnEnterGaze;
        public static event GazeDelegate OnExitGaze;

        public LayerMask trackedObjectLayers = 1;


#if EMTEQ_EYE

        private static string _filePath => $"{Application.persistentDataPath}/{DateTime.Now.ToString("yyyy-MM-dd HHmmss")}.eyedata";

        private static FileStream _fileStream;

        private static BinaryFormatter _formatter;

        private static EyeData_v2 _eyeData;
        private static bool _eyeCallbackRegistered = false;

        private static byte[] _eyeDataBytes;
        private static TrackedObjectInfo _trackedObjectInfo;
        
        
        private FocusInfo _focusInfo;
        private readonly float _maxDistance = 20;
        private readonly GazeIndex[] _gazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        
        private Collider _objectInFocus;
        private Collider _previousObjectInFocus;
        private TrackedObject _trackedObject;
        
        
        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
            }
            else
            {
                _fileStream = new FileStream(_filePath, FileMode.Create);
                _formatter = new BinaryFormatter();
            }
        }

        private void Update()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && _eyeCallbackRegistered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                _eyeCallbackRegistered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && _eyeCallbackRegistered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                _eyeCallbackRegistered = false;
            }

            bool eyeFocus = false;
            foreach (GazeIndex index in _gazePriority)
            {
                Ray gazeRay;

                if (_eyeCallbackRegistered)
                    eyeFocus = SRanipal_Eye_v2.Focus(index, out gazeRay, out _focusInfo, 0, _maxDistance, trackedObjectLayers, _eyeData);
                else
                    eyeFocus = SRanipal_Eye_v2.Focus(index, out gazeRay, out _focusInfo, 0, _maxDistance, trackedObjectLayers);

                if (eyeFocus)
                {
                    _previousObjectInFocus = _objectInFocus;
                    _objectInFocus = _focusInfo.collider;

                    var previousTrackedObject = _trackedObject;
                    _trackedObject = _focusInfo.transform.GetComponent<TrackedObject>();
                    if (_trackedObject != null)
                    {
                        _trackedObjectInfo = new TrackedObjectInfo(_trackedObject.ObjectId, _focusInfo.distance);
                    }

                    if (_objectInFocus == _previousObjectInFocus)
                    {
                        //do nothing
                    }
                    else
                    {
                        if (previousTrackedObject != null)
                        {
                            ExitGaze(previousTrackedObject);
                        }
                        
                        if (_trackedObject != null)
                        {
                            EnterGaze(_trackedObject);
                        }
                    }
                    break;
                }
            }
            if(!eyeFocus)
            {
                _previousObjectInFocus = null;
                _objectInFocus = null;
                if (_trackedObject != null)
                {
                    ExitGaze(_trackedObject);
                }
                _trackedObject = null;
            }
        }
        
        private void OnDisable()
        {
            Release();
        }

        void OnApplicationQuit()
        {
            Release();
        }

        private void EnterGaze(TrackedObject trackedObject)
        {
            trackedObject.EnterGaze(trackedObject.name);
            OnEnterGaze?.Invoke(trackedObject);
        }
        
        private void ExitGaze(TrackedObject trackedObject)
        {
            trackedObject.ExitGaze(trackedObject.name);
            OnExitGaze?.Invoke(trackedObject);
        }

        /// <summary>
        /// Release callback thread when disabled or quit
        /// </summary>
        private static void Release()
        {
            
            if (_eyeCallbackRegistered == true)
            {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                _eyeCallbackRegistered = false;
            }
            
            // _dataCollection.CompleteAdding();
            _fileStream?.Close();
        }

        /// <summary>
        /// Required class for IL2CPP scripting backend support
        /// </summary>
        internal class MonoPInvokeCallbackAttribute : System.Attribute
        {
            public MonoPInvokeCallbackAttribute() { }
        }

        /// <summary>
        /// Eye tracking data callback thread.
        /// Callback runs on a separate thread to report at ~120hz.
        /// Note: Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
        /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
        /// </summary>
        /// <param name="eye_data">Reference to latest eye_data</param>
        [MonoPInvokeCallback]
        private static void EyeCallback(ref EyeData_v2 eye_data)
        {
            _eyeData = eye_data;
            
            EmteqEyeData data = new EmteqEyeData(_eyeData);
            
            _formatter.Serialize(_fileStream, data);
                
            _trackedObjectInfo = new TrackedObjectInfo();
        }
        
        public List<EmteqEyeData> Deserialize(string filePath)
        {        
            //$"{Application.persistentDataPath}/" + fileName + ".eyedata";

            List<EmteqEyeData> eyeDataList = new List<EmteqEyeData>();

            FileStream fs = new FileStream(filePath, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the data from the file and
                // assign the reference to the local variable. 
                while (true)
                {
                    var eyeData = (EmteqEyeData)formatter.Deserialize(fs);
                    eyeDataList.Add(eyeData);
                }
            }
            catch (SerializationException e)
            {
                Debug.Log("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
                Debug.Log("Deserialization complete");
            }

            return eyeDataList;
            
        }
#endif
    }
    public class Vector3SerializationSurrogate : ISerializationSurrogate
    {

        // Method called to serialize a Vector3 object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {

            Vector3 v3 = (Vector3)obj;
            info.AddValue("x", v3.x);
            info.AddValue("y", v3.y);
            info.AddValue("z", v3.z);
        }

        // Method called to deserialize a Vector3 object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {

            Vector3 v3 = (Vector3)obj;
            v3.x = (float)info.GetValue("x", typeof(float));
            v3.y = (float)info.GetValue("y", typeof(float));
            v3.z = (float)info.GetValue("z", typeof(float));
            obj = v3;
            return obj;
        }
    }
}