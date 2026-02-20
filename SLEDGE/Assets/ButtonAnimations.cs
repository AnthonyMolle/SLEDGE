using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
using System.Xml;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAnimations : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] Button button;
    [SerializeField] UnityEvent onClick;
    private Vector2 baseDimensions;
    [Header("Text")]
    [SerializeField] TextMeshProUGUI text;
    private float textBaseFontSize;
    [Header("Color")]
    private UnityEngine.Color normalColor;
    [SerializeField] UnityEngine.Color highlightedColor;
    [SerializeField] UnityEngine.Color pressedColor;
    [Header("Curves")]
    [SerializeField] AnimationCurve normX;
    [SerializeField] AnimationCurve normY;
    [SerializeField] float normT;
    [SerializeField] float normS = 1;
    [SerializeField] AnimationCurve highX;
    [SerializeField] AnimationCurve highY;
    [SerializeField] float highT;
    [SerializeField] float highS = 1;
    [SerializeField] AnimationCurve pressX;
    [SerializeField] AnimationCurve pressY;
    [SerializeField] float pressT;
    [SerializeField] float pressS = 1;
    [SerializeField] AnimationCurve selecX;
    [SerializeField] AnimationCurve selecY;
    [SerializeField] float selecT;
    [SerializeField] float selecS = 1;
    [SerializeField] AnimationCurve disabX;
    [SerializeField] AnimationCurve disabY; 
    [SerializeField] float disabT;
    [SerializeField] float disabS = 1;

    [SerializeField] float T;
    private enum ButtonState
    {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }
    private ButtonState buttonstate = ButtonState.Normal;
    // bool moving;
    bool swapping;

    void Start()
    {
        baseDimensions = button.image.rectTransform.sizeDelta;
        textBaseFontSize = text.fontSize;
        normalColor = text.color;
    }

    public void Normal()
    {
        if (buttonstate != ButtonState.Normal)
        {
            swapping = true;
            T = 0;
        }
        text.color = normalColor;
        buttonstate = ButtonState.Normal;
    }

    public void Highlighted()
    {
        if (buttonstate != ButtonState.Highlighted)
        {
            swapping = true;
            T = 0;
        }
        text.color = highlightedColor;
        buttonstate = ButtonState.Highlighted;
    }

    public void Pressed()
    {
        if (buttonstate != ButtonState.Pressed)
        {
            swapping = true;
            T = 0;
        }
        text.color = pressedColor;
        buttonstate = ButtonState.Pressed;
    }

    void Update()
    {
        // if (moving)
        // {
            switch (buttonstate)
            {
                case ButtonState.Normal:
                if (swapping)
                    {
                        EvalCurve(normX, normY, T/normT, normS);
                    }
                break;
                case ButtonState.Highlighted:
                if (swapping)
                    {
                        EvalCurve(highX, highY, T/highT, highS);
                    }
                break;
                case ButtonState.Pressed:
                if (swapping)
                    {
                        EvalCurve(pressX, pressY, T/pressT, pressS);
                    }
                break;
            }
            T += Time.unscaledDeltaTime;
        // }
    }

    public void OnClick()
    {
        onClick.Invoke();
    }

    private void EvalCurve(AnimationCurve curveX, AnimationCurve curveY, float T, float S)
    {
        // var localScale = button.image.transform.localScale;
        var x = curveX.Evaluate(T) * S;
        var y = curveY.Evaluate(T) * S;
        button.image.rectTransform.sizeDelta = new Vector2(baseDimensions.x * x, baseDimensions.y * y);
        text.fontSize = textBaseFontSize * y;
        // localScale = new Vector3(baseScale.x * x, baseScale.y * y, localScale.z);
        // button.image.transform.localScale = localScale;
    }
}
