using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSFXPlayer : MonoBehaviour
{
    [Header("Menu UI SFX Event References")]
    public FMODUnity.EventReference MenuHoverSFX;
    public FMODUnity.EventReference MenuSelectSFX;
    public FMODUnity.EventReference MenuEnterLevelSFX;


    public void PlayMenuHoverSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(MenuHoverSFX, transform.position);
    }

    public void PlayMenuSelectSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(MenuSelectSFX, transform.position);
    }

    public void PlayMenuEnterLevelSFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot(MenuEnterLevelSFX, transform.position);
    }
}
