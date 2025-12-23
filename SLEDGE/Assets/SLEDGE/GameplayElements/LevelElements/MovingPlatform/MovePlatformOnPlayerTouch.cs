using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MovePlatformOnPlayerTouch : MonoBehaviour
{
    private SplineAnimate splineAnimator;

    void Awake()
    {
        splineAnimator = GetComponent<SplineAnimate>();
    }
    // Start is called before the first frame update
    void Start()
    {
        splineAnimator.PlayOnAwake = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            splineAnimator.Play();
        }
    }
}
