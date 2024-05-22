using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    [SerializeField] GameObject cameraObj;
    [SerializeField] GameObject hammerCameraObject;
    float shakeTimer;

    //this script needs easing
    public IEnumerator Shake(float duration, float timeBetweenShake, float buildTime, float fallTime, float power)
    {
        Vector3 orignalMainCamPos = cameraObj.transform.localPosition;
        Vector3 originalHammerCamPos = hammerCameraObject.transform.localPosition;

        float currentPower = 0;

        shakeTimer = 0;
        while (shakeTimer < buildTime)
        {
            shakeTimer += timeBetweenShake;
            float xPos = Random.Range(-currentPower, currentPower);
            float yPos = Random.Range(-currentPower, currentPower);

            cameraObj.transform.localPosition = new Vector3(xPos, yPos, orignalMainCamPos.z);
            //hammerCameraObject.transform.localPosition = new Vector3(-xPos, -yPos, originalHammerCamPos.z);

            currentPower = power * shakeTimer / buildTime;

            yield return new WaitForSeconds(timeBetweenShake);
        }

        currentPower = power;

        shakeTimer = duration;
        while (shakeTimer > 0)
        {
            shakeTimer -= timeBetweenShake;
            float xPos = Random.Range(-currentPower, currentPower);
            float yPos = Random.Range(-currentPower, currentPower);

            cameraObj.transform.localPosition = new Vector3(xPos, yPos, orignalMainCamPos.z);
            //hammerCameraObject.transform.localPosition = new Vector3(-xPos, -yPos, originalHammerCamPos.z);

            yield return new WaitForSeconds(timeBetweenShake);
        }

        shakeTimer = fallTime;
        while (shakeTimer > 0)
        {
            shakeTimer -= timeBetweenShake;
            float xPos = Random.Range(-currentPower, currentPower);
            float yPos = Random.Range(-currentPower, currentPower);

            cameraObj.transform.localPosition = new Vector3(xPos, yPos, orignalMainCamPos.z);
            //hammerCameraObject.transform.localPosition = new Vector3(-xPos, -yPos, originalHammerCamPos.z);

            currentPower = power * shakeTimer / fallTime;

            yield return new WaitForSeconds(timeBetweenShake);
        }

        cameraObj.transform.localPosition = orignalMainCamPos;
        hammerCameraObject.transform.localPosition = originalHammerCamPos;
    }

    public IEnumerator ContinuousShake()
    {
        //this will be a function we can use to do screenshake over a period of time we do not have a measure of, and there will be a secondary function that can be used to cut it off
        yield return null;
    }
}
