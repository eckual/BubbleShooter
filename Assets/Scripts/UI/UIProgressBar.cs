using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIProgressBar : MonoBehaviour
{
    public string format = "{0}/{1}";
    public TMP_Text info;
    public Slider slider;
    public GameObject filledObject;

    public void SetData(float value,float minValue, float maxValue)
    {
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = value;

        if (info)
            info.text = string.Format(format, value, maxValue);

        if (filledObject)
            filledObject.SetActive(value >= maxValue);
    }
}
