using EmteqLabs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartRateBaselineCalibration : MonoBehaviour
{
    public Image TimerImage;
    public float BaselineTimeInSeconds = 120f;
    private float calibrationStartTime = 0;
    public CustomCalibration Calibration;
    public HeartRateManager HeartRateManager;

    void OnEnable()
    {
        calibrationStartTime = Time.time;
        HeartRateManager.CalculateBaseline();
    }

    private void Update()
    {
        ReduceTimer();
    }

    private void ReduceTimer()
    {
        float timeDelta = Time.time - calibrationStartTime;
        if(TimerImage.fillAmount < 1)
        {
            TimerImage.fillAmount = timeDelta / BaselineTimeInSeconds;
        }
        else
        {
            HeartRateManager.ShowBaselineResult();
            Calibration.ContinueExpressionCalibration();
            gameObject.SetActive(false);
        }
    }
}
