using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCaption : MonoBehaviour
{
    public static TutorialCaption instance;

    public const float defaultBGWidth = 500;
    public const float defaultBGHeight = 150;

    [SerializeField]
    TextMeshProUGUI captionText;

    Image captionBG;
    Animator captionAnim;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        captionBG = GetComponent<Image>();
        captionAnim = GetComponent<Animator>();
    }

    public void ShowCaption(string newText, float width = defaultBGHeight, float height = defaultBGHeight)
    {
        captionBG.rectTransform.sizeDelta = new Vector2(width, height);
        captionAnim.Play("ShowCaption");
        captionText.text = newText;
    }

    public void HideCaption()
    {
        captionAnim.Play("HideCaption");
    }
}
