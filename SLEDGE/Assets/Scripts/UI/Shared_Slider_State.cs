using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Shared_Slider_State : MonoBehaviour
{
    public float value;
    public float min;
    public float max;

    public TMP_InputField textBox;
    public Slider slider;

    public UnityEvent onValueChange;

    private void Awake()
    {
        textBox.onSubmit.AddListener(delegate { TextUpdated(); });
        slider.onValueChanged.AddListener(delegate { SliderUpdate(); });
    }
    public void TextUpdated()
    {
        if (float.TryParse(textBox.text, out float result))
        {
            UpdateValue(result);
        }  
    }

    public void SliderUpdate()
    {
        UpdateValue(slider.value);
    }

    public void UpdateValue(float value)
    {
        this.value = Mathf.Clamp(value, min, max);
        textBox.text = Mathf.Round(value).ToString();
        slider.value = value;
        onValueChange.Invoke();
    }
}
