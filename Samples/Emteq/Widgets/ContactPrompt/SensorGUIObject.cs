using System;
using EmteqLabs.Faceplate;
using EmteqLabs.MaskProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmteqLabs
{
    public class SensorGUIObject : MonoBehaviour
    {
        public MuscleMapping SensorName;
        public bool IsDebugSensor = false;
        [SerializeField] 
        private TMP_Text _displayValue;
        [SerializeField]
        private Image[] _sensorImages;
        [SerializeField]
        private MeshRenderer _sensorRenderer;

        public void SetSensorValue(ushort value)
        {
            _displayValue.text = value.ToString();
        }

        public void SetContactState(MuscleMapping sensorName, ContactState contactState)
        {
            switch (contactState)
            {
                case ContactState.Off:
                case ContactState.Off_On:
                case ContactState.Off_Stable:
                case ContactState.Stable_Off:
                case ContactState.On_Off:
                    SetSensorColor(Color.gray);
                    break;
                case ContactState.On:
                case ContactState.On_Stable:
                case ContactState.Stable_On:
                    SetSensorColor(Color.cyan);
                    break;
                case ContactState.Stable:
                case ContactState.Settled:
                    SetSensorColor(Color.green);
                    break;
                case ContactState.Fault_Stable:
                case ContactState.Stable_Fault:
                case ContactState.Fault:
                    SetSensorColor(Color.red);
                    break;
                default:
                    SetSensorColor(Color.white);
                    break;
            }
        }

        private void SetSensorColor(Color color)
        {
           
            if (IsDebugSensor)
            {
                try
                {
                    foreach (Image image in _sensorImages)
                    {
                        image.color = color;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogMessage(string.Format("Object name: {0}", gameObject.name));
                    Logger.LogMessage(e.ToString(), LogType.Exception);
                    throw;
                }
            }
            else
            {
                try
                {
                    _sensorRenderer.material.color = color;
                }
                catch (Exception e)
                {
                    Logger.LogMessage(string.Format("Object name: {0}", gameObject.name));
                    Logger.LogMessage(e.ToString(), LogType.Exception);
                    throw;
                }
            }
        }
    }
}