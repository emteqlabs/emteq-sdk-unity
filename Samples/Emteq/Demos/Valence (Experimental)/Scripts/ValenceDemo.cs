
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace EmteqLabs
{
    public class ValenceDemo : MonoBehaviour
    {
        [SerializeField] private Button neutralButton;
        [SerializeField] private Button smileButton;
        [SerializeField] private Button enableRmsButton;
        [SerializeField] private Button finishButton;
        [SerializeField] private Text instructionText;
        [SerializeField] private ExpressionGauge expressionGauge;

        private const string _instructions = "Push the button to start recording";
        private IEnumerator _timerCoroutine;
        //To be wired up to SDK 2
        //private CalibrationTypes _currentType;
        private float _timeElapsed = 0f;

        void Start()
        {
            // Sync framerate to monitors refresh rate
            QualitySettings.vSyncCount = 1;

            finishButton.enabled = false;
            enableRmsButton.enabled = false;
            neutralButton.enabled = true;
            smileButton.enabled = true;
            //To be wired up to SDK 2
            //EmteqManager.OnValenceReceived += OnValenceReceived;
        }
        void OnDestroy()
        {
            if (EmteqManager.Instance)
            {
                //EmteqManager.Instance.OnValenceReceived -= OnValenceReceived;
            }
        }

        private void OnValenceReceived(object sender, float valence)
        {
            expressionGauge.SetValue(valence);
        }

        public void StartNeutral()
        {
            //To be wired up to SDK 2
            //_currentType = CalibrationTypes.Baseline;
            StartCalibration();
        }

        public void StartSmile()
        {
            //To be wired up to SDK 2
            //_currentType = CalibrationTypes.Expression;
            StartCalibration();
        }

        private void StartCalibration()
        {
            finishButton.enabled = true;
            neutralButton.enabled = false;
            smileButton.enabled = false;
            enableRmsButton.enabled = false;
            _timerCoroutine = TimerCoroutineHandler(0.1f);
            StartCoroutine(_timerCoroutine);
            //To be wired up to SDK 2
            //EmteqDevice.StartCalibration(_currentType);
        }

        public void FinishCalibration()
        {
            StopCoroutine(_timerCoroutine);
            _timeElapsed = 0f;
            finishButton.enabled = false;
            enableRmsButton.enabled = true;
            neutralButton.enabled = true;
            smileButton.enabled = true;
            instructionText.text = _instructions;
            //To be wired up to SDK 2
            //EmteqDevice.StopCalibration(_currentType);
        }

        public void EnableNormRms()
        {
            //To be wired up to SDK 2
            //EmteqDevice.StartNormalisedEmgRms();
        }

        private IEnumerator TimerCoroutineHandler(float waitTime)
        {
            while (true)
            {
                _timeElapsed += waitTime;
                instructionText.text = string.Format("{0} seconds", Math.Round(_timeElapsed, 2));
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}