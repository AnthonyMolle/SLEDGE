using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSound : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip hit2;

    private void Start()
    {
        Source.PlayOneShot(hit2);
        Destroy(gameObject, 2);
    }
}
