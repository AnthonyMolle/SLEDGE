using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HammerSound : MonoBehaviour
{
    [SerializeField] private EventReference hammerSlamSound;
    public AudioSource Source;
    public AudioClip hit2;

    private void Start()
    {
        //Source.PlayOneShot(hit2);
        AudioManager.instance.PlayOneShot(hammerSlamSound, this.transform.position);
        Destroy(gameObject, 2);
    }
}
