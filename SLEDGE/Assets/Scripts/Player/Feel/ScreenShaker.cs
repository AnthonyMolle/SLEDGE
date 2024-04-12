using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    GameObject cameraObj;
    float shakeTimer;

    // Start is called before the first frame update
    void Start()
    {
        cameraObj = Camera.main.gameObject;
    }

    public void Shake(float duration, float power)
    {
        shakeTimer = duration;
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            float xPos = Random.Range(-power, power);
            float yPos = Random.Range(-power, power);

            cameraObj.transform.localPosition = new Vector3(xPos, yPos + 0.5f, cameraObj.transform.position.z);
        }

        cameraObj.transform.localPosition = new Vector3(0, 0.5f, 0);
    }
    
}
