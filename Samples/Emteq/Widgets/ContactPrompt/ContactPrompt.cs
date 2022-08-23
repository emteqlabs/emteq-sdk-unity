using System.Collections;
using System.Collections.Generic;
using EmteqLabs.MaskProtocol;
using UnityEngine;
using UnityEngine.Serialization;

namespace EmteqLabs
{
    public class ContactPrompt : MonoBehaviour
    {
        private const float _fitStateBufferInSeconds = 5;
        [SerializeField]
        private FitState _fitStateThreshold = FitState.Average;
        [SerializeField]
        private float _hidePromptIfStableForSeconds = 3;
        [SerializeField]
        private string _promptMessage;
        private EmteqMaskGUI _emteqMaskGUI;
        
        private float _currentCountdownValue;
        private bool _isEvaluating = false;
        private FitState _currentFitState = FitState.None;
        private bool _isBuffering = false;
        private bool _startContactBuffer = false;
        private bool _initialised = false;

        void Start()
        {
            _initialised = true;
            _emteqMaskGUI = GetComponentInChildren <EmteqMaskGUI>();
            EmteqManager.Instance.OnDeviceFitStateChange += OnFitStateChange;
            _emteqMaskGUI.SetInstructions(_promptMessage);
        }
        
        private void Update()
        {
            EvaluateFitStateStability();
            if (_startContactBuffer == true)
            {
                _startContactBuffer = false;
                StartCoroutine("FitStateBufferTimer");
            }
        }

        private void OnDestroy()
        {
            if (_initialised == true)
            {
                EmteqManager.Instance.OnDeviceFitStateChange -= OnFitStateChange;
            }
        }

        private void OnFitStateChange(FitState fitState)
        {
            _currentFitState = fitState;
            
            if (EmteqManager.ShowContactPrompt == false)
            {
                return;
            }
            
            Logger.LogMessage(string.Format("FitState: {0}", _currentFitState.ToString()), LogType.Log);
            
            //if fitstate falls below threshold, prompt VR User
            if (fitState < _fitStateThreshold)
            {
                if (!_isBuffering)
                {
                    _startContactBuffer = true;
                }
            }
        }

        private void StartEvaluatingStability()
        {
            if (_emteqMaskGUI.IsActive() == false)
            {
                _emteqMaskGUI.Show();
            }
            _currentCountdownValue = _hidePromptIfStableForSeconds;
            _isEvaluating = true;
        }

        private void EvaluateFitStateStability()
        {
            if (_isEvaluating == true)
            {
                // fitstate must stay stable for a number of seconds
                if (_currentFitState < _fitStateThreshold)
                {
                    StartEvaluatingStability();
                }
                else
                {
                    _currentCountdownValue -= Time.deltaTime;
                    if(_currentCountdownValue <= 0f)
                    {
                        _emteqMaskGUI.Hide();
                        _isEvaluating = false;
                    }
                }
            }
        }

        private IEnumerator FitStateBufferTimer()
        {
            _isBuffering = true;
            yield return new WaitForSeconds(_fitStateBufferInSeconds);
            if (_currentFitState < _fitStateThreshold)
            {
                //trigger only if not already evaluating
                if (_isEvaluating == false)
                {
                    StartEvaluatingStability();
                }
            }
            _isBuffering = false;
        }

        public void DismissContactPrompt()
        {
            _emteqMaskGUI.Hide();
            StopEvaluatingFitStateStability();
            StopBufferingFitStateChange();
        }

        private void StopEvaluatingFitStateStability()
        {
            _isEvaluating = false;
        }
        
        private void StopBufferingFitStateChange()
        {
            StopAllCoroutines();
            _isBuffering = false;
        }

        public void DisableContactPrompt()
        {
            DismissContactPrompt();
            EmteqManager.ShowContactPrompt = false;
        }
    }
}