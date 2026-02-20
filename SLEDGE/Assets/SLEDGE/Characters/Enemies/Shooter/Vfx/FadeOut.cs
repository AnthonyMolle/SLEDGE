using System.Data;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FadeoutData
{
    public bool swap;
    public bool hasAlpha;
    public Material baseMat;
    public Material swapMat;
}

public class FadeOut : MonoBehaviour
{
    public List<FadeoutData> fadeoutData = new List<FadeoutData>();
    public GameObject rig;
    public float startTime = 0f;
    public float fadeDuration = 1f;
    public AnimationCurve fadeCurve;
    public bool destroyOnDone;
    public bool fadingOut = false;

    public void Fade()
    {
        startTime = Time.time;
        fadingOut = true;
        SwapMaterials();
    }


    [ContextMenu("Swap Materials")]
    void SwapMaterials()
    {
        if (rig != null && fadeoutData != null && fadeoutData.Count > 0)
        {
            foreach (var data in fadeoutData)
            {
                if (data.baseMat != null && data.swapMat != null && data.swap)
                {
                    SwapMaterial(rig, data.baseMat, data.swapMat, data);
                }
            }
        }
    }

    public void ResetMaterials()
    {
        if (rig != null && fadeoutData != null && fadeoutData.Count > 0)
        {
            foreach (var data in fadeoutData)
            {
                if (data.baseMat != null && data.swapMat != null && data.swap)
                {
                    SwapMaterial(rig, data.swapMat, data.baseMat, data);
                }
            }
        }
    }

    void Update()
    {
        if (!fadingOut) return;
        if (fadeoutData != null && fadeoutData.Count > 0)
        {
            foreach (var data in fadeoutData)
            {
                if (data.baseMat != null && data.swapMat != null && data.swap && Time.time <= startTime + fadeDuration && data.hasAlpha)
                {
                    float alpha = fadeCurve.Evaluate((Time.time - startTime) / fadeDuration);
                    Color baseColor = data.baseMat.color;
                    Color swapColor = data.swapMat.color;

                    baseColor.a = 1f - alpha;
                    swapColor.a = alpha;

                    data.baseMat.color = baseColor;
                    data.swapMat.color = swapColor;
                }
            }
        }
    }

    void SwapMaterial(GameObject obj, Material oldMat, Material newMat, FadeoutData data)
    {
        Renderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (renderer.sharedMaterial == oldMat)
            {
                renderer.sharedMaterial = newMat;
            }
        }
        foreach (Transform child in obj.transform)
        {
            SwapMaterial(child.gameObject, oldMat, newMat, data);
        }
    }
}