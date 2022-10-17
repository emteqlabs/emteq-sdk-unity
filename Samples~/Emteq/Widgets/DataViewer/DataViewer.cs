using System.Collections.Generic;
using EmteqLabs.Faceplate;
using EmteqLabs.MaskProtocol;
using UnityEngine;

namespace EmteqLabs
{
    public class DataViewer : MonoBehaviour
    {
        [SerializeField]
        private List<SensorGUIObject> _sensors;

        private Dictionary<MuscleMapping, ushort> _emgAmplitudeRms;

        private void Start()
        {
            EmteqManager.OnSensorContactStateChange += OnSensorContactStateChange;
        }

        void Update()
        {
            _emgAmplitudeRms = EmteqManager.GetEmgAmplitudeRms();
            foreach(SensorGUIObject sensor in _sensors)
            {
                sensor.SetSensorValue(_emgAmplitudeRms[sensor.SensorName]);
            }
        }

        private void OnDisable()
        {
            EmteqManager.OnSensorContactStateChange -= OnSensorContactStateChange;
        }
        
        private void OnSensorContactStateChange(Dictionary<MuscleMapping, ContactState> sensorcontactstate)
        {
            foreach(SensorGUIObject sensor in _sensors)
            {
                sensor.SetContactState(sensor.SensorName, sensorcontactstate[sensor.SensorName]);
            }
        }

        void SetVisible(bool value)
        {
            this.gameObject.SetActive(value);
        }
    }
}