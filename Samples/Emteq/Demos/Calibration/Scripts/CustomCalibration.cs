using System;
using System.Collections;
using EmteqLabs.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace EmteqLabs
{
    public class CustomCalibration : MonoBehaviour
    {
        #region Public variables
        // Subscribe to this to remove the panel once calibration is finished
        public delegate void OnExpressionCalibrationCompleteDelegate(EmgCalibrationData expressionCalibrationData);
        public event OnExpressionCalibrationCompleteDelegate OnExpressionCalibrationComplete;
        public bool UseRecordingButton = false;
        #endregion

        #region Private serialized fields
        /// <summary>
        /// Calibration now includes new initial Heart-rate baseline step.
        /// </summary>
        [SerializeField] 
        private int _numberCalibrationSteps = 7;
        [SerializeField]
        private int _recordingTime = 3;
        [SerializeField]
        private int _instructionsDisplayTime = 10;
        [SerializeField]
        private int _currentCalibrationStep;

        [SerializeField] 
        private string[] _calibrationStepInstructions;
        [SerializeField] 
        private string[] _calibrationStepNames;

        [SerializeField] 
        private ExpressionAvatar[] _calibrationExpressions;
        
        [SerializeField] 
        private GameObject _rootCanvas;
        [SerializeField]
        private GameObject _instructionsCountdownParent;
        [SerializeField]
        private GameObject _calibrationStepInstructionsPanel;
        [SerializeField]
        private GameObject _calibrationCompletePanel;
        [SerializeField]
        private GameObject _baselineCalibrationPanel;

        [SerializeField] 
        private Button _recordingButton;

        [SerializeField]
        private TMP_Text _recordingCountdownTextfieldTMP;
        [SerializeField]
        private TMP_Text _instructionsCountdownTextfieldTMP;
        [SerializeField]
        private TMP_Text _calibrationStepInstructionsTextfieldTMP;
        [SerializeField]
        private TMP_Text _calibrationStepNameTextfieldTMP;
        
        [SerializeField] 
        private Animator _progressUIAnimator;

        [SerializeField] 
        private Slider _calibrationProgressBar;
        #endregion

        #region Private variables
        private EmgCalibrationData _emgCalibrationData;
        private string _currentCalibrationStepInstructions;
        private string _currentCalibrationStepName;
        private int _currentCountdownTime = 0;
        #endregion

        #region Unity Methods
        void Start()
        {
            Initialise();
            UpdateCalibrationStepText();
            ChangeExpressionAvatar();
            _recordingButton.gameObject.SetActive(false);
            _instructionsCountdownParent.SetActive(false);
            //HandleRecording(true);
        }
        #endregion


        #region Public Methods
        public void ContinueExpressionCalibration()
        {
            MoveToNextCalibrationStep();
            UpdateCalibrationStepText();
            ChangeExpressionAvatar();
            UpdateCalibrationProgressBar((float)_currentCalibrationStep / (_numberCalibrationSteps));
            HandleRecording(true);
        }

        public void RecordExpression()
        {
            StartCoroutine("HandleCalibrationProgress");
            EnableCalibrationStepUI(false);
        }

        public void EndCurrentExpression()
        {
            EmteqManager.EndExpressionCalibration();
            MoveToNextCalibrationStep();
            UpdateCalibrationProgressBar((float)_currentCalibrationStep / (_numberCalibrationSteps));
        }

        public void EndCalibration()
        {
            EmteqManager.EndDataSection("Main calibration sequence");

            EmgCalibrationData data = EmteqManager.EndExpressionCalibration();
            //Logger.LogMessage($"Calibration Complete: {data.ToString()}");

            UpdateCalibrationProgressBar(1);
            AnimateCalibrationStepUI("Complete");
            OnExpressionCalibrationComplete?.Invoke(data);
        }
        #endregion

        #region Private Methods
        private void HandleRecording(bool displayButton)
        {
            if (UseRecordingButton == true)
            {
                _recordingButton.gameObject.SetActive(displayButton);
                _instructionsCountdownParent.SetActive(false);
            }
            else
            {
                _instructionsCountdownParent.SetActive(displayButton);
                _recordingButton.gameObject.SetActive(false);
                if (displayButton)
                {
                    StartCoroutine(HandleCalibrationInstructionsTimer());
                }
            }
        }

        private void Initialise()
        {
            if (_numberCalibrationSteps != 0)
            {
                //ExpressionAvatar is only required by _currentCalibrationStep index 1 -> _numberCalibrationSteps - 1, as the new Heart-rate baseline step has been added.
                if (_calibrationExpressions == null)
                {
                    _calibrationExpressions = new ExpressionAvatar[_numberCalibrationSteps - 1];
                }
            }

            if (_calibrationStepInstructions == null)
            {
                _calibrationStepInstructions = new string[_numberCalibrationSteps];
            }
            if (_calibrationStepNames == null)
            {
                _calibrationStepNames = new string[_numberCalibrationSteps];
            }

            _rootCanvas.SetActive(true);
            _currentCalibrationStep = 0;
            _calibrationProgressBar.value = 0;
        }
        
        private void UpdateCalibrationStepText()
        {
            _currentCalibrationStepInstructions = _calibrationStepInstructions[_currentCalibrationStep];
            _currentCalibrationStepName = _calibrationStepNames[_currentCalibrationStep];
            _calibrationStepInstructionsTextfieldTMP.text = _currentCalibrationStepInstructions;
            _calibrationStepNameTextfieldTMP.text = _currentCalibrationStepName;
        }
        
        private void ChangeExpressionAvatar()
        {
            ResetExpressionAvatars();
            EnableNextExpressionAvatar();
        }
        
        private void ResetExpressionAvatars()
        {
            foreach (ExpressionAvatar expressionAvatar in _calibrationExpressions)
            {
                expressionAvatar.gameObject.SetActive(false);
            }
        }
        
        private void EnableNextExpressionAvatar()
        {
            if (_currentCalibrationStep != 0)
            {
                _baselineCalibrationPanel.SetActive(false);
                _calibrationExpressions[_currentCalibrationStep - 1].gameObject.SetActive(true);
            }
            else
            {
                //If _currentCalibrationStep == 0, this is the new Heart-rate baseline step.
                EmteqManager.StartDataSection("Main calibration sequence");

                _baselineCalibrationPanel.SetActive(true);
            }
        }
                
        private IEnumerator HandleCalibrationProgress()
        {
            if (_currentCalibrationStep != 0)
            {
                InitialiseCountdownTime();
                AnimateCalibrationStepUI("Recording");

                EmteqManager.StartExpressionCalibration(_calibrationExpressions[_currentCalibrationStep - 1].SelectedExpression);

                for (int i = 0; i < _recordingTime; i++)
                {
                    DecreaseRecordingCountdownTimer();
                    yield return new WaitForSeconds(1f);

                    if (_currentCountdownTime == 0)
                    {
                        if (_currentCalibrationStep < _numberCalibrationSteps - 1)
                        {
                            EndCurrentExpression();
                            UpdateCalibrationStepText();
                            ResetRecordingCountdownTimer();
                            AnimateCalibrationStepUI("Finished");
                            EnableCalibrationStepUI(true);
                            ChangeExpressionAvatar();
                        }
                        else
                        {
                            EndCalibration();
                            EnableCalibrationCompletePanel();
                        }
                    }
                } 
            }
        }

        private void InitialiseCountdownTime()
        {
            _currentCountdownTime = _recordingTime;
        }
        
        private void InitialiseInstructionsCountdownTime()
        {
            _currentCountdownTime = _instructionsDisplayTime;
        }

        private IEnumerator HandleCalibrationInstructionsTimer()
        {
            InitialiseInstructionsCountdownTime();
            
            for (int i = 0; i < _instructionsDisplayTime; i++)
            {
                DecreaseInstructionsCountdownTimer();
                yield return new WaitForSeconds(1f);

                if (_currentCountdownTime == 0)
                {
                    ResetInstructionsCountdownTimer();
                    RecordExpression();
                }
            }
        }

        private void EnableCalibrationStepUI(bool enabled)
        {
            _calibrationStepInstructionsPanel.SetActive(enabled);
            EnableInstructionsTransition(enabled);
        }

        private void EnableInstructionsTransition(bool enabled)
        {
            if (UseRecordingButton)
            {
                HandleRecording(enabled);
            }
            else if(enabled)
            {
                StartCoroutine(HandleCalibrationInstructionsTimer());
            }
        }

        private void AnimateCalibrationStepUI(string animationTrigger)
        {
            _progressUIAnimator.SetTrigger(animationTrigger);
        }
        
        private void DecreaseRecordingCountdownTimer()
        {
            _recordingCountdownTextfieldTMP.text = _currentCountdownTime.ToString();
            _currentCountdownTime--;
        }
        
        private void DecreaseInstructionsCountdownTimer()
        {
            _instructionsCountdownTextfieldTMP.text = _currentCountdownTime.ToString();
            _currentCountdownTime--;
        }
         
        private void ResetRecordingCountdownTimer()
        {
            _recordingCountdownTextfieldTMP.text = "";
            _currentCountdownTime = _instructionsDisplayTime;
        }

        private void ResetInstructionsCountdownTimer()
        {
            _instructionsCountdownTextfieldTMP.text = "";
            _currentCountdownTime = _recordingTime;
        }
        
        private void EnableCalibrationCompletePanel()
        {
            _calibrationCompletePanel.SetActive(true);
        }
        
        private void UpdateCalibrationProgressBar(float progress)
        {
            _calibrationProgressBar.value = progress;
        }

        private void MoveToNextCalibrationStep()
        {
            if (_currentCalibrationStep < _numberCalibrationSteps - 1)
            {
                _currentCalibrationStep++;
            }
        }
        #endregion
    }
}